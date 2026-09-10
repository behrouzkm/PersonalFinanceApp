using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Application.Common.Errors;

public static class ApplicationErrorCodes
{
    public static class Common
    {
        public const string NotFound = "ErrorCodes.Common.NotFound";
        public const string ValidationFailed = "ErrorCodes.Common.ValidationFailed";
        public const string ConcurrencyConflict = "ErrorCodes.Common.ConcurrencyConflict";
        public const string UnexpectedError = "ErrorCodes.Common.UnexpectedError";
        public const string DuplicateValueConflict = "ErrorCodes.Common.DuplicateValueConflict";

    }

    public static class AccountTypeTranslation
    {
        public const string AccountTypeTranslationIdRequired = "ErrorCodes.AccountTypeTranslation.AccountTypeTranslationIdRequired";
        public const string LanguageIdRequired = "ErrorCodes.AccountTypeTranslation.LanguageIdRequired";
        public const string TranslationRequired = "ErrorCodes.AccountTypeTranslation.TranslationRequired";
        public const string DuplicateRecord = "ErrorCodes.AccountTypeTranslation.DuplicateRecord";
    }

    public static class Attachment
    {
        public const string OwnerIdRequired = "ErrorCodes.Attachment.OwnerIdRequired";
        public const string FileNameRequired = "ErrorCodes.Attachment.FileNameRequired";
        public const string UnsupportedContentType = "ErrorCodes.Attachment.UnsupportedContentType";
        public const string FileTooLarge = "ErrorCodes.Attachment.FileTooLarge";
    }

    public static class Currency
    {
        public const string IdRequired = "ErrorCodes.Currency.IdRequired";
        public const string CodeRequired = "ErrorCodes.Currency.CodeRequired";
        public const string InvalidCurrencyCode = "ErrorCodes.Currency.InvalidCurrencyCode";
        public const string NameRequired = "ErrorCodes.Currency.NameRequired";
        public const string DecimalRequired = "ErrorCodes.Currency.DecimalRequired";
        public const string InvalidDecimalPlaces = "ErrorCodes.Currency.InvalidDecimalPlaces";
        public const string SymbolRequired = "ErrorCodes.Currency.SymbolRequired";
        public const string DuplicateCodeOrName = "ErrorCodes.Currency.DuplicateCodeOrName";
        public const string CurrencyInUse = "ErrorCodes.Currency.CurrencyInUse";
        public const string InvalidDisplayOrder = "ErrorCodes.Currency.InvalidDisplayOrder";
         public const string CurrencyDeactivated = "ErrorCodes.Currency.CurrencyDeactivated";
   }

    public static class Language
    {
        public const string IdRequired = "ErrorCodes.Language.IdRequired";
        public const string CodeRequired = "ErrorCodes.Language.CodeRequired";
        public const string InvalidLanguageCode = "ErrorCodes.Language.InvalidLanguageCode";
        public const string NameRequired = "ErrorCodes.Language.NameRequired";
        public const string LanguageDeactivated = "ErrorCodes.Language.LanguageDeactivated";
        //public const string InvalidDecimalPlaces = "ErrorCodes.Language.InvalidDecimalPlaces";
        //public const string SymbolRequired = "ErrorCodes.Language.SymbolRequired";
        public const string DuplicateCodeOrName = "ErrorCodes.Language.DuplicateCodeOrName";
        public const string LanguageInUse = "ErrorCodes.Language.LanguageInUse";
        public const string InvalidDisplayOrder = "ErrorCodes.Language.InvalidDisplayOrder";
    }

