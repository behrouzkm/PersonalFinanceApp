using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Common.Models;
using PersonalFinanceApp.Application.Features.Persons.Common;


namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonsOptions;

public class GetPersonsOptionsQueryHandler : IRequestHandler<GetPersonsOptionsQuery, List<PersonOptionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonsOptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PersonOptionDto>> Handle(GetPersonsOptionsQuery request,
                        CancellationToken cancellationToken)
    {
        var options = await _context.Persons
            .AsNoTracking()
            .OrderBy(o => o.DisplayOrder)
            .Select(r => new PersonOptionDto
            {
                Id = r.Id,
                PersonType = r.PersonType,
                DisplayName = r.DisplayName
            })
            .ToListAsync(cancellationToken);

        return options;
    }
}
