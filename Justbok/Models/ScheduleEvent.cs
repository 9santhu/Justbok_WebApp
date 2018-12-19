using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class ScheduleEvent
    {
        public int SchedulerId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
    }

    [Serializable]
    public class SchedulerViewModel
    {
        public int MemberId { get; set; }
        public int StaffId { get; set; }
        public DateTime StartDate { get; set; }
        public double dStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public int SelectType { get; set; }
        public string SelectedDay { get; set; }
        public string Repeatable { get; set; }
        public string strStartDate { get; set; }
    }
}