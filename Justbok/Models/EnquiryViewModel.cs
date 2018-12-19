using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Justbok.Models
{
    public class EnquiryViewModel
    {

        public Nullable<int> EnquiryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<long> MobileNumber { get; set; }
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> DOB { get; set; }
         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> EnquiryDate { get; set; }
        public string EmailId { get; set; }
        public Nullable<long> PhoneNumberResidence { get; set; }
        public Nullable<long> PhoneNumberOffice { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public Nullable<int> Age { get; set; }
        public string Intention { get; set; }
        public string ProgramSuggested { get; set; }
        public string Category { get; set; }
        public Nullable<decimal> AmountOffered { get; set; }
        public string TrailOffered { get; set; }
        public Nullable<System.DateTime> TrailDate { get; set; }
        public string HowDidYouKnow { get; set; }
        public string RecievedBy { get; set; }
        public string Notes { get; set; }
        public bool SMS { get; set; }
        public bool call { get; set; }
        public bool Email { get; set; }
        public Nullable<int> BranchId { get; set; }
        public List<SelectListItem> Representatives { get; set; }

        public int FollowupId { get; set; }
          [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> LastFollowUpDate { get; set; }
          [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> NextFollowUpDate { get; set; }
        public string EnqStatus { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
        public Nullable<int> GymId { get; set; }
    }
}