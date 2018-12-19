using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class PurchaseOrderViewModel
    {
        public PurchaseOrderViewModel()
        {
            this.PurchaseOrderDetails = new List<PurchaseOrderDetailViewModel>();
        }

        public int PurchaseId { get; set; }
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public Nullable<DateTime> OrderDate { get; set; }
        public string PaymentVia { get; set; }
        public string Representative { get; set; }
        public string GymId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int StaffId { get; set; }
        public int BranchId { get; set; }

        public List<PurchaseOrderDetailViewModel> PurchaseOrderDetails { get; set; }
    }

    public class PurchaseOrderDetailViewModel
    {
        public int DetailId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public int DiscountType { get; set; }
        public decimal Discount { get; set; }
        public int GST { get; set; }
        public decimal NetAmount { get; set; }
        public decimal GstAmount { get; set; }
        public string ProductName { get; set; }

    }
}