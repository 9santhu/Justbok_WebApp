using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class ClassModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public int AttendenceLimit { get; set; }
        public int ReservationLimit { get; set; }
    }
}