    public static class Auth
    {
        public const string EmailRequired = "ErrorCodes.Auth.EmailRequired";
        public const string EmailInvalid = "ErrorCodes.Auth.EmailInvalid";
        public const string PasswordRequired = "ErrorCodes.Auth.PasswordRequired";
        public const string TenantNameRequired = "ErrorCodes.Auth.TenantNameRequired";
        public const string FirstNameRequired = "ErrorCodes.Auth.FirstNameRequired";
        public const string LastNameRequired = "ErrorCodes.Auth.LastNameRequired";
        public const string DefaultLanguageRequired = "ErrorCodes.Auth.DefaultLanguageRequired";
        public const string DefaultCurrencyRequired = "ErrorCodes.Auth.DefaultCurrencyRequired";
        public const string RegistrationFailed = "ErrorCodes.Auth.RegistrationFailed";
        public const string LoginFailed = "ErrorCodes.Auth.LoginFailed";
        public const string TenantInactive = "ErrorCodes.Auth.TenantInactive";
        public const string DefaultLanguageNotFound = "ErrorCodes.Auth.DefaultLanguageNotFound";
        public const string DefaultLanguageTranslationsIncomplete = "ErrorCodes.Auth.DefaultLanguageTranslationsIncomplete";
  }

    public static class ExpenditureList
    {
        public const string FromLaterThanToDate = "ErrorCodes.ExpenditureList.FromLaterThanToDate";
    }

    public static class Expenditure
    {
        public const string ExpenseAccountNotPostable = "ErrorCodes.Expenditure.ExpenseAccountNotPostable";
        public const string InsufficientBalance = "ErrorCodes.Expenditure.InsufficientBalance";
        public const string DocumentDateInFuture = "ErrorCodes.Expenditure.DocumentDateInFuture";
        public const string DocumentDateRequired = "ErrorCodes.Expenditure.DocumentDateRequired";
        public const string CurrencyRequired = "ErrorCodes.Expenditure.CurrencyRequired";
        public const string LinesRequired = "ErrorCodes.Expenditure.LinesRequired";
        public const string ExpenseAccountRequired = "ErrorCodes.Expenditure.ExpenseAccountRequired";
        public const string LineAmountMustBePositive = "ErrorCodes.Expenditure.LineAmountMustBePositive";
        public const string PaymentsRequired = "ErrorCodes.Expenditure.PaymentsRequired";
        public const string PaymentAmountMustBePositive = "ErrorCodes.Expenditure.PaymentAmountMustBePositive";
        public const string MonetaryAccountRequired = "ErrorCodes.Expenditure.MonetaryAccountRequired";
        public const string PersonRequired = "ErrorCodes.Expenditure.PersonRequired";
        public const string NotBalanced = "ErrorCodes.Expenditure.NotBalanced";

        public const string AccountingDocumentIdRequired = "ErrorCodes.Expenditure.AccountingDocumentIdRequired";
        public const string RowVersionRequired = "ErrorCodes.Expenditure.RowVersionRequired";
        public const string EntryNotFoundOnDocument = "ErrorCodes.Expenditure.EntryNotFoundOnDocument";
    }

    public static class Income
    {
        public const string IncomeAccountNotPostable = "ErrorCodes.Income.IncomeAccountNotPostable";
        public const string InsufficientBalance = "ErrorCodes.Income.InsufficientBalance";
        public const string DocumentDateInFuture = "ErrorCodes.Income.DocumentDateInFuture";
        public const string DocumentDateRequired = "ErrorCodes.Income.DocumentDateRequired";
        public const string CurrencyRequired = "ErrorCodes.Income.CurrencyRequired";
        public const string LinesRequired = "ErrorCodes.Income.LinesRequired";
        public const string IncomeAccountRequired = "ErrorCodes.Income.IncomeAccountRequired";
        public const string LineAmountMustBePositive = "ErrorCodes.Income.LineAmountMustBePositive";
        //public const string PaymentsRequired = "ErrorCodes.Income.PaymentsRequired";
        public const string IncomeAmountMustBePositive = "ErrorCodes.Income.IncomeAmountMustBePositive";
        public const string MonetaryAccountEntriesRequired = "ErrorCodes.Income.MonetaryAccountEntriesRequired";
        public const string MonetaryAccountRequired = "ErrorCodes.Income.MonetaryAccountRequired";
        //public const string PersonRequired = "ErrorCodes.Income.PersonRequired";
        public const string NotBalanced = "ErrorCodes.Income.NotBalanced";

