using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Justbok.Models
{
    public class GymViewModel
    {
        public int Loginid { get; set; }
        public int Id { get; set; }
        public string GymId { get; set; }
        public string GymName { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string Openedat { get; set; }
        public string Closeat { get; set; }
        public string GymAddress { get; set; }
        public string Email { get; set; }

          [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> EnrolDate { get; set; }
        public Nullable<long> MobileNumber { get; set; }
        public Nullable<long> PhoneNumberGym { get; set; }
        public string Representative { get; set; }

        public string Services { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

       


        public int PackageId { get; set; }
        public string GymPackageId { get; set; }
        public string PackageType { get; set; }
        public Nullable<decimal> PackageAmount { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Note { get; set; }
        public string GymStatus { get; set; }
        public Nullable<int> Months { get; set; }

        public int RecieptNumber { get; set; }
        public string PaymentType { get; set; }
        public Nullable<decimal> PaymentAmount { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public Nullable<System.DateTime> PaymentDueDate { get; set; }
        public Nullable<long> ReferenceNumber { get; set; }

        public bool   IsLoginActive { get; set; }

        public string Branchcode { get; set; }
        public string BranchName { get; set; }
        public string BranchAdress { get; set; }
        public Nullable<long> PhoneNo { get; set; }
        public string City { get; set; }
        public string BranchState { get; set; }

        public string EstablishedYear { get; set; }
        public string InstructionMessage { get; set; }
        public string About { get; set; }
        public string FeedBack { get; set; }



    }
}