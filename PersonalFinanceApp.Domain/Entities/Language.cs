using System.ComponentModel.DataAnnotations;
using PersonalFinanceApp.Domain.Errors;

namespace PersonalFinanceApp.Domain.Entities;

public class Language
{
    public byte Id { get; private set; }

    public string Code { get; private set; } = string.Empty!;

    public string Name { get; private set; } = string.Empty!;

    public bool IsActive { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsRightToLeft { get; private set; } = false;


    private Language() { }

    public Language(string code, string name, bool isActive, int displayOrder, bool isRightToLeft = false)
    {
        ChangeCode(code);
        ChangeName(name);
        IsActive = isActive;
        SetDisplayOrder ( displayOrder);
        SetRightToLeft(isRightToLeft);
    }

    public void UpdateLanguage(string code, string name, bool isActive, int displayOrder, bool isRightToLeft = false)
    {
        ChangeCode(code);
        ChangeName(name);
        IsActive = isActive;
        SetDisplayOrder(displayOrder);
        SetRightToLeft(isRightToLeft);
    }

    public void SetDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SetRightToLeft(bool isRightToLeft) => IsRightToLeft = isRightToLeft;

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(DomainErrors.Language.NameRequired);

        Name = name.Trim();
    }

    public void ChangeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(DomainErrors.Language.CodeRequired);

        Code = code.Trim().ToUpperInvariant();
    }
}
