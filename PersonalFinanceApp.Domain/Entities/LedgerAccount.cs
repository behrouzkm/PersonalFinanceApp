using PersonalFinanceApp.Domain.Common;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class LedgerAccount : BaseAuditableEntity
{
    public byte AccountTypeId { get; private set; }
    public AccountType AccountType { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty!;

    // Is this account allowed for using in AccountEntry?
    public bool IsPostingAccount { get; private set; }

    public bool HasBeenUsedInEntries {get; private set;}

    // Link to the parent
    public Guid? ParentId { get; private set; }
    public LedgerAccount? Parent { get; private set; }


    private readonly List<LedgerAccount> _children = new();
    public IReadOnlyCollection<LedgerAccount> Children  => _children.AsReadOnly();


    private LedgerAccount() { }

    public LedgerAccount(byte accountTypeId, string name, Guid tenantId, Guid createdBy, string? description = null)
                            : base(tenantId, createdBy,description)
    {
        ChangeAccountType(accountTypeId);
        ChangeName(name);

        IsPostingAccount=true;
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

    public void ChangeAccountType(byte accountTypeId)
    {
        if (HasBeenUsedInEntries && AccountTypeId != accountTypeId)
            throw new DomainException(DomainErrors.LedgerAccount.CannotModifyUsedAccount);

        AccountTypeId = accountTypeId;
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException(DomainErrors.LedgerAccount.NameRequired);

        Name = newName.Trim();
    }

}
