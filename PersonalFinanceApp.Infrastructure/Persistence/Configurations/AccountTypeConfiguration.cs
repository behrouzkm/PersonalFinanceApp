using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Category).HasConversion<int>();
        builder.Property(a => a.NormalBalance).HasConversion<int>();


        builder.HasIndex(a => a.Category).IsUnique();

        builder.HasData(
            new { Id = 1, Category = AccountCategory.ExpenseAccount, CanDeleteDirectly = true, NormalBalance = NormalBalance.Debit },
            new { Id = 2, Category = AccountCategory.IncomeAccount, CanDeleteDirectly = true, NormalBalance = NormalBalance.Credit },
            new { Id = 3, Category = AccountCategory.PersonAccount, CanDeleteDirectly = false, NormalBalance = NormalBalance.FloatingBalance },
            new { Id = 4, Category = AccountCategory.BankAccount, CanDeleteDirectly = false, NormalBalance = NormalBalance.FloatingBalance },
            new { Id = 5, Category = AccountCategory.CashAccount, CanDeleteDirectly = false, NormalBalance = NormalBalance.Debit },
            new { Id = 6, Category = AccountCategory.CurrencyExchangeClearing, CanDeleteDirectly = false, NormalBalance = NormalBalance.FloatingBalance },
            new { Id = 7, Category = AccountCategory.OpeningBalanceEquity, CanDeleteDirectly = false, NormalBalance = NormalBalance.FloatingBalance }
        );
    }
}

