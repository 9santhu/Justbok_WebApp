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
    public class MembershipPaymentReport
    {
        JustbokEntities db = new JustbokEntities();
        public static readonly string MembershipType = "---Select---";
        public static readonly string PaymentType = "---Select---";

        //memberid
        public List<MemberViewModel> MembershipReports(int memberid, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                                   where m.GymId == gymid && m.BranchId == branchid && m.MemberID == memberid

                                                   select new MemberViewModel
                                                   {
                                                       FirstName = m.FirstName + " " + m.LastName,
                                                       MobileNumber = m.MobileNumber,
                                                       MembershipType = s.MembershipType,
                                                       Months = s.Months,
                                                       RecieptNumber = p.RecieptNumber,
                                                       Amount = s.Amount,
                                                       PaymentDate = p.PaymentDate,
                                                       PaymentAmount = p.PaymentAmount,
                                                       PaymentType = p.PaymentType,
                                                       PaymentDueDate = p.PaymentDueDate,
                                                       Note = s.Note.ToString()

                                                   }).ToList();

            return membershipReport;
        }


        //membername
        public List<MemberViewModel> MembershipReports(string membername, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid && (m.FirstName.ToLower()+" "+m.LastName.ToLower()).Contains(membername)

                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //date

        public List<MemberViewModel> MembershipReports(DateTime startdate,DateTime endate, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid && s.StartDate>=startdate && s.EndDate<=endate

                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //membership
        public List<MemberViewModel> MembershipReports(int gymid, int branchid,string membership)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim()) 

                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //pmode
        public List<MemberViewModel> MembershipReports(int gymid, string paymentmode, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid && p.PaymentType.ToLower().Equals(paymentmode)

                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--mship
        public List<MemberViewModel> MembershipReports(int memberid,string membership,int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid && m.MemberID == memberid && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim())

                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--mship
        public List<MemberViewModel> MembershipReports(string membername, string membership, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                      (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim())
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--date
        public List<MemberViewModel> MembershipReports(string membername, DateTime startdate,DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                      (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && s.StartDate>=startdate && s.EndDate<=enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--date
        public List<MemberViewModel> MembershipReports(int memberid, DateTime startdate, DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                      m.MemberID==memberid && s.StartDate >= startdate && s.EndDate <= enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--pmode
        public List<MemberViewModel> MembershipReports(string membername, int gymid, string paymentmode, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                      (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--pmode
        public List<MemberViewModel> MembershipReports(int memberid, int gymid, string paymentmode, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                     m.MemberID == memberid && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }
        //mmebership--pmode
        public List<MemberViewModel> MembershipReports(int gymid, int branchid, string membership, string paymentmode)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                     s.MembershipType.ToLower().Trim().Equals(membership.Trim().ToLower()) && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mmebership--date
        public List<MemberViewModel> MembershipReports(int gymid, int branchid, string membership, DateTime startdate,DateTime enddate)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                     s.MembershipType.ToLower().Trim().Equals(membership.Trim().ToLower()) && s.StartDate>=startdate && s.EndDate<=enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        public List<MemberViewModel> MembershipReports(int gymid, int branchid, DateTime startdate, DateTime enddate,string payment)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    p.PaymentType.ToLower().Trim().Equals(payment.ToLower().Trim()) && s.StartDate >= startdate && s.EndDate <= enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }



        //mid--mship-date
        public List<MemberViewModel> MembershipReports(int memberid, string membership,DateTime startdate,DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                     m.MemberID == memberid && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim()) && s.StartDate>=startdate &&s.EndDate<=enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--mship-date
        public List<MemberViewModel> MembershipReports(string membername, string membership, DateTime startdate, DateTime enddate, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim()) && s.StartDate >= startdate && s.EndDate <= enddate
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        } 
        
        
        //mname--mship-pmode
        public List<MemberViewModel> MembershipReports(string membername, string membership, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim()) && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--mship-pmode
        public List<MemberViewModel> MembershipReports(int memberid, string membership, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    m.MemberID==memberid && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim()) && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--date-pmode
        public List<MemberViewModel> MembershipReports(int memberid, DateTime startdate,DateTime enddate, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    m.MemberID == memberid && s.StartDate>=startdate && s.EndDate<=enddate && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--date-pmode
        public List<MemberViewModel> MembershipReports(string membername, DateTime startdate, DateTime enddate, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                    (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername) && s.StartDate >= startdate && s.EndDate <= enddate && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mship--date--pmode

        public List<MemberViewModel> MembershipReports(int gymid,string membership, DateTime startdate, DateTime enddate, string paymentmode, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                   s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim())
 && s.StartDate >= startdate && s.EndDate <= enddate && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mid--mship--date--pmode

        public List<MemberViewModel> MembershipReports(int memberid,string membership, DateTime startdate, DateTime enddate, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                 m.MemberID==memberid && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim())
 && s.StartDate >= startdate && s.EndDate <= enddate && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }

        //mname--mship--date--pmode

        public List<MemberViewModel> MembershipReports(string membername, string membership, DateTime startdate, DateTime enddate, string paymentmode, int gymid, int branchid)
        {

            List<MemberViewModel> membershipReport = (from m in db.MemberInfoes
                                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                                      where m.GymId == gymid && m.BranchId == branchid &&
                                                 (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername)
 && s.MembershipType.ToLower().Trim().Equals(membership.ToLower().Trim())
 && s.StartDate >= startdate && s.EndDate <= enddate && p.PaymentType.ToLower().Equals(paymentmode)
                                                      select new MemberViewModel
                                                      {
                                                          FirstName = m.FirstName + " " + m.LastName,
                                                          MobileNumber = m.MobileNumber,
                                                          MembershipType = s.MembershipType,
                                                          Months = s.Months,
                                                          RecieptNumber = p.RecieptNumber,
                                                          Amount = s.Amount,
                                                          PaymentDate = p.PaymentDate,
                                                          PaymentAmount = p.PaymentAmount,
                                                          PaymentType = p.PaymentType,
                                                          PaymentDueDate = p.PaymentDueDate,
                                                          Note = s.Note.ToString()

                                                      }).ToList();

            return membershipReport;
        }


    }
}