        public const string AccountingDocumentIdRequired = "ErrorCodes.Income.AccountingDocumentIdRequired";
        public const string RowVersionRequired = "ErrorCodes.Income.RowVersionRequired";
        public const string EntryNotFoundOnDocument = "ErrorCodes.Income.EntryNotFoundOnDocument";
    }

    public static class MoneyTransfer
    {
        public const string TransferDocumentIdRequired = "ErrorCodes.MoneyTransfer.TransferDocumentIdRequired";
        public const string CurrencyRequired = "ErrorCodes.MoneyTransfer.CurrencyRequired";
        public const string SourceDestinationCurrencyMismatch = "ErrorCodes.MoneyTransfer.SourceDestinationCurrencyMismatch";
        public const string TransferDateRequired = "ErrorCodes.MoneyTransfer.TransferDateRequired";
        public const string TransferDateInFuture = "ErrorCodes.MoneyTransfer.TransferDateInFuture";
        public const string FromMonetaryAccountIdRequired = "ErrorCodes.MoneyTransfer.FromMonetaryAccountIdRequired";
        public const string ToMonetaryAccountIdRequired = "ErrorCodes.MoneyTransfer.ToMonetaryAccountIdRequired";
        public const string SourceAndDestinationMustDiffer = "ErrorCodes.MoneyTransfer.SourceAndDestinationMustDiffer";
        public const string TransferAmountMustBePositive = "ErrorCodes.MoneyTransfer.TransferAmountMustBePositive";
        public const string InsufficientBalance = "ErrorCodes.MoneyTransfer.InsufficientBalance";
        public const string RowVersionRequired = "ErrorCodes.MoneyTransfer.RowVersionRequired";
        public const string CreditEntryNotFound = "ErrorCodes.MoneyTransfer.CreditEntryNotFound";
        public const string DebitEntryNotFound = "ErrorCodes.MoneyTransfer.DebitEntryNotFound";
        public const string FromMonetaryAccountNotFound = "ErrorCodes.MoneyTransfer.FromMonetaryAccountNotFound";
        public const string ToMonetaryAccountNotFound = "ErrorCodes.MoneyTransfer.ToMonetaryAccountNotFound";
        public const string FromLaterThanToDate = "ErrorCodes.MoneyTransfer.FromLaterThanToDate";
    }

    public static class CurrencyExchange
    {
        public const string DocumentIdRequired = "ErrorCodes.CurrencyExchange.DocumentIdRequired";
        public const string ClearingEntryNotFound = "ErrorCodes.CurrencyExchange.ClearingEntryNotFound";
        public const string FundSourceEntryNotFound = "ErrorCodes.CurrencyExchange.FundSourceEntryNotFound";
        public const string SameCurrencyExchangeNotAllowed = "ErrorCodes.CurrencyExchange.SameCurrencyExchangeNotAllowed";
        public const string ExchangeDateRequired = "ErrorCodes.CurrencyExchange.ExchangeDateRequired";
        public const string ExchangeDateInFuture = "ErrorCodes.CurrencyExchange.ExchangeDateInFuture";
        public const string FromLedgerAccountRequired = "ErrorCodes.CurrencyExchange.FromLedgerAccountRequired";
        public const string ToLedgerAccountRequired = "ErrorCodes.CurrencyExchange.ToLedgerAccountRequired";
        public const string SourceAndDestinationMustDiffer = "ErrorCodes.CurrencyExchange.SourceAndDestinationMustDiffer";
        public const string AmountRateMismatch = "ErrorCodes.CurrencyExchange.AmountRateMismatch";
        public const string FromAmountMustBePositive = "ErrorCodes.CurrencyExchange.FromAmountMustBePositive";
        public const string ToAmountMustBePositive = "ErrorCodes.CurrencyExchange.ToAmountMustBePositive";
        public const string ExchangeRateMustBePositive = "ErrorCodes.CurrencyExchange.ExchangeRateMustBePositive";
        public const string InsufficientBalance = "ErrorCodes.CurrencyExchange.InsufficientBalance";
        public const string RowVersionRequired = "ErrorCodes.CurrencyExchange.RowVersionRequired";
        public const string CreditEntryNotFound = "ErrorCodes.CurrencyExchange.CreditEntryNotFound";
        public const string DebitEntryNotFound = "ErrorCodes.CurrencyExchange.DebitEntryNotFound";
        public const string FromMonetaryAccountNotFound = "ErrorCodes.CurrencyExchange.FromMonetaryAccountNotFound";
        public const string ToMonetaryAccountNotFound = "ErrorCodes.CurrencyExchange.ToMonetaryAccountNotFound";
    }

