using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Languages.Commands.DeleteLanguage;

public class DeleteLanguageCommand : IRequest
{
    public int Id { get; set; }

}
