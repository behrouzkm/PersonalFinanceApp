using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class CurrencyExchange : BaseAuditableEntity
{
    public Guid FromDocumentId { get; private set; }
    public AccountingDocument FromDocument { get; private set; } = null!;

    public Guid ToDocumentId { get; private set; }
    public AccountingDocument ToDocument { get; private set; } = null!;

    public decimal ExchangeRate { get; private set; }

    [Timestamp]
    public byte[] RowVersion { get; private set; } = default!;

    private CurrencyExchange() { }

    public CurrencyExchange(Guid fromDocumentId, Guid toDocumentId, decimal exchangeRate,
                         Guid tenantId, Guid createdBy, string? description = null) : base(tenantId, createdBy, description)
    {
        SetAccountingDocuments(fromDocumentId, toDocumentId);
        SetExchangeRate(exchangeRate);
    }

    public void UpdateExchangeRate(decimal exchangeRate, Guid modifiedBy, string? description = null)
    {
        SetExchangeRate(exchangeRate);
        SetDescription(description);

        UpdateAudit(modifiedBy);
    }

    private void SetAccountingDocuments(Guid fromDocumentId, Guid toDocumentId)
    {
        if (fromDocumentId == Guid.Empty)
            throw new DomainException(DomainErrors.CurrencyExchange.FromDocumentIdRequired);

        if (toDocumentId == Guid.Empty)
            throw new DomainException(DomainErrors.CurrencyExchange.ToDocumentIdRequired);

        if (fromDocumentId == toDocumentId) throw new DomainException(DomainErrors.CurrencyExchange.FromAndToAccountCantBeSame);

        FromDocumentId = fromDocumentId;
        ToDocumentId = toDocumentId;
    }


    private void SetExchangeRate(decimal rate)
    {
        if (rate <= 0)
            throw new DomainException(DomainErrors.CurrencyExchange.ExchangeRateMustBePositive);

        ExchangeRate = rate;
    }

}
