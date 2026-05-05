using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence;

public class FinanceDbContext : DbContext
{

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {

    }


    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Account
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

            entity.Property(a => a.Type)
            .IsRequired();
        });

        //JournalEntry
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Date)
            .IsRequired();

            entity.HasMany(x => x.Lines)
                .WithOne()
                .HasForeignKey("JournalEntryId");

            entity.Navigation(x => x.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        });

        //JournalLine
        modelBuilder.Entity<JournalLine>(entity =>
        {
            entity.HasKey("Id");

            entity.Property(x => x.Debit)
            .HasColumnType("decimal(18,2)");

            entity.Property(x => x.Credit)
            .HasColumnType("decimal(18,2)");

        });
    }
}
