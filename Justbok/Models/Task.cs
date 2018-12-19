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
    
    public partial class Task
    {
        public Task()
        {
            this.DailyTaskLists = new HashSet<DailyTaskList>();
        }
    
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string TaskDescription { get; set; }
        public Nullable<int> Interval { get; set; }
        public string IntervalType { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> GymId { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual GymList GymList { get; set; }
        public virtual ICollection<DailyTaskList> DailyTaskLists { get; set; }
    }
}