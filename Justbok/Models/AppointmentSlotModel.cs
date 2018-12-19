using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class AppointmentSlotModel
    {
        public string SlotId { get { return Guid.NewGuid().ToString(); } }
        public int AppointmentSlotId { get; set; }
        public int AppSlotID { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public string StaffName { get; set; }
        public int StaffId { get; set; }
        public int TotalDuration { get; set; }
        public int UsedDuration { get; set; }
        public int MinDuration { get; set; }
        public string Pattern { get; set; }
        public bool isBooked { get; set; }
        public int BranchId { get; set; }
    }
}