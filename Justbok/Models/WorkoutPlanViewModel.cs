using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class WorkoutPlanViewModel
    {
        public string Workout { get; set; }
        public string NumberOfSets { get; set; }
        public string NumberOfMinutes { get; set; }
        public string NumberofDays { get; set; }
        public string ExcerciseOrder { get; set; }
        public Nullable<int> PlaneNameId { get; set; }
        public Nullable<int> Repeats { get; set; }
        public int Planid { get; set; }
        public string PlanName { get; set; }
        public int MemberWorkoutPlanid { get; set; }
        public string SetMin { get; set; }
        public Nullable<int> BranchId { get; set; }
    }
}