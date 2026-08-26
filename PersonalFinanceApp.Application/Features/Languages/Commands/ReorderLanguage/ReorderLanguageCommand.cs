using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace PersonalFinanceApp.Application.Features.Languages.Commands.ReorderLanguage;

public class ReorderLanguageCommand : IRequest
{
    public int Id { get; set; }
    public int NewDisplayOrder { get; set; }
}
