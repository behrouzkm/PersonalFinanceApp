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
}
