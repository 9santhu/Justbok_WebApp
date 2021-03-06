//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Justbok.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Expens
    {
        public int ExpensesId { get; set; }
        public Nullable<int> ExpenseTypeId { get; set; }
        public Nullable<decimal> ExpenseAmount { get; set; }
        public Nullable<System.DateTime> ExpenseDate { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> GymId { get; set; }
        public string ReferenceNumber { get; set; }
        public string ExpenseMode { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ExpensesType ExpensesType { get; set; }
        public virtual GymList GymList { get; set; }
    }
}
