using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class NewGymPackages
    {
        public string PackageId { get; set; }
        public string PackageType { get; set; }
        public string PackageAmount { get; set; }
        public string Months { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Note { get; set; }
        public string GymStatus { get; set; }
        public string PaymentAmount { get; set; }

        
    }
}