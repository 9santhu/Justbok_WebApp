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
    
    public partial class Appointment
    {
        public Appointment()
        {
            this.AppointmentSlotConfigs = new HashSet<AppointmentSlotConfig>();
            this.AppointmentStaffs = new HashSet<AppointmentStaff>();
            this.AppointmentSlabs = new HashSet<AppointmentSlab>();
            this.BookAppointments = new HashSet<BookAppointment>();
        }
    
        public int AppointmentId { get; set; }
        public string Title { get; set; }
        public Nullable<bool> IsAllStaff { get; set; }
        public Nullable<int> TaxType { get; set; }
        public Nullable<int> GymId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<int> BranchId { get; set; }
    
        public virtual ICollection<AppointmentSlotConfig> AppointmentSlotConfigs { get; set; }
        public virtual ICollection<AppointmentStaff> AppointmentStaffs { get; set; }
        public virtual ICollection<AppointmentSlab> AppointmentSlabs { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual GymList GymList { get; set; }
        public virtual TaxType TaxType1 { get; set; }
        public virtual ICollection<BookAppointment> BookAppointments { get; set; }
    }
}