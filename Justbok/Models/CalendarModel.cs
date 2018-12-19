using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class CalendarModel
    {
        public string Id { get { return Guid.NewGuid().ToString(); } }
        public int ActualId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public bool isAllDay { get; set; }
        public bool isClass { get; set; }
        public int ReservationLimint { get; set; }
        public int AttendenceLimit { get; set; }
        public int StaffId { get; set; }
    }
}