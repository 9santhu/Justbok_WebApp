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
    
    public partial class PurchaseOrderHeader
    {
        public PurchaseOrderHeader()
        {
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }
    
        public int PurchaseId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string PaymentVia { get; set; }
        public string Representative { get; set; }
        public string GymId { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> StaffId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
