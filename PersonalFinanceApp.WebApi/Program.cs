using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PersonalFinanceApp.Application;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Infrastructure;
using PersonalFinanceApp.Infrastructure.Identity;
using PersonalFinanceApp.Infrastructure.Persistence;
using PersonalFinanceApp.WebApi.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context,services,configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiAuditLoggingMiddleware>();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync(); // applies HasData rows in every environment

    var seedOpts = scope.ServiceProvider.GetRequiredService<IOptions<DevelopmentSeedOptions>>().Value;

    if (app.Environment.IsProduction() && seedOpts.Enabled)
        throw new InvalidOperationException("DevelopmentSeed.Enabled must never be true in Production.");

    if (app.Environment.IsDevelopment() && seedOpts.Enabled)
        await scope.ServiceProvider.GetRequiredService<IDevelopmentDataSeeder>().SeedAsync(CancellationToken.None);
}


app.Run();

