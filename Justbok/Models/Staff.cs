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
    
    public partial class Staff
    {
        public Staff()
        {
            this.AppointmentSlots = new HashSet<AppointmentSlot>();
            this.AppointmentStaffs = new HashSet<AppointmentStaff>();
            this.BookAppointments = new HashSet<BookAppointment>();
            this.CalendarInstructors = new HashSet<CalendarInstructor>();
            this.ClassTimings = new HashSet<ClassTiming>();
            this.PurchaseOrderHeaders = new HashSet<PurchaseOrderHeader>();
            this.Schedulers = new HashSet<Scheduler>();
            this.StaffBranches = new HashSet<StaffBranch>();
            this.StaffLogins = new HashSet<StaffLogin>();
            this.StaffShifts = new HashSet<StaffShift>();
            this.TrainerDetails = new HashSet<TrainerDetail>();
        }
    
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<long> PhoneNumber { get; set; }
        public string StaffAddress { get; set; }
        public string StaffRole { get; set; }
        public Nullable<decimal> DailySalary { get; set; }
        public string Isactive { get; set; }
        public Nullable<int> GymId { get; set; }
    
        public virtual ICollection<AppointmentSlot> AppointmentSlots { get; set; }
        public virtual ICollection<AppointmentStaff> AppointmentStaffs { get; set; }
        public virtual ICollection<BookAppointment> BookAppointments { get; set; }
        public virtual ICollection<CalendarInstructor> CalendarInstructors { get; set; }
        public virtual ICollection<ClassTiming> ClassTimings { get; set; }
        public virtual GymList GymList { get; set; }
        public virtual ICollection<PurchaseOrderHeader> PurchaseOrderHeaders { get; set; }
        public virtual ICollection<Scheduler> Schedulers { get; set; }
        public virtual ICollection<StaffBranch> StaffBranches { get; set; }
        public virtual ICollection<StaffLogin> StaffLogins { get; set; }
        public virtual ICollection<StaffShift> StaffShifts { get; set; }
        public virtual ICollection<TrainerDetail> TrainerDetails { get; set; }
    }
}
