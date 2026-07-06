using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class MoneyTransfer : BaseAuditableEntity
{

    public Guid FromMonetaryAccountId { get; private set; }
    public MonetaryAccount FromMonetaryAccount { get; private set; } = null!;

    public Guid ToMonetaryAccountId { get; private set; }
    public MonetaryAccount ToMonetaryAccount { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public byte CurrencyId { get; private set; }
    public Currency Currency { get; private set; } = null!;


    public DateTime TransferDate { get; private set; }

    private MoneyTransfer() { }

    public MoneyTransfer(Guid fromMonetaryAccountId, Guid toMonetaryAccountId, decimal amount, byte currencyId, DateTime transferDate,
                         Guid tenantId, Guid createdBy, string? description = null) : base(tenantId, createdBy,description)
    {
        SetMonetaryAccounts(fromMonetaryAccountId, toMonetaryAccountId);
        SetAmount(amount);
        SetCurrencyId(currencyId);
        SetTransferDate(transferDate);
     }

    public void UpdateMoneyTransfer(Guid fromMonetaryAccountId, Guid toMonetaryAccountId, decimal amount, byte currencyId,
                                    DateTime transferDate, string? description = null)
    {
        SetMonetaryAccounts(fromMonetaryAccountId, toMonetaryAccountId);


        SetAmount(amount);
        SetCurrencyId(currencyId);
        SetTransferDate(transferDate);

        SetDescription(description);
    }

    public void SetMonetaryAccounts(Guid fromMonetaryAccountId, Guid toMonetaryAccountId)
    {
        if (fromMonetaryAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.MoneyTransfer.FromMonetaryAccountRequired);

        if (toMonetaryAccountId == Guid.Empty)
            throw new DomainException(DomainErrors.MoneyTransfer.ToMonetaryAccountRequired);

        if (fromMonetaryAccountId == toMonetaryAccountId) throw new DomainException(DomainErrors.MoneyTransfer.SameAccount);

        FromMonetaryAccountId = fromMonetaryAccountId;
        ToMonetaryAccountId = toMonetaryAccountId;
    }


    public void SetAmount(decimal amount)
    {
        if (amount < 0)
            throw new DomainException(DomainErrors.MoneyTransfer.AmountMustBePositive);

        Amount = amount;
    }

    public void SetCurrencyId(byte currencyId) => CurrencyId = currencyId;

    public void SetTransferDate(DateTime transferDate)
    {
        if (transferDate > DateTime.UtcNow)
            throw new DomainException(DomainErrors.MoneyTransfer.MoneyTransferDateCannotBeInFuture);

        TransferDate = transferDate;
    }
}
