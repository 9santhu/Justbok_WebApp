//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Justbok.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimeSheet
    {
        public long TimeSheetId { get; set; }
        public Nullable<System.DateTime> AttendenceDate { get; set; }
        public Nullable<long> Present { get; set; }
        public Nullable<int> Absent { get; set; }
        public Nullable<int> GymId { get; set; }
        public Nullable<int> StaffId { get; set; }
        public Nullable<int> BranchId { get; set; }
    }
}
