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
    
    public partial class MemberImage
    {
        public int ImageId { get; set; }
        public string ImageData { get; set; }
        public Nullable<int> MemberId { get; set; }
    
        public virtual MemberInfo MemberInfo { get; set; }
    }
}