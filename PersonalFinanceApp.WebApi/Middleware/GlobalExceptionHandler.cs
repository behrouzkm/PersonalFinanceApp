using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.WebApi.Middleware;

// Maps every exception the Application/Domain layers can throw to an HTTP status
// code and a small JSON body carrying ONLY a machine-readable error code (plus raw
// parameters where relevant) - never a formatted English sentence. Translating the
// code into the user's language is the client's job, not this API's - see the
// multi-lingual design decided earlier in this project.
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var correlationId = httpContext.TraceIdentifier;
        var (statusCode, body) = Map(exception,correlationId);

        // Full exception (with English message, stack trace) goes to the log only -
        // never to the client response.
        _logger.LogError(exception, "Request {CorrelationId} failed, mapped to {StatusCode}",correlationId, statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);

        return true;
    }

    private static (int StatusCode, object Body) Map(Exception exception,string correlationId) => exception switch
    {
        DomainException domainEx => (
            StatusCodes.Status400BadRequest,
            new { errorCode = domainEx.ErrorCode, parameters = Array.Empty<object>(),correlationId }),

        BusinessRuleException businessEx => (
            StatusCodes.Status400BadRequest,
            new { errorCode = businessEx.ErrorCode, parameters = businessEx.Parameters, correlationId }),

        NotFoundException notFoundEx => (
            StatusCodes.Status404NotFound,
            new { errorCode = notFoundEx.ErrorCode, entityName = notFoundEx.EntityName, key = notFoundEx.Key, correlationId }),

        ValidationException validationEx => (
            StatusCodes.Status400BadRequest,
            new { errorCode = ApplicationErrorCodes.Common.ValidationFailed, errors = validationEx.Errors, correlationId }),

        DbUpdateConcurrencyException => (
            StatusCodes.Status409Conflict,
            new { errorCode = ApplicationErrorCodes.Common.ConcurrencyConflict, parameters = Array.Empty<object>(), correlationId }),

        _ => (
            StatusCodes.Status500InternalServerError,
            new { errorCode = ApplicationErrorCodes.Common.UnexpectedError, parameters = Array.Empty<object>(), correlationId })
    };
}
