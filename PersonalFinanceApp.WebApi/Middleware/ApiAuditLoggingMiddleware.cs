using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.WebApi.Middleware;

public class ApiAuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiAuditLoggingMiddleware> _logger;

    public ApiAuditLoggingMiddleware(RequestDelegate next, ILogger<ApiAuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsynk(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            try
            {
                await WriteAuditLogAsync(context, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write API audit log for {Method} {Path}", context.Request.Method, context.Request.Path);
            }
        }
    }

    private static async Task WriteAuditLogAsync(HttpContext context, long durationMs)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tenantIdClaim = context.User.FindFirstValue("tenant_id");

        Guid? userId = Guid.TryParse(userIdClaim, out var uid) ? uid : null;
        Guid? tenantId = Guid.TryParse(tenantIdClaim, out var tid) ? tid : null;

        var controllerActionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var auditLog = new ApiAuditLog(
            userId,
            tenantId,
            context.Request.Method,
            context.Request.Path,
            controllerActionDescriptor?.ControllerName,
            controllerActionDescriptor?.ActionName,
            context.Response.StatusCode,
            durationMs,
            context.TraceIdentifier,
            context.Connection.RemoteIpAddress?.ToString(),
            context.Request.Headers.UserAgent.ToString());

        using var scope = context.RequestServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        dbContext.ApiAuditLogs.Add(auditLog);
        await dbContext.SaveChangesAsync(CancellationToken.None);

    }
}
