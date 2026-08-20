using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Infrastructure.Persistence;

namespace PersonalFinanceApp.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
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

        var token = _tokenService.GenerateToken(user.Id, user.TenantId, user.Email!);

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
            byte defaultLanguageId,
            byte defaultCurrencyId,
            CancellationToken cancellationToken)
    {

        // Transaction guards against an orphaned Tenant if user creation fails
        // afterward (weak password, duplicate email, etc.) - both succeed together
        // or neither is persisted.
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var tenant = new Tenant(tenantName, defaultLanguageId, defaultCurrencyId);

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

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

        await transaction.CommitAsync(cancellationToken);

        return new IdentityRegistrationResult
        {
            Succeeded = true,
            UserId = user.Id
        };
    }
}
