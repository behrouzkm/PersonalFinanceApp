using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Errors;

public static class DomainErrors
{
    public static class Person
    {
        public const string DisplayNameRequired = "ErrorCodes.Person.DisplayNameRequired";
        public const string LedgerAccountRequired = "ErrorCodes.Person.LedgerAccountRequired";
        public const string CurrencyRequired = "ErrorCodes.Person.CurrencyRequired";
        public const string InvalidEmailFormat = "ErrorCodes.Person.InvalidEmailFormat";
        public const string DisplayOrderCannotBeNegative = "ErrorCodes.Person.DisplayOrderCannotBeNegative";
        public const string InvalidMobileNumberFormat = "ErrorCodes.Person.InvalidMobileNumberFormat";
        public const string InvalidTelNumberFormat = "ErrorCodes.Person.InvalidTelNumberFormat";

    }

    public static class Attachment
    {
        // public const string ReferenceTypeRequired = "ErrorCodes.Attachment.ReferenceTypeRequired";
        // public const string ReferenceIdRequired = "ErrorCodes.Attachment.ReferenceIdRequired";
        public const string FileNameRequired = "ErrorCodes.Attachment.FileNameRequired";
        public const string FilePathRequired = "ErrorCodes.Attachment.FilePathRequired";
        public const string FileContentRequired = "ErrorCodes.Attachment.FileContentRequired";
    }

    public static class LedgerAccount
    {
        public const string NameRequired = "ErrorCodes.LedgerAccount.NameRequired";
        public const string CannotSetParentAsPosting = "ErrorCodes.LedgerAccount.CannotSetParentAsPosting";
        public const string CannotModifyUsedAccount = "ErrorCodes.LedgerAccount.CannotModifyUsedAccount";

    }

    public static class DocumentType
    {
        public const string NameRequired = "ErrorCodes.DocumentType.NameRequired";
    }
    // public static class AccountType
    // {
    //     public const string NameRequired = "ErrorCodes.AccountType.NameRequired";
    // }

    public static class Language
    {
        public const string NameRequired = "ErrorCodes.Language.NameRequired";
        public const string CodeRequired = "ErrorCodes.Language.CodeRequired";
    }

    public static class Tenant
    {
        public const string NameRequired = "ErrorCodes.Tenant.NameRequired";
    }


    public static class Currency
    {
        public const string CodeRequired = "ErrorCodes.Currency.CodeRequired";
        public const string CodeMustBeThreeLetters = "ErrorCodes.Currency.CodeMustBeThreeLetters";
        public const string NameRequired = "ErrorCodes.Currency.NameRequired";
        public const string DecimalPlacesInvalid = "ErrorCodes.Currency.DecimalPlacesInvalid";
        public const string SymbolRequired = "ErrorCodes.Currency.SymbolRequired";
        public const string DecimalPlacesTooHigh = "ErrorCodes.Currency.DecimalPlacesTooHigh";
    }

    public static class MonetaryAccount
    {
        public const string NameRequired = "ErrorCodes.MonetaryAccount.NameRequired";
        public const string CurrencyRequired = "ErrorCodes.MonetaryAccount.CurrencyRequired";
        public const string LedgerAccountRequired = "ErrorCodes.MonetaryAccount.LedgerAccountRequired";
        public const string InitialBalanceCannotBeNegative = "ErrorCodes.MonetaryAccount.InitialBalanceCannotBeNegative";
        public const string DisplayOrderCannotBeNegative = "ErrorCodes.MonetaryAccount.DisplayOrderCannotBeNegative";
        public const string CurrentBalanceCannotBeNegative = "ErrorCodes.MonetaryAccount.CurrentBalanceCannotBeNegative";
    }

    public static class CashAccount
    {
        public const string LocationRequired = "ErrorCodes.CashAccount.LocationRequired";
    }
    public static class BankAccount
    {
        public const string BankNameRequired = "ErrorCodes.BankAccount.BankNameRequired";
        public const string BankAccountNumberRequired = "ErrorCodes.BankAccount.BankAccountNumberRequired";
    }

    public static class AccountingDocument
    {
        public const string DocumentTypeRequired = "ErrorCodes.AccountingDocument.DocumentTypeRequired";
        public const string DocumentDateCannotBeInFuture = "ErrorCodes.AccountingDocument.DocumentDateCannotBeInFuture";
        public const string EntryRequired = "ErrorCodes.AccountingDocument.EntryRequired";
        public const string DuplicateEntryNotAllowed = "ErrorCodes.AccountingDocument.DuplicateEntryNotAllowed";
        public const string CurrencyMismatch = "ErrorCodes.AccountingDocument.CurrencyMismatch";
        public const string CurrencyRequired = "ErrorCodes.AccountingDocument.CurrencyRequired";
    }

    public static class AccountingEntry
    {
        public const string DocumentRequired = "ErrorCodes.AccountingEntry.DocumentRequired";
        public const string LedgerAccountRequired = "ErrorCodes.AccountingEntry.LedgerAccountRequired";
        public const string NegativeAmountNotAllowed = "ErrorCodes.AccountingEntry.NegativeAmountNotAllowed";
        public const string CannotBeBothDebitAndCredit = "ErrorCodes.AccountingEntry.CannotBeBothDebitAndCredit";
        public const string DebitAndCreditCannotBothBeZero = "ErrorCodes.AccountingEntry.DebitAndCreditCannotBothBeZero";
    }

    public static class MoneyTransfer
    {
        public const string AmountMustBePositive = "ErrorCodes.MoneyTransfer.AmountMustBePositive";
        public const string MoneyTransferDateCannotBeInFuture = "ErrorCodes.MoneyTransfer.MoneyTransferDateCannotBeInFuture";
        public const string FromMonetaryAccountRequired = "ErrorCodes.MoneyTransfer.FromMonetaryAccountRequired";
        public const string ToMonetaryAccountRequired = "ErrorCodes.MoneyTransfer.ToMonetaryAccountRequired";
        public const string SameAccount = "ErrorCodes.MoneyTransfer.SameAccount";

    }

    public static class SystemTemplate
    {
        public const string TemplateKeyRequired = "ErrorCodes.SystemTemplate.TemplateKeyRequired";
        public const string JsonDataRequired = "ErrorCodes.SystemTemplate.JsonDataRequired";
    }
}
