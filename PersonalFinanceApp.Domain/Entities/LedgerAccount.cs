using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;
using PersonalFinanceApp.Domain.Interfaces;

namespace PersonalFinanceApp.Domain.Entities;

public class LedgerAccount : BaseAuditableEntity ,IReorderable
{
    public int AccountTypeId { get; private set; }
    public AccountType AccountType { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty!;

    public int? CurrencyId { get; private set; }
    public Currency? Currency { get; private set; }
    
    // Is this account allowed for using in AccountEntry?
    public bool IsPostingAccount { get; private set; }

    public bool HasBeenUsedInEntries {get; private set;}

    // Link to the parent
    public Guid? ParentId { get; private set; }
    public LedgerAccount? Parent { get; private set; }


    // [Timestamp]
    // public byte[] RowVersion { get; set; } = default!;



    private readonly List<LedgerAccount> _children = new();
    public IReadOnlyCollection<LedgerAccount> Children  => _children.AsReadOnly();

    // Controls the display order of this account among its siblings in the Chart of Accounts tree.
    // This is independent of Person.DisplayOrder and MonetaryAccount.DisplayOrder,
    // which control the display order of their respective entities in list views.
    public int DisplayOrder { get; private set; }

    private LedgerAccount() { }

    public LedgerAccount(int accountTypeId, string name, Guid tenantId, Guid createdBy, string? description = null)
                            : base(tenantId, createdBy,description)
    {
        AccountTypeId = accountTypeId;

        SetName(name);

        IsPostingAccount=true;
    }

    public void UpdateLedgerAccount(string name, Guid modifiedBy, string? description)
    {
        SetName(name);
        SetDescription(description);

        UpdateAudit(modifiedBy);
    }

    public void AddChild(LedgerAccount child)
    {
        if(HasBeenUsedInEntries)
            throw new DomainException(DomainErrors.LedgerAccount.CannotModifyUsedAccount);

        IsPostingAccount = false;
        _children.Add(child);
    }

    public void MarkAsUsed()
    {
        HasBeenUsedInEntries=true;
    }

    public void SetAsPostingAccount()
    {
        if (Children.Any())
            throw new DomainException(DomainErrors.LedgerAccount.CannotSetParentAsPosting);

        IsPostingAccount = true;
    }

    // public void ChangeAccountType(int accountTypeId)
    // {
    //     if (HasBeenUsedInEntries && AccountTypeId != accountTypeId)
    //         throw new DomainException(DomainErrors.LedgerAccount.CannotModifyUsedAccount);


    // }

    private void SetName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException(DomainErrors.LedgerAccount.NameRequired);

        Name = newName.Trim();
    }

    public void SetDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new DomainException(DomainErrors.MonetaryAccount.DisplayOrderCannotBeNegative);

        DisplayOrder = displayOrder;
    }

}
