using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class NewMemberShip
    {
      
        public string MembershipID { get; set; }
        public string MemershipType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Months { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
        public string PaidAmount { get; set; }
        public string Notes { get; set; }
        public string RemainingDays { get; set; }
    }

    public class GetMemberList
    {
        List<NewMemberShip> lstMemberships = new List<NewMemberShip>();
    }
}