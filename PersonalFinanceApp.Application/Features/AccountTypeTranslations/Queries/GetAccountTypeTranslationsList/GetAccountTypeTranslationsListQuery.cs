using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.AccountTypeTranslations.Common;

namespace PersonalFinanceApp.Application.Features.AccountTypeTranslations.Queries.GetAccountTypeTranslationsList;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetAccountTypeTranslationsListQuery : IRequest<PaginatedList<AccountTypeTranslationDto>>
{
    public int? AccountTypeId { get; set; }
    public int? LanguageId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
