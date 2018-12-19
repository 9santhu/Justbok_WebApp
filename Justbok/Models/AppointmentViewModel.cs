using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class AppointmentConfigViewModel
    {
        public AppointmentConfigViewModel()
        {
            this.ArrAppointmentStaffs = new List<string>();
            this.ArrAppointmentSlabs = new List<AppointmentSlabModel>();
        }
        public int AppointmentId { get; set; }
        public int TaxType { get; set; }
        public string Title { get; set; }
        public bool IsAllStaff { get; set; }
        public string AppointmentStaffs { get; set; }
        public string AppointmentSlabs { get; set; }
        public decimal TaxPercentage { get; set; }

        public List<string> ArrAppointmentStaffs { get; set; }
        public List<AppointmentSlabModel> ArrAppointmentSlabs { get; set; }

    }

    public class AppointmentSlabModel
    {
        public int Minutes { get; set; }
        public decimal Price { get; set; }
    }
}