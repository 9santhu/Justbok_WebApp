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
    
    public partial class DailyTaskList
    {
        public int DailyTaskId { get; set; }
        public Nullable<System.DateTime> TaskDate { get; set; }
        public string Title { get; set; }
        public string TitleDescription { get; set; }
        public string TaskStatus { get; set; }
        public Nullable<int> TaskId { get; set; }
    
        public virtual Task Task { get; set; }
    }
}
