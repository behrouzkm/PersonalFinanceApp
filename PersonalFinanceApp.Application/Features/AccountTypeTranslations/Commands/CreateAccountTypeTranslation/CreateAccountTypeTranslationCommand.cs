using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Domain.Entities;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.CreateAccountTypeTranslation;

public class CreateAccountTypeTranslationCommand : IRequest<int>
{
    public int AccountTypeId { get; set; }
    public int LanguageId { get; set; }
    public string Translation { get; set; } = string.Empty!;
    public string? Description { get; set; }
}



