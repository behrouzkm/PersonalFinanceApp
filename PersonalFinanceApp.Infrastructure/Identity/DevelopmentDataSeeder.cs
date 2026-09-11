using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PersonalFinanceApp.Infrastructure.Persistence;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Infrastructure.Persistence.Seed;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class DevelopmentDataSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IOptions<DevelopmentSeedOptions> options) : IDevelopmentDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await context.Tenants.AnyAsync(cancellationToken))
            return; // idempotent

        await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        if (!await roleManager.RoleExistsAsync("Administrators"))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>("Administrators"));
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        var tenant = new Tenant(
            name: options.Value.TenantName,
            defaultLanguageId: SeedIds.Languages.English,
            defaultCurrencyId: SeedIds.Currencies.Usd);

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync(cancellationToken);

        var admin = new ApplicationUser
        {
            UserName = options.Value.AdminEmail,
            Email = options.Value.AdminEmail,
            TenantId = tenant.Id,
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(admin, options.Value.AdminPassword);
        if (!userResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", userResult.Errors.Select(e => e.Description)));

        var roleAssignResult = await userManager.AddToRoleAsync(admin, "Administrators");
        if (!roleAssignResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));

        await tx.CommitAsync(cancellationToken);
    }
}
