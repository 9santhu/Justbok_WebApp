using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Justbok.Models
{
    public class DietViewModel
    {
        public int DietId { get; set; }
        public string DietTime { get; set; }
        public string MondayDiet { get; set; }
        public string TuesdayDiet { get; set; }
        public string WednesdayDiet { get; set; }
        public string ThursdayDiet { get; set; }
        public string FridayDiet { get; set; }
        public string SaturdayDiet { get; set; }
        public string SundayDiet { get; set; }
        public Nullable<int> DietPlanId { get; set; }

        public string DietPlanName1 { get; set; }

        public int MealTimeId { get; set; }
        public string MealTime1 { get; set; }
        public string MealDescription { get; set; }
        public int MemberDietPlanid { get; set; }

        public Nullable<int> BranchId { get; set; }
    }
}