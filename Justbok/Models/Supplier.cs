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
    
    public partial class Supplier
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<long> RegistrationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ForSale { get; set; }
        public Nullable<long> PhoneNumber { get; set; }
        public string SupplierAddress { get; set; }
        public string FaxNumber { get; set; }
        public Nullable<int> GymId { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual GymList GymList { get; set; }
    }
}
