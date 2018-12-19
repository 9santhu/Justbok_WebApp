using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class SlotModel
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string ConfigIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsRepeat { get; set; }
        public int MinDuration { get; set; }
        public int TotalDuration { get; set; }
        public string RepeatsOn { get; set; }
        public string strStartDate { get; set; }
        public int SlotType { get; set; }
    }
}