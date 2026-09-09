using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Application.Common.Exceptions;
using PersonalFinanceApp.Application.Common.Interfaces;
using PersonalFinanceApp.Application.Features.Persons.Common;
using PersonalFinanceApp.Domain.Entities;
using PersonalFinanceApp.Domain.Enums;

namespace PersonalFinanceApp.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttachmentService _attachmentService;

    public GetPersonByIdQueryHandler(IApplicationDbContext context, IAttachmentService attachmentService)
    {
        _context = context;
        _attachmentService = attachmentService;
    }

    public async Task<PersonDto> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _context.Persons
            .Include(p => p.Currency)
                .FirstOrDefaultAsync(d => d.Id == request.PersonId, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.PersonId);

        var attachments = await _attachmentService.GetForOwnerAsync(
            AttachmentOwnerType.Person, person.Id, cancellationToken);

        return new PersonDto
        {
            Id = person.Id,
            PersonType = person.PersonType,
            DisplayName = person.DisplayName,
            LedgerAccountId = person.LedgerAccountId,
            OpeningDate = person.OpeningDate,
            InitialBalance = person.InitialBalance,
            CurrentBalance = person.CurrentBalance,
            CreditLimit = person.CreditLimit,
            OpeningAccountingDocumentId = person.OpeningAccountingDocumentId,
            CurrencyId = person.CurrencyId,
            CurrencyName = person.Currency.Name,
            CurrencySymbol = person.Currency.Symbol,
            DisplayOrder = person.DisplayOrder,
            Email = person.Email,
            MobileNumber = person.MobileNumber,
            TelNumber = person.TelNumber,
            Attachments = attachments
        };

    }
}
