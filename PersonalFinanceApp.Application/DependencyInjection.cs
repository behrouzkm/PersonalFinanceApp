using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using PersonalFinanceApp.Application.Common.Behaviors;
using System.Reflection;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Services;

namespace PersonalFinanceApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IAccountingLookupService, AccountingLookupService>();
        services.AddScoped<ILedgerBalanceValidationService, LedgerBalanceValidationService>();
        services.AddScoped<IOpeningBalanceService, OpeningBalanceService>();
        services.AddScoped<IReorderService, ReorderService>();


        services.AddScoped<IAttachmentService, AttachmentService>();
 

        return services;
    }
}
