using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class NewTimeSheet
    {
        public DateTime AttendenceDate { get; set; }
        public string Present { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string Leave { get; set; }
        public int LeaveId { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public string LeaveDetails { get; set; }
        public Nullable<int> StaffId { get; set; }
        public Nullable<int> GymId { get; set; }
        public string FirstName { get; set; }

        public long TimeSheetId { get; set; }
        public string LastName { get; set; }
        public string BranchId { get; set; }
        public Nullable<int> MemberID { get; set; }

        public Nullable<long> MobileNumber { get; set; }
    }
}