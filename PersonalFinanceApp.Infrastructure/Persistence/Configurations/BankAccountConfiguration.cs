using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Infrastructure.Persistence.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable("BankAccounts");

        builder.Property(b => b.BankName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.BranchName).HasMaxLength(200);
        builder.Property(b => b.BankAccountNumber).IsRequired().HasMaxLength(50);
        builder.Property(b => b.IBAN).HasMaxLength(50);
        builder.Property(b => b.BankAccountType).HasConversion<int>();
    }
}
