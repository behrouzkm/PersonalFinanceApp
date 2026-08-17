using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Domain.Services;

// A minimal, read-only view of one point on a fund source's chronological ledger -
// deliberately not the full AccountingEntry entity, so this stays usable both for
// real, already-persisted entries and for a not-yet-saved proposed entry alike.
public interface IledgerEntryPoint
{
    Guid EntryId { get; }
    DateOnly DocumentDate { get; }
    DateTime CreatedAt { get; }
    decimal Debit { get; }
    decimal Credit { get; }
}
