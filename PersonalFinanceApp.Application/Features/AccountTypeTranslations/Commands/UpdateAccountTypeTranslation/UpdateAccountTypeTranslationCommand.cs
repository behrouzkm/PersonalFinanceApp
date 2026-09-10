using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.UpdateAccountTypeTranslation;

public class UpdateAccountTypeTranslationCommand : IRequest
{
    public int Id { get; set; }
    public int AccountTypeId { get;  set; }
    public int LanguageId { get;  set; }
    public string Translation { get;  set; } = string.Empty!;
    public string? Description { get;  set; }
}
