using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;
using Justbok.ADModel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Dynamic;
using System.Web.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using log4net;
using System.Web.Mvc;


namespace Justbok.BL
{
    public class JustbokReports
    {
        JustbokEntities db = new JustbokEntities();


        public static readonly string Status = "Not Active";
        public static readonly string MembershipType = "---Select---";
        public static readonly string ActiveStatus = "Active";

       

        //search by member id
        public List<MemberViewModel> MembershipReport(int memberid,int gymid,int branchid)
        {
            List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
                                                join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                join p in db.Payments on ms.MembershipId equals p.MembershipId
                                                where mi.MemberID == memberid && mi.GymId == gymid && mi.BranchId == branchid
                                                select new  MemberViewModel
                                                {
                                                    MemberID = mi.MemberID,
                                                    FirstName = mi.FirstName + " " + mi.LastName,
                                                    MemberAddress = mi.MemberAddress.ToString(),
                                                    MobileNumber = mi.MobileNumber,
                                                    Dob = mi.Dob,
                                                    Email = mi.Email,
                                                    EnrollDate = mi.EnrollDate,
                                                    MembershipType = ms.MembershipType,
                                                    StartDate = ms.StartDate,
                                                    EndDate = ms.EndDate,
                                                    Amount = ms.Amount,
                                                    PaymentAmount = p.PaymentAmount,
                                                    Status = ms.Status

                                                }).ToList();
            return membershipReport;
        
        }

        //search by member name
        public List<MemberViewModel> MembershipReport(string membername, int gymid, int branchid)
        {
            List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
                                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                                      where (mi.FirstName.ToLower().Trim()+" "+mi.LastName.ToLower().Trim()).Contains( membername.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid
                                                      select new MemberViewModel
                                                      {
                                                          MemberID = mi.MemberID,
                                                          FirstName = mi.FirstName + " " + mi.LastName,
                                                          MemberAddress = mi.MemberAddress.ToString(),
                                                          MobileNumber = mi.MobileNumber,
                                                          Dob = mi.Dob,
                                                          Email = mi.Email,
                                                          EnrollDate = mi.EnrollDate,
                                                          MembershipType = ms.MembershipType,
                                                          StartDate = ms.StartDate,
                                                          EndDate = ms.EndDate,
                                                          Amount = ms.Amount,
                                                          PaymentAmount = p.PaymentAmount,
                                                          Status = ms.Status

                                                      }).ToList();

            return membershipReport;

        }

        //search by start date and end date
        public List<MemberViewModel> MembershipReport(DateTime startdate,DateTime enddate, int gymid, int branchid)
        {
            List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
                                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                                      where ms.StartDate >= startdate && ms.EndDate<=enddate && mi.GymId == gymid && mi.BranchId == branchid
                                                      select new MemberViewModel
                                                      {
                                                          MemberID = mi.MemberID,
                                                          FirstName = mi.FirstName + " " + mi.LastName,
                                                          MemberAddress = mi.MemberAddress.ToString(),
                                                          MobileNumber = mi.MobileNumber,
                                                          Dob = mi.Dob,
                                                          Email = mi.Email,
                                                          EnrollDate = mi.EnrollDate,
                                                          MembershipType = ms.MembershipType,
                                                          StartDate = ms.StartDate,
                                                          EndDate = ms.EndDate,
                                                          Amount = ms.Amount,
                                                          PaymentAmount = p.PaymentAmount,
                                                          Status = ms.Status

                                                      }).ToList();

