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
    
    public partial class MemberWorkoutPlan
    {
        public int MemberWorkoutPlanid { get; set; }
        public string Workout { get; set; }
        public string NumberOfSets { get; set; }
        public string NumberOfMinutes { get; set; }
        public string NumberofDays { get; set; }
        public string ExcerciseOrder { get; set; }
        public Nullable<int> PlaneNameId { get; set; }
        public Nullable<int> MemberID { get; set; }
        public Nullable<int> Repeats { get; set; }
    
        public virtual MemberInfo MemberInfo { get; set; }
        public virtual WorkoutPlanName WorkoutPlanName { get; set; }
    }
}
