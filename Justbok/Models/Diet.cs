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
    
    public partial class Diet
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
    
        public virtual DietPlanName DietPlanName { get; set; }
    }
}
