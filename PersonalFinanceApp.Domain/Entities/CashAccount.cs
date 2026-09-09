using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class CashAccount : MonetaryAccount
{
    public string Location { get; private set; } = string.Empty;
    public bool IsPhysical { get; private set; } = true;

    private CashAccount() { }


    public CashAccount(
        string displayName,
        Guid ledgerAccountId,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
        int displayOrder,
        string location,
        Guid tenantId,
        Guid createdBy,
        bool isPhysical = true,
        string? description = null,
        Guid? openingAccountingDocumentId = null) :
                        base(displayName, ledgerAccountId, currencyId, openingDate, initialBalance,
                            displayOrder, tenantId, createdBy, 0, description, openingAccountingDocumentId)
    {
        ChangeLocation(location);
        SetIsPhysical(isPhysical);
    }

    public void UpdateCashAccount(
        string displayName,
        int currencyId,
        DateOnly openingDate,
        decimal initialBalance,
        Guid modifiedBy,
        string location,
        bool isPhysical,
        string? description = null)
    {
        ChangeLocation(location);
        SetIsPhysical(isPhysical);

        UpdateMonetaryAccount(
            displayName,
            currencyId,
            openingDate,
            initialBalance,
            modifiedBy,
            0,
            description);
    }

    private void ChangeLocation(string newLocation)
    {
        if (string.IsNullOrWhiteSpace(newLocation))
            throw new DomainException(DomainErrors.CashAccount.LocationRequired);

        Location = newLocation.Trim();
    }

    private void SetIsPhysical(bool isPhysical)
    {
        IsPhysical = isPhysical;
    }
}
