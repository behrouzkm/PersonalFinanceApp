using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Common;
using PersonalFinanceApp.Application.Features.Persons.Common;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsList;

// One flexible query with optional filters, rather than a separate query per filter
// axis - covers listing, date-range reporting, and account/person-based views at once.
public class GetPersonsListQuery : IRequest<PaginatedList<PersonListItemDto>>
{
    public string? SearchText { get; set; }
    public PersonType? PersonType { get; set; }
    public int? CurrencyId { get; set; }
    public string? PhoneNumber { get; set; }

    public decimal? FromBalance { get; set; }
    public decimal? ToBalance { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