            return membershipReport;

        }

        //search by status
        public List<MemberViewModel> MembershipReport(int gymid, int branchid,string status)
        {
          
            List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
                                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                                      where ms.Status == status && mi.GymId == gymid && mi.BranchId == branchid
                                                      select new MemberViewModel
                                                      {
                                                          MemberID = mi.MemberID,
                                                          FirstName = mi.FirstName + " " + mi.LastName,
                                                          MemberAddress = mi.MemberAddress.ToString(),
                                                          MobileNumber = mi.MobileNumber,
                                                          Dob = mi.Dob,
                                                          Email = mi.Email,
                                                          EnrollDate = mi.EnrollDate,
                                                          MembershipType = ms.MembershipType,
                                                          StartDate = ms.StartDate,
                                                          EndDate = ms.EndDate,
                                                          Amount = ms.Amount,
                                                          PaymentAmount = p.PaymentAmount,
                                                          Status = ms.Status


                                                      }).ToList();

            return membershipReport;

        }

        //search by membership type
        public List<MemberViewModel> MembershipReport(int gymid, string membershiptype, int branchid)
        {
            List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
                                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                                      where ms.MembershipType.Trim() == membershiptype.Trim() && mi.GymId == gymid && mi.BranchId == branchid
                                                      select new MemberViewModel
                                                      {
                                                          MemberID = mi.MemberID,
                                                          FirstName = mi.FirstName + " " + mi.LastName,
                                                          MemberAddress = mi.MemberAddress.ToString(),
                                                          MobileNumber = mi.MobileNumber,
                                                          Dob = mi.Dob,
                                                          Email = mi.Email,
                                                          EnrollDate = mi.EnrollDate,
                                                          MembershipType = ms.MembershipType,
                                                          StartDate = ms.StartDate,
                                                          EndDate = ms.EndDate,
                                                          Amount = ms.Amount,
                                                          PaymentAmount = p.PaymentAmount,
                                                          Status = ms.Status

                                                      }).ToList();

            return membershipReport;

        }

        //search by all conditions by memberid
        //public List<MemberViewModel> MembershipReport(int memberid,string membershiptype,DateTime startdate,DateTime enddate,string status,int gymid, int branchid)
        //{

           
        //    List<MemberViewModel> membershipReport = (from mi in db.MemberInfoes
        //                                              join ms in db.MemberShips on mi.MemberID equals ms.MemberID
        //                                              join p in db.Payments on ms.MembershipId equals p.MembershipId
        //                                              where mi.MemberID==memberid && ms.MembershipType.Trim() == membershiptype.Trim() && ms.StartDate>=startdate &&   
        //                                                  ms.EndDate<=enddate && ms.Status.ToLower()==status.ToLower()
        //                                                  && mi.GymId == gymid && mi.BranchId == branchid
        //                                              select new MemberViewModel
        //                                              {
        //                                                  MemberID = mi.MemberID,
        //                                                  FirstName = mi.FirstName + " " + mi.LastName,
        //                                                  MemberAddress = mi.MemberAddress.ToString(),
        //                                                  MobileNumber = mi.MobileNumber,
        //                                                  Dob = mi.Dob,
        //                                                  Email = mi.Email,
        //                                                  EnrollDate = mi.EnrollDate,
        //                                                  MembershipType = ms.MembershipType,
        //                                                  StartDate = ms.StartDate,
        //                                                  EndDate = ms.EndDate,
        //                                                  Amount = ms.Amount,
        //                                                  PaymentAmount = p.PaymentAmount,
        //                                                  Status = ms.Status

        //                                              }).ToList();

        //    return membershipReport;

        //}

        //search by all conditions by member 
        public List<MemberViewModel> MembershipReport(string membername, string membershiptype, DateTime startdate, DateTime enddate, string status, int gymid, int branchid)
        {
            List<MemberViewModel> membershipReport = new List<MemberViewModel>();

            int memberId = 0;
            string memberName = string.Empty;

            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }



            if (memberName!="" && DateTime.MinValue != startdate && DateTime.MinValue != enddate && status !=  Status.ToString() && membershiptype != JustbokReports.MembershipType)
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) && ms.MembershipType.Trim() == membershiptype.Trim()  && ms.Status.ToLower() == status.ToLower() && ms.StartDate>=startdate && ms.EndDate<=enddate 
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //id -->ship
            else if (memberName != "" && !string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType)
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) && 
                                    ms.MembershipType.Trim() == membershiptype.Trim() 
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            
            }
                //id-->ship-->startdate->enddate
            else if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && startdate != DateTime.MinValue)
            {
               if(enddate==DateTime.MinValue)
               {
                  enddate=DateTime.Today.Date;
               }

                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) && 
                                    ms.MembershipType.Trim() == membershiptype.Trim() && ms.StartDate>=startdate && ms.EndDate<=enddate
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //id-->start--end
            else if (!string.IsNullOrEmpty(memberName) && startdate != DateTime.MinValue)
            {
                if(enddate==DateTime.MinValue)
               {
                  enddate=DateTime.Today.Date;
               }

                  membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) && 
                                   ms.StartDate>=startdate && ms.EndDate<=enddate
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }

                //id--start--end--active
            else if (!string.IsNullOrEmpty(memberName) && startdate != DateTime.MinValue && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                if(enddate==DateTime.MinValue)
               {
                  enddate=DateTime.Today.Date;
               }

                 membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) &&
                                   ms.StartDate >= startdate && ms.EndDate <= enddate && ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus)
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //id--active
            else if (!string.IsNullOrEmpty(memberName) && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
               membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus)
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //id--ship--active
            else if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername.ToLower().Trim()) &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus) && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //ship--start--end
             else if(!string.IsNullOrEmpty(membershiptype) && membershiptype!=JustbokReports.MembershipType &&  startdate!=DateTime.MinValue )
            {
               if(enddate==DateTime.MinValue)
               {
                  enddate=DateTime.Today.Date;
               }

               membershipReport = (from mi in db.MemberInfoes
                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                   join p in db.Payments on ms.MembershipId equals p.MembershipId
                                   where  ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && ms.StartDate>=startdate && ms.EndDate<=enddate && mi.GymId == gymid && mi.BranchId == branchid
                                   select new MemberViewModel
                                   {
                                       MemberID = mi.MemberID,
                                       FirstName = mi.FirstName + " " + mi.LastName,
                                       MemberAddress = mi.MemberAddress.ToString(),
                                       MobileNumber = mi.MobileNumber,
                                       Dob = mi.Dob,
                                       Email = mi.Email,
                                       EnrollDate = mi.EnrollDate,
                                       MembershipType = ms.MembershipType,
                                       StartDate = ms.StartDate,
                                       EndDate = ms.EndDate,
                                       Amount = ms.Amount,
                                       PaymentAmount = p.PaymentAmount,
                                       Status = ms.Status

                                   }).ToList();

             

            }
               //ship--start--end--active
            else if (!string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && startdate != DateTime.MinValue && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                if(enddate==DateTime.MinValue)
               {
                  enddate=DateTime.Today.Date;
               }
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && ms.StartDate >= startdate && ms.EndDate <= enddate &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus) &&
                                    mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();


            }
                //ship--active
            else if (!string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim())  &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus) &&
                                    mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
            //start-end-active
            else if (startdate != DateTime.MinValue && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                if (enddate == DateTime.MinValue)
                {
                    enddate = DateTime.Today.Date;
                }

                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where  ms.StartDate >= startdate && ms.EndDate <= enddate &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus) &&
                                    mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
                //memberid --ship
            else if (memberId != 0 && membershiptype != "" && membershiptype != JustbokReports.MembershipType)
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID==memberId &&
                                    ms.MembershipType.Trim() == membershiptype.Trim()
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();

            }
            //id-->ship-->startdate->enddate
            else if (memberId != 0 && !string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && startdate != DateTime.MinValue)
            {
                if (enddate == DateTime.MinValue)
                {
                    enddate = DateTime.Today.Date;
                }

                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID == memberId &&
                                    ms.MembershipType.Trim() == membershiptype.Trim() && ms.StartDate >= startdate && ms.EndDate <= enddate
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
            //id-->start--end
            else if (memberId != 0 && startdate != DateTime.MinValue)
            {
                if (enddate == DateTime.MinValue)
                {
                    enddate = DateTime.Today.Date;
                }

                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID == memberId &&
                                   ms.StartDate >= startdate && ms.EndDate <= enddate
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }

               //id--start--end--active
            else if (memberId != 0 && startdate != DateTime.MinValue && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                if (enddate == DateTime.MinValue)
                {
                    enddate = DateTime.Today.Date;
                }

                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID == memberId &&
                                   ms.StartDate >= startdate && ms.EndDate <= enddate && ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus)
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
            //id--active
            else if (memberId != 0 && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID == memberId &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus)
                                        && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }
            //id--ship--active
            else if (memberId != 0 && !string.IsNullOrEmpty(membershiptype) && membershiptype != JustbokReports.MembershipType && status != JustbokReports.Status && !string.IsNullOrEmpty(status))
            {
                membershipReport = (from mi in db.MemberInfoes
                                    join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                    join p in db.Payments on ms.MembershipId equals p.MembershipId
                                    where mi.MemberID == memberId &&
                                    ms.Status.ToLower().Trim().Equals(JustbokReports.ActiveStatus) && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid
                                    select new MemberViewModel
                                    {
                                        MemberID = mi.MemberID,
                                        FirstName = mi.FirstName + " " + mi.LastName,
                                        MemberAddress = mi.MemberAddress.ToString(),
                                        MobileNumber = mi.MobileNumber,
                                        Dob = mi.Dob,
                                        Email = mi.Email,
                                        EnrollDate = mi.EnrollDate,
                                        MembershipType = ms.MembershipType,
                                        StartDate = ms.StartDate,
                                        EndDate = ms.EndDate,
                                        Amount = ms.Amount,
                                        PaymentAmount = p.PaymentAmount,
                                        Status = ms.Status

                                    }).ToList();
            }

           

            return membershipReport;

        }







    }
}