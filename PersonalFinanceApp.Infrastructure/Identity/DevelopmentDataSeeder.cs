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
using PersonalFinanceApp.Application.Common.Constants;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class DevelopmentDataSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    ITransactionManager transactionManager,
    IOptions<DevelopmentSeedOptions> options) : IDevelopmentDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await context.Tenants.AnyAsync(cancellationToken))
            return;

        await using var tx = await transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var tenant = new Tenant(
                name: options.Value.TenantName,
                defaultLanguageId: SeedIds.Languages.English,
                defaultCurrencyId: SeedIds.Currencies.Usd);

            context.Tenants.Add(tenant);
            await unitOfWork.SaveChangesAsync(cancellationToken);

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

            if (!await roleManager.RoleExistsAsync(Roles.SystemAdministrators))
                await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.SystemAdministrators));

            if (!await roleManager.RoleExistsAsync(Roles.TenantAdministrators))
                await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.TenantAdministrators));

            var systemAdminRoleAssignResult = await userManager.AddToRoleAsync(admin, Roles.SystemAdministrators);
            if (!systemAdminRoleAssignResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", systemAdminRoleAssignResult.Errors.Select(e => e.Description)));

            var tenantAdminRoleAssignResult = await userManager.AddToRoleAsync(admin, Roles.TenantAdministrators);
            if (!tenantAdminRoleAssignResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", tenantAdminRoleAssignResult.Errors.Select(e => e.Description)));

            var accountTypes = await (
            from accountType in context.AccountTypes
            join translation in context.AccountTypeTranslations
                on accountType.Id equals translation.AccountTypeId
            where translation.LanguageId == SeedIds.Languages.English
            select new
            {
                accountType.Category,
                accountType.Id,
                translation.Translation,
                translation.Description
            })
          .ToDictionaryAsync(
              x => x.Category,
              x => new
              {
                  x.Id,
                  x.Translation,
                  x.Description

              },
              cancellationToken);

            int displayOrder = 1;
            // Create LedgerAccount roots
            foreach (AccountCategory category in Enum.GetValues<AccountCategory>())
            {
                var ledgerAccount = new LedgerAccount(
                    accountTypes[category].Id,
                    accountTypes[category].Translation,
                    tenant.Id,
                    admin.Id,
                    displayOrder++,
                    accountTypes[category].Description);

                await context.LedgerAccounts.AddAsync(ledgerAccount);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
