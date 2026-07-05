using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Exceptions;
using ApplicationValidationException = PersonalFinanceApp.Application.Common.Exceptions.ValidationException;

namespace PersonalFinanceApp.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> :
                        MediatR.IPipelineBehavior<TRequest, TResponse>
                        where TRequest : MediatR.IRequest<TResponse>
{

    private readonly IEnumerable<FluentValidation.IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<FluentValidation.IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {

        if (!_validators.Any())
        {
            return await next();
        }

        var context = new FluentValidation.ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

        if (failures.Count != 0)
        {
            throw new ApplicationValidationException(failures);
        }

        return await next();
    }
}
