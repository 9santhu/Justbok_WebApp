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
    
    public partial class GymLogin
    {
        public int Loginid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<int> GymId { get; set; }
        public string IsLoginActive { get; set; }
        public string Role { get; set; }
        public Nullable<int> StaffId { get; set; }
        public Nullable<int> MemberId { get; set; }
    
        public virtual GymList GymList { get; set; }
    }
}
