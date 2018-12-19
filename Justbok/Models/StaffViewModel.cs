using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class StaffViewModel
    {

        //Staff
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<long> PhoneNumber { get; set; }
        public string StaffAddress { get; set; }
        public string StaffRole { get; set; }
        public Nullable<decimal> DailySalary { get; set; }
        public bool Isactive { get; set; }
        public int ShiftId { get; set; }
        //Staff Bracnch 

        public string MultiselectBranch { get; set; }

        public string MultiselectShiftType { get; set; }
        public int StaffBranchId { get; set; }
       
        public Nullable<int> BranchId { get; set; }

        public string Role { get; set; }
        public int Loginid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsLoginactive { get; set; }

        public string Branchcode { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public Nullable<long> PhoneNo { get; set; }
        public string City { get; set; }
        public string BranchState { get; set; }

        public string ShiftName { get; set; }

        public int Trainerid { get; set; }
        public string ImageData { get; set; }
        public Nullable<int> Experience { get; set; }
        public string Qulifiaction { get; set; }
        public string TrainerDescription { get; set; }


    }
}