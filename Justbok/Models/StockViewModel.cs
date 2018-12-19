using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Justbok.Models
{
    public class StockViewModel
    {
        public int ProductId { get; set; }
        public Nullable<int> Quantity { get; set; }

        public int StockId { get; set; }
        public string Manufacture { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> CurrentStock { get; set; }
        public Nullable<int> StockIn { get; set; }
        public Nullable<int> StockOut { get; set; }
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> StockinDate { get; set; }
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> StockoutDate { get; set; }
    }
}