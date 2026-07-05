// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Threading.Tasks;
// using PersonalFinanceApp.Domain.Common;

// namespace PersonalFinanceApp.Domain.Entities;

// // Represents a type of document (e.g., Opening Entry, Expense Entry, Income Entry, Transfer Entry, ...)
// public class DocumentType
// {
//     public byte Id { get; private set; }

//     public bool IsActive { get; private set; }

//     public int DisplayOrder { get; private set; }

//     private DocumentType() { }

//     public DocumentType(bool isActive, int displayOrder)
//     {
//         IsActive = isActive;
//         DisplayOrder = displayOrder;
//     }

//     public void Activate() => IsActive = true;

//     public void Deactivate() => IsActive = false;

//     public void SetDisplayOrder(int displayOrder) => DisplayOrder = displayOrder;
// }
