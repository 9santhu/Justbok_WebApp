using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class MemberFreezeModel
    {
        public int FreezeId { get; set; }
        public int MemberShipId { get; set; }
        public string MemershipType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}