    public static class FundSource
    {
        public const string InitialDebtExceedsCreditLimit = "ErrorCodes.FundSource.InitialDebtExceedsCreditLimit";
        public const string InvalidParentLedgerAccount = "ErrorCodes.FundSource.InvalidParentLedgerAccount";
        public const string InvalidOpeningAccountEquityLedgerAccount = "ErrorCodes.FundSource.InvalidOpeningAccountEquityLedgerAccount";
        public const string OpeningDateCannotBeAfterExistingTransactions = "ErrorCodes.FundSource.OpeningDateCannotBeAfterExistingTransactions";
        public const string CannotChangeCurrencyWithExistingTransactions = "ErrorCodes.FundSource.CannotChangeCurrencyWithExistingTransactions";
    }

    public static class Person
    {
        public const string PersonIdRequired = "ErrorCodes.Person.PersonIdRequired";
        public const string DisplayNameRequired = "ErrorCodes.Person.DisplayNameRequired";
        public const string CurrencyRequired = "ErrorCodes.Person.CurrencyRequired";
        public const string InvalidCreditLimit = "ErrorCodes.Person.InvalidCreditLimit";
        public const string InvalidParentLedgerId = "ErrorCodes.Person.InvalidParentLedgerId";
        public const string InvalidEmailAddress = "ErrorCodes.Person.InvalidEmailAddress";
        public const string MobileNumberIsTooLong = "ErrorCodes.Person.MobileNumberIsTooLong";
        public const string TelNumberIsTooLong = "ErrorCodes.Person.TelNumberIsTooLong";
        public const string EmailAddressIsTooLong = "ErrorCodes.Person.EmailAddressIsTooLong";
        public const string OpeningDateRequired = "ErrorCodes.Person.OpeningDateRequired";
        public const string OpeningDateInFuture = "ErrorCodes.Person.OpeningDateInFuture";
        public const string InvalidParentLedgerAccount = "ErrorCodes.Person.InvalidParentLedgerAccount";
        public const string InvalidOpeningAccountEquityLedgerAccount = "ErrorCodes.Person.InvalidOpeningAccountEquityLedgerAccount";
        public const string OpeningDateCannotBeAfterExistingTransactions = "ErrorCodes.Person.OpeningDateCannotBeAfterExistingTransactions";
        public const string CannotDeleteWithAccountingHistory = "ErrorCodes.Person.CannotDeleteWithAccountingHistory";
        public const string InvalidDisplayOrder = "ErrorCodes.Person.InvalidDisplayOrder";
    }


