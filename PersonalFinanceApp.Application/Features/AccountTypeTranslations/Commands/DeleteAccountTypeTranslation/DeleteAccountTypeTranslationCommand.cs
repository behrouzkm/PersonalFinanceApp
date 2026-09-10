using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Commands.DeleteAccountTypeTranslation;

public class DeleteAccountTypeTranslationCommand : IRequest
{
    public int Id { get; set; }

}
