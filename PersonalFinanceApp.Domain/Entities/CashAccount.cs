using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class CashAccount : MonetaryAccount
{
    public string Location { get; private set; } = string.Empty;
    public bool IsPhysical { get; private set; } = true;

    private CashAccount() { }


    public CashAccount(string name, Guid ledgerAccountId, byte currencyId, decimal initialBalance, int displayOrder,
                            string location, Guid tenantId, Guid createdBy, bool isPhysical = true) :
                                base(name, ledgerAccountId, currencyId, initialBalance, displayOrder, tenantId, createdBy)
    {
        ChangeLocation(location);
        SetIsPhysical(isPhysical);
    }


    public void ChangeLocation(string newLocation)
    {
        if (string.IsNullOrWhiteSpace(newLocation))
            throw new DomainException(DomainErrors.CashAccount.LocationRequired);

        Location = newLocation.Trim();
    }

    public void SetIsPhysical(bool isPhysical)
    {
        IsPhysical = isPhysical;
    }
}
