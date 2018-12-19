using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Justbok.Models
{
     //[Bind(Exclude = "MemberID")]
    public class MemberViewModel
    {

        //[ScaffoldColumn(false)]
        public int MemberID { get; set; }

     [Required(ErrorMessage = "Field Required")]
     [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string FirstName { get; set; }

         [Required(ErrorMessage = "Field Required")]
         [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string LastName { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
         public Nullable<System.DateTime> Dob { get; set; }
      
        public string Gender { get; set; }
         //[Required(ErrorMessage = "Field Required")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }
          [Required(ErrorMessage = "Field Required")]
          [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public Nullable<long> MobileNumber { get; set; }
        public string MemberAddress { get; set; }

         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> EnrollDate { get; set; }
        public bool Married { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string SpouseName { get; set; }
        public Nullable<System.DateTime> SpouseBirthDate { get; set; }
        public Nullable<System.DateTime> AnniversaryDate { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Occupation { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Designation { get; set; }
         
        public string MemberSource { get; set; }
         [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must be numeric")]
        public Nullable<long> PhoneResidence { get; set; }
         [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must be numeric")]
        public Nullable<long> PhoneOffice { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string ReferredBy { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Programme { get; set; }

        public string MemberReference { get; set; }

        public string Representative { get; set; }

        public Nullable<int> GymId { get; set; }

        //Membership

        public int MembershipId { get; set; }

         [Required(ErrorMessage = "Field Required")]
        public string MembershipType { get; set; }
         [Required(ErrorMessage = "Field Required")]
        public Nullable<int> Months { get; set; }
         [Required(ErrorMessage = "Field Required")]
         [RegularExpression("^[0-9]*$", ErrorMessage = "Use numbers only")]
        public Nullable<decimal> Amount { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Note { get; set; }
        
        public string Status { get; set; }

        public string Other { get; set; }

        public Nullable<int> BranchId { get; set; }
    

        //Payment
        public int RecieptNumber { get; set; }
        public string PaymentType { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> PaymentDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: dd/MM/yyyy}")]
        public Nullable<System.DateTime> PaymentDueDate { get; set; }
        public Nullable<long> ReferenceNumber { get; set; }
        

        //Transfer

        public int TransferId { get; set; }
        public Nullable<int> TransferFrom { get; set; }
        public Nullable<System.DateTime> TransferDate { get; set; }
        public Nullable<int> TransferTo { get; set; }

        //Measurement

        public int MeasurementId { get; set; }
        public Nullable<System.DateTime> MeasurementDate { get; set; }
        public Nullable<System.DateTime> NextMeasurementDate { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> weight { get; set; }
        public Nullable<decimal> UpperArm { get; set; }
        public Nullable<decimal> ForeArm { get; set; }
        public Nullable<decimal> Calves { get; set; }
        public Nullable<decimal> BMI { get; set; }
        public Nullable<decimal> VFat { get; set; }
        public Nullable<decimal> Shoulder { get; set; }
        public Nullable<decimal> Chest { get; set; }
        public Nullable<decimal> Arms { get; set; }
        public Nullable<decimal> UpperABS { get; set; }
        public Nullable<decimal> WaistABS { get; set; }
        public Nullable<decimal> LowerABS { get; set; }
        public Nullable<decimal> Glutes { get; set; }
        public Nullable<decimal> Thighs { get; set; }

        public string imagepath { get; set; }


        public string Workout { get; set; }
        public string NumberOfSets { get; set; }
        public string NumberOfMinutes { get; set; }
        public string NumberofDays { get; set; }
        public string ExcerciseOrder { get; set; }
        public Nullable<int> PlaneNameId { get; set; }
        public Nullable<int> Repeats { get; set; }
        public int Planid { get; set; }
        public string PlanName { get; set; }
       
    }
}