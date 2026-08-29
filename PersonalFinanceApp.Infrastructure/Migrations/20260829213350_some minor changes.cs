using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class someminorchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_LedgerAccountId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_CashAccounts_LedgerAccountId",
                table: "CashAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_LedgerAccountId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AccountTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "OpeningAccountingDocumentId",
                table: "Persons",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_LedgerAccountId",
                table: "Persons",
                column: "LedgerAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_OpeningAccountingDocumentId",
                table: "Persons",
                column: "OpeningAccountingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_LedgerAccountId",
                table: "CashAccounts",
                column: "LedgerAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_LedgerAccountId",
                table: "BankAccounts",
                column: "LedgerAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_Category",
                table: "AccountTypes",
                column: "Category",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_AccountingDocuments_OpeningAccountingDocumentId",
                table: "Persons",
                column: "OpeningAccountingDocumentId",
                principalTable: "AccountingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_AccountingDocuments_OpeningAccountingDocumentId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_LedgerAccountId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_OpeningAccountingDocumentId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_CashAccounts_LedgerAccountId",
                table: "CashAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_LedgerAccountId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypes_Category",
                table: "AccountTypes");

            migrationBuilder.DropColumn(
                name: "OpeningAccountingDocumentId",
                table: "Persons");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AccountTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_LedgerAccountId",
                table: "Persons",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_LedgerAccountId",
                table: "CashAccounts",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_LedgerAccountId",
                table: "BankAccounts",
                column: "LedgerAccountId");
        }
    }
}