    public static class BankAccount
    {
        public const string BankAccountIdRequired = "ErrorCodes.BankAccount.BankAccountIdRequired";
        public const string DisplayNameRequired = "ErrorCodes.BankAccount.DisplayNameRequired";
        public const string CurrencyRequired = "ErrorCodes.BankAccount.CurrencyRequired";
        public const string CreditLimitMustBePositive = "ErrorCodes.BankAccount.CreditLimitMustBePositive";
        public const string InvalidParentLedgerId = "ErrorCodes.BankAccount.InvalidParentLedgerId";
        public const string InvalidBankName = "ErrorCodes.BankAccount.InvalidBankName";
        public const string InvalidBankAccountNo = "ErrorCodes.BankAccount.InvalidBankAccountNo";
        public const string TelNumberIsTooLong = "ErrorCodes.BankAccount.TelNumberIsTooLong";
        public const string OpeningDateRequired = "ErrorCodes.BankAccount.OpeningDateRequired";
        public const string OpeningDateInFuture = "ErrorCodes.BankAccount.OpeningDateInFuture";
        public const string InvalidParentLedgerAccount = "ErrorCodes.BankAccount.InvalidParentLedgerAccount";
        public const string InvalidOpeningAccountEquityLedgerAccount = "ErrorCodes.BankAccount.InvalidOpeningAccountEquityLedgerAccount";
        public const string OpeningDateCannotBeAfterExistingTransactions = "ErrorCodes.BankAccount.OpeningDateCannotBeAfterExistingTransactions";
        public const string CannotDeleteWithAccountingHistory = "ErrorCodes.BankAccount.CannotDeleteWithAccountingHistory";
        public const string InvalidDisplayOrder = "ErrorCodes.BankAccount.InvalidDisplayOrder";
    }

    public static class CashAccount
    {
        public const string CashAccountIdRequired = "ErrorCodes.CashAccount.CashAccountIdRequired";
        public const string DisplayNameRequired = "ErrorCodes.CashAccount.DisplayNameRequired";
        public const string LocationRequired = "ErrorCodes.CashAccount.LocationRequired";
        public const string CurrencyRequired = "ErrorCodes.CashAccount.CurrencyRequired";
        public const string CreditLimitMustBePositive = "ErrorCodes.CashAccount.CreditLimitMustBePositive";
        public const string InvalidParentLedgerId = "ErrorCodes.CashAccount.InvalidParentLedgerId";
        public const string InvalidCashName = "ErrorCodes.CashAccount.InvalidCashName";
        public const string InvalidCashAccountNo = "ErrorCodes.CashAccount.InvalidCashAccountNo";
        public const string TelNumberIsTooLong = "ErrorCodes.CashAccount.TelNumberIsTooLong";
        public const string OpeningDateRequired = "ErrorCodes.CashAccount.OpeningDateRequired";
        public const string OpeningDateInFuture = "ErrorCodes.CashAccount.OpeningDateInFuture";
        public const string InvalidParentLedgerAccount = "ErrorCodes.CashAccount.InvalidParentLedgerAccount";
        public const string InvalidOpeningAccountEquityLedgerAccount = "ErrorCodes.CashAccount.InvalidOpeningAccountEquityLedgerAccount";
        public const string OpeningDateCannotBeAfterExistingTransactions = "ErrorCodes.CashAccount.OpeningDateCannotBeAfterExistingTransactions";
        public const string CannotDeleteWithAccountingHistory = "ErrorCodes.CashAccount.CannotDeleteWithAccountingHistory";
        public const string InvalidDisplayOrder = "ErrorCodes.CashAccount.InvalidDisplayOrder";
    }

    public static class LedgerAccount
    {
        public const string LedgerAccountIdRequired = "ErrorCodes.LedgerAccount.LedgerAccountIdRequired";
        public const string NameRequired = "ErrorCodes.LedgerAccount.NameRequired";
        public const string InvalidParentLedgerId = "ErrorCodes.LedgerAccount.InvalidParentLedgerId";
        public const string InvalidParentLedgerAccount = "ErrorCodes.LedgerAccount.InvalidParentLedgerAccount";
        public const string CannotDeleteWithAccountingHistory = "ErrorCodes.LedgerAccount.CannotDeleteWithAccountingHistory";
        public const string InvalidDisplayOrder = "ErrorCodes.LedgerAccount.InvalidDisplayOrder";
        public const string InvalidAccountTypeId = "ErrorCodes.LedgerAccount.InvalidAccountTypeId";
        public const string CannotDeleteDirectly = "ErrorCodes.LedgerAccount.CannotDeleteDirectly";
    }
}
