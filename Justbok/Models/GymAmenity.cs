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
    
    public partial class GymAmenity
    {
        public int GymAmenitiesId { get; set; }
        public string GymAmenitiesName { get; set; }
        public Nullable<bool> AminityEnable { get; set; }
        public Nullable<int> GymId { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual GymList GymList { get; set; }
    }
}
