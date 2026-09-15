using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Constants;
using PersonalFinanceApp.Application.Common.Errors;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;
using PersonalFinanceApp.Infrastructure.Persistence;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionManager _transactionManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ITransactionManager transactionManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _context = context;
        _unitOfWork = unitOfWork;
        _transactionManager = transactionManager;
    }


    public async Task<IdentityRegistrationResult> CreateUserForExistingTenantAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                TenantId = tenantId
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new IdentityRegistrationResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Users invited into an existing tenant get the baseline Users role -
            // TenantAdministrators is granted only at tenant-creation time (RegisterAsync),
            // never here. If this app later needs "invite as tenant admin", that's a
            // separate, explicit parameter on this call - not an implicit upgrade path.
            if (!await _roleManager.RoleExistsAsync(Roles.Users))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Users));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Users);

            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new IdentityRegistrationResult
                {
                    Succeeded = false,
                    Errors = roleResult.Errors.Select(e => e.Description).ToList()
                };
            }

            await transaction.CommitAsync(cancellationToken);

            return new IdentityRegistrationResult
            {
                Succeeded = true,
                UserId = user.Id
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IdentityLoginResult> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)

    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new IdentityLoginResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid credentials." }
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return new IdentityLoginResult
            {
                Succeeded = false,
                Errors = new[] { "Invalid credentials." }
            };
        }

        // JWT bearer auth is stateless - [Authorize(Roles = ...)] reads role claims out
        // of the token itself, not a live DB lookup on every request. The user's current
        // roles have to be fetched and embedded here, or role checks can never pass no
        // matter what's assigned in the database.
        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.GenerateToken(user.Id, user.TenantId, user.Email!, roles);

        return new IdentityLoginResult
        {
            Succeeded = true,
            Token = token
        };
    }

    public async Task<IdentityRegistrationResult> RegisterAsync(
            string email,
            string password,
            string tenantName,
            string firstName,
            string lastName,
            int defaultLanguageId,
            int defaultCurrencyId,
            CancellationToken cancellationToken)
    {
        // fail fast, before opening the transaction, if the chosen language
        // doesn't have a complete set of AccountTypeTranslation rows. Without this,
        // the LedgerAccount-seeding loop further down throws an unguarded
        // KeyNotFoundException instead of a clean, translatable error.
        await EnsureLanguageHasCompleteAccountTypeTranslationsAsync(defaultLanguageId, cancellationToken);

        // Transaction guards against an orphaned Tenant if user creation fails
        // afterward (weak password, duplicate email, etc.) - both succeed together
        // or neither is persisted.
        await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var tenant = new Tenant(tenantName, defaultLanguageId, defaultCurrencyId);

            _context.Tenants.Add(tenant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                TenantId = tenant.Id,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new IdentityRegistrationResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(s => s.Description).ToList()
                };
            }

            // the user who registers is the tenant's owner - grant them the
            // Administrators role for their own tenant. Seed the role itself on first
            // use if it doesn't exist yet (participates in the same transaction/context).
            if (!await _roleManager.RoleExistsAsync(Roles.TenantAdministrators))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.TenantAdministrators));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.TenantAdministrators);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);

                return new IdentityRegistrationResult
                {
                    Succeeded = false,
                    Errors = roleResult.Errors.Select(s => s.Description).ToList()
                };
            }

            var accountTypes = await (
                from accountType in _context.AccountTypes
                join translation in _context.AccountTypeTranslations
                    on accountType.Id equals translation.AccountTypeId
                where translation.LanguageId == defaultLanguageId
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
                    user.Id,
                    displayOrder++,
                    accountTypes[category].Description);

                await _context.LedgerAccounts.AddAsync(ledgerAccount);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new IdentityRegistrationResult
            {
                Succeeded = true,
                UserId = user.Id
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // every AccountCategory value must have a translation row for the
    // requested language, or the LedgerAccount root-seeding loop above will fail.
    // Checked up front so a bad/incomplete language choice surfaces as a normal,
    // translatable BusinessRuleException instead of an unhandled exception mid-transaction.
    private async Task EnsureLanguageHasCompleteAccountTypeTranslationsAsync(
        int languageId, CancellationToken cancellationToken)
    {
        var languageExists = await _context.Languages.AnyAsync(l => l.Id == languageId, cancellationToken);
        if (!languageExists)
            throw new BusinessRuleException(ApplicationErrorCodes.Auth.DefaultLanguageNotFound, languageId);

        var translatedCategoryCount = await _context.AccountTypeTranslations
            .Where(t => t.LanguageId == languageId)
            .Select(t => t.AccountType.Category)
            .Distinct()
            .CountAsync(cancellationToken);

        var requiredCategoryCount = Enum.GetValues<AccountCategory>().Length;

        if (translatedCategoryCount < requiredCategoryCount)
            throw new BusinessRuleException(
                ApplicationErrorCodes.Auth.DefaultLanguageTranslationsIncomplete, languageId);
    }
}
