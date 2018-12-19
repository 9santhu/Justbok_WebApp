using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class EventModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public bool IncludedTax { get; set; }
        public bool IsAllDay { get; set; }
        public decimal Price { get; set; }
        public int RegistrationLimit { get; set; }
        public string PhotoUrl { get; set; }
    }
}