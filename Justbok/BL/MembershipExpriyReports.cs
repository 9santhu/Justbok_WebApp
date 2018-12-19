using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;
using Justbok.ADModel;

namespace Justbok.BL
{
    public class MembershipExpriyReports
    {
        JustbokEntities db = new JustbokEntities();
        public static readonly string Status = "Not Active";
        public static readonly string MembershipType = "---Select---";
        public static readonly string ActiveStatus = "Active";

        DateTime todayDate = DateTime.Today.Date;

        public List<MemberViewModel> ExpiredMembershipReports(int memberid,int gymid,int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                              join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where mi.MemberID == memberid && mi.GymId == gymid && mi.BranchId == branchid && ms.EndDate <= todayDate

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
                                                          Months=ms.Months

                                                      }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(string membername, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername) && ms.EndDate <= todayDate && mi.GymId == gymid && mi.BranchId == branchid 

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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(DateTime startdate,DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where ms.StartDate>=startdate && ms.EndDate <=enddate && mi.GymId == gymid && mi.BranchId == branchid

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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(int gymid, string membershiptype, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid && ms.EndDate <= todayDate

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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(string membername,DateTime startdate,DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername) && ms.StartDate>=startdate && ms.EndDate <=enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }
        public List<MemberViewModel> ExpiredMembershipReports(int memberid, DateTime startdate, DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where mi.MemberID==memberid && ms.StartDate >= startdate && ms.EndDate <= enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(int memberid, string membershiptype, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where mi.MemberID == memberid && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(string membername, string membershiptype, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername) && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(DateTime startdate, DateTime enddate, string membershiptype, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) &&  ms.StartDate >= startdate && ms.EndDate <= enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(string membershiptype,int gymid, int branchid,DateTime startdate, DateTime enddate )
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && ms.StartDate >= startdate && ms.EndDate <= enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(int memberid,string membershiptype, DateTime startdate, DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where mi.MemberID==memberid && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) &&  ms.StartDate >= startdate && ms.EndDate <= enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }

        public List<MemberViewModel> ExpiredMembershipReports(string membername, string membershiptype, DateTime startdate, DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> expiredReport = (from mi in db.MemberInfoes
                                                   join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                                   where (mi.FirstName.ToLower().Trim() + " " + mi.LastName.ToLower().Trim()).Contains(membername)  && ms.MembershipType.ToLower().Trim().Equals(membershiptype.ToLower().Trim()) && ms.StartDate >= startdate && ms.EndDate <= enddate && mi.GymId == gymid && mi.BranchId == branchid
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
                                                       Months = ms.Months

                                                   }).ToList();

            return expiredReport;
        }


    }
}