using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
using Justbok.BL;





namespace Justbok.Controllers
{

    public class AddProfitLoss
    {
        public string DateYear { get; set; }
        public string Payment { get; set; }
        public string Expense { get; set; }
    }

    public class TotalAttendance
    {
        public string DateYear { get; set; }
        public string PresentStaff { get; set; }
        public string PresentMembers { get; set; }
    }

    public class MemberReference
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public List<MemberReferenceList> ReferenceName { get; set; }
        public string ReferenceId { get; set; }
        public List<Nullable<System.DateTime>> JoinedDate { get; set; }
        public List<string> Membership { get; set; }
        public List<Nullable<decimal>> Amount { get; set; }
    }

    public class AttendanceDetails
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string  WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int LeaveDays { get; set; }

        public int SalaryId { get; set; }
        public int  TotalSalary { get; set; }
    }

    public class MemberReferenceList
    {
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string Membership { get; set; }
        public string JoinedDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }

    public class YearSale
    {
      public string Year{get;set;}
        public string MembershipSale{get;set;}
        public string Expense{get;set;}
        public string NetProfit{get;set;}
        public string POSSale{get;set;}
        public string Enquiry{get;set;}
        public string SoldMembership{get;set;}

    }

    public class TimesheetCalender
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string WorkingHours { get; set; }

        public string AbsentID { get; set; }
        
    }


    
      
    
    


    public class ReportsController : LayoutBaseModel
    {
        //
        // GET: /Reports/
        //public static readonly string Status = "Not Active";
        //public static readonly string MembershipType = "---Select---";
        //public static readonly string ActiveStatus = "Active";
        public static readonly string MembershipType = "---Select---";

        JustbokEntities db = new JustbokEntities();
        String pathMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private static readonly ILog Log =
           LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //Declaring Log4Net 

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult ReportTimeSheet()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        public JsonResult TimesheetReports(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime dt = DateTime.Today;
            int numberOfDays = DateTime.DaysInMonth(dt.Year, dt.Month);
            //var staffList = (from s in db.Staffs
            //                 join sb in db.StaffBranches on s.StaffId equals sb.StaffId
            //                 join T in db.TimeSheets on s.StaffId equals T.StaffId
            //                 where s.GymId==gymid && sb.BranchId == BranchId
            //                 group new {T,s} by new { T.StaffId } into lstStaff
            //                   select new
            //                   {
            //                       StaffId = lstStaff.Key.StaffId ,
            //                       Name = lstStaff.Select(x=>x.s.FirstName + " " +x.s.LastName),
            //                       WorkingDays=numberOfDays,
            //                       Present=lstStaff.Select(x=>x.T.Present),
            //                       Absent=lstStaff.Select(x=>x.T.Leave)

            //                   }).ToList();


            var staffList = (from s in db.Staffs
                             join sb in db.StaffBranches on s.StaffId equals sb.StaffId
                             where s.GymId == gymid && sb.BranchId == BranchId
                             select new
                             {
                                 StaffId = s.StaffId,
                                 Name = s.FirstName + " " + s.LastName,
                                 WorkingDays = numberOfDays
                             }).ToList();

            List<AttendanceDetails> lstAttendance = new List<AttendanceDetails>();


            foreach(var details in staffList)
            {
                var lstPresent = (from s in db.TimeSheets
                                  where s.StaffId == details.StaffId && s.AttendenceDate.Value.Month == dt.Month && s.AttendenceDate.Value.Year == dt.Year
                                  group new {s} by new {staffid =s.StaffId } into present 
                                  select new
                                  {
                                      StaffId = present.Key.staffid,
                                      Present = present.Select(x=>x.s.Present),
                                      Absent=present.Select(x=>x.s.Absent),
                                  }).ToList();
                var lstSalary = (from s in db.Salaries
                                 where s.StaffId == details.StaffId && s.SalaryDate.Value.Month == dt.Month && s.SalaryDate.Value.Year == dt.Year
                                 select new
                                 {
                                     SalaryId=s.SalaryId,
                                     SalaryAmount =s.SalaryAmount
                                 }
                                   ).ToList();

                var pCount = 0;
                var aCount = 0;
                var salaryAmount = 0;
                var salaryId = 0;

                foreach (var present in lstPresent)
                {
                    foreach (var p in present.Present)
                    {
                        if (p != null && p > 0)
                        {
                            pCount += 1;
                        }
                    }
                    foreach (var a in present.Absent)
                    {
                        if (a != null && a > 0)
                        {
                            aCount += 1;
                        }
                    }
                }

                //var presnetCount = (from p in lstPresent where p.Present != null select p.Present>0).ToList();
                //var absentCount = (from p in lstPresent where p.Absent != null select p.Absent).ToList();

                if (lstSalary.Count > 0)
                {
                   salaryAmount=Convert.ToInt32(lstSalary[0].SalaryAmount);
                   salaryId = Convert.ToInt32(lstSalary[0].SalaryId);
                }

                lstAttendance.Add(new AttendanceDetails()
                {
                    StaffId = details.StaffId,
                    SalaryId=salaryId,
                    StaffName = details.Name,
                    PresentDays = pCount,
                    LeaveDays = aCount,
                    WorkingDays =numberOfDays.ToString(),
                    TotalSalary = salaryAmount
                   
                });

            }
            // TotalSalary =Convert.ToInt32(lstSalary[0].SalaryAmount)
            
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstAttendance.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSalaryEmployee(Salary salary)
        {
            Salary objSalary = new Salary();
            objSalary.SalaryAmount = salary.SalaryAmount;
            objSalary.SalaryDate = salary.SalaryDate;
            objSalary.ReferenceNumber = salary.ReferenceNumber;
            objSalary.SalaryMode = salary.SalaryMode;
            objSalary.Comments = salary.Comments;
            objSalary.SalaryId = salary.SalaryId;
            objSalary.StaffId = salary.StaffId;
            db.Entry(objSalary).State = objSalary.SalaryId == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FullMonthCalenderBindTimeSheet(int StaffId, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime dt = DateTime.Today;
            int numberOfDays = DateTime.DaysInMonth(dt.Year, dt.Month);
            List<DateTime> lstDay = new List<DateTime>();
            for (int i = 1; i <= numberOfDays; i++)
            {
                string dateToString = "";
                if (i < 10)
                {
                     dateToString = "0" + i.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();

                }
                else
                {
                    dateToString =  i.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
                } 
                DateTime loopDate = DateTime.ParseExact(dateToString, "dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture);
                lstDay.Add(loopDate);
            
            }

           
            var lstTimesheet = (from s in db.TimeSheets
                                join t in db.Staffs on s.StaffId equals t.StaffId
                              where s.GymId==gymid && s.BranchId==BranchId && s.StaffId==StaffId && s.AttendenceDate.Value.Month == dt.Month && s.AttendenceDate.Value.Year == dt.Year
                              //group new { s } by new { staffid = s.StaffId } into present
                              select new
                              {
                                  StaffId = s.StaffId,
                                  StaffName = t.FirstName+" "+t.LastName,
                                  Present = s.Present,
                                  Absent = s.Absent,
                                  AttendanceDate=s.AttendenceDate
                              }).ToList();

            var lstPresent = (from s in db.StaffPresentDetails
                              where s.StaffId == StaffId
                              //group new { s } by new { staffid = s.StaffId } into present
                              select new
                              {
                                   PresentID = s.PresentId,
                                  StaffId = s.StaffId,
                                  InTime = s.StaffInTime,
                                  OutTime = s.StaffOutTime,
                              }).ToList();



            List<TimesheetCalender> lstCalender = new List<TimesheetCalender>();


            foreach(var date in lstDay)
            {
                string staffInTime=string.Empty;
                string staffOutTime=string.Empty;
                string absentId = "";
                var timesheetDetail = (from p in lstTimesheet where p.AttendanceDate == date select p).ToList();
               
                if(timesheetDetail.Count>0)
                {
                    absentId = timesheetDetail[0].Absent.ToString();
                    if(timesheetDetail[0].Present>0)
                    {
                        var presentDetails=(from p in lstPresent where p.PresentID == timesheetDetail[0].Present select p).ToList();
                        staffInTime=presentDetails[0].InTime;
                        staffOutTime=presentDetails[0].OutTime;
                    }
                 

                }
                //string dateToString = "";
                //if (i < 10)
                //{
                //    dateToString = "0" + date.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();

                //}
                //else
                //{
                //    dateToString = i.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString();
                //} 
                string day = date.Day.ToString();
                string dayName = date.DayOfWeek.ToString();
                dayName = dayName.Substring(0, 3);

                if (date.Day.ToString().Length < 2)
                {
                    day = "0" + date.Day.ToString();
                }
                lstCalender.Add(new TimesheetCalender{
                StaffId=StaffId,
                StaffName=lstTimesheet[0].StaffName,
                Date = day + "/" + date.Month.ToString() + "/" + date.Year.ToString() + " - " + dayName,
                InTime=staffInTime,
                OutTime=staffOutTime,
                AbsentID=absentId
                });

            }

            return Json(lstCalender, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportMembership()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult MembersList(int? page, float pagesize, int BranchId)
        {
             int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
             var memberslist = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId

                                select new
                                {
                                    MemberID = m.MemberID,
                                    FirstName = m.FirstName + " " + m.LastName,
                                    Address = m.MemberAddress.ToString(),
                                    MobileNumber = m.MobileNumber,
                                    Dob = m.Dob.ToString(),
                                    Email = m.Email,
                                    EnrollDate = m.EnrollDate.ToString(),
                                    Package = s.MembershipType,
                                    StartDate = s.StartDate.ToString(),
                                    EndDate = s.EndDate.ToString(),
                                    Amount = s.Amount,
                                    PaymentAmount = p.PaymentAmount,
                                    Status = s.Status
                                }).ToList();

             int pageIndex = 1;
             pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
             double pages = Math.Ceiling(memberslist.Count / pagesize);
             if (pageIndex > pages)
             {
                 if (pages > 0)
                 {
                     pageIndex = (int)pages;
                 }
                 else
                 {
                     pageIndex = 1;
                 }
             }

             return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
          
        }


        [HttpGet]
        public JsonResult SearchMemberList(int? page, float pagesize, string membername, string membershiptype, string startdate, string todate, string category, bool active, int branchid)

        {
            Log.Info("SearchMemberList()");
            Log.Info("startdate:"+ startdate);
               Log.Info("enddate:"+ todate);
            JustbokReports justbokReports = new JustbokReports();
            List<MemberViewModel> lstmembership = new List<MemberViewModel>();
            int memberId = 0;
            string memberName = "";
            //int membershipId = 0;
            DateTime startDate=new DateTime();
            DateTime endDate = new DateTime();
            int Category = 0;
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            string status = JustbokReports.Status;
         
            //if (membershipid != "")
            //{
            //    membershipId = Convert.ToInt32(membershipid);
            //}
            if (category != "")
            {
                Category = Convert.ToInt32(category);
            }
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                Log.Info("Converted Date" + startDate.ToString());
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (active)
            {
                status = JustbokReports.ActiveStatus;
            }

            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (memberName != "" && membershiptype == JustbokReports.MembershipType && startdate == "" && todate == "" && status == JustbokReports.Status)
            {
                lstmembership = justbokReports.MembershipReport(memberName, gymid, branchid);
            }

            else if (memberId != 0 && membershiptype == JustbokReports.MembershipType && startdate == "" && todate == "" && status == JustbokReports.Status)
            {
                lstmembership = justbokReports.MembershipReport(memberId, gymid, branchid);
            }
            else if (startdate != "" && membershiptype == JustbokReports.MembershipType && membername == "" && status == JustbokReports.Status)
            {
                if (todate != "")
                {
                    endDate = DateTime.Today.Date;
                }

                lstmembership = justbokReports.MembershipReport(startDate,endDate,gymid,branchid);
            }
            else if (membershiptype != "" && membershiptype != JustbokReports.MembershipType && startdate == "" && todate == "" && membername == "" && status == JustbokReports.Status)
            {
               
                lstmembership = justbokReports.MembershipReport(gymid, membershiptype, branchid);
            }
            else if (status != "" && status != JustbokReports.Status && startdate == "" && todate == "" && membershiptype == JustbokReports.MembershipType && membername == "")
            {
                lstmembership = justbokReports.MembershipReport(gymid, branchid, status);
            }

            else
            {
                lstmembership = justbokReports.MembershipReport(membername, membershiptype, startDate, endDate, status, gymid, branchid);

            }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstmembership.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }
            return Json(new { Pages = pages, Result = lstmembership.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BindMembership()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberShip = (from gp in db.MembershipOffers
                              where gp.GymId == gymid
                              select new
                              {
                                  MembershipOfferId = gp.MembershipOfferId,
                                  MemershipType = gp.OfferName+" "+gp.Months+" Month "+"("+gp.Amount+")"
                              }).ToList();
            return Json(memberShip, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportDueMembership()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
             
        }

        [HttpGet]
        public JsonResult SearchDueMemberList(int? page, float pagesize, string membername, string startdate, string todate,int branchid)
        {
            int memberId = 0;
            string memberName = "";
            int membershipId = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (memberName != "" && memberName != null)
            {

                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where ((mi.FirstName + " " + mi.LastName).Contains(memberName) || ms.MembershipId == membershipId
                                  || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            else if (memberId != 0)
            {

                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (mi.MemberID == memberId || ms.MembershipId == membershipId
                                  || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if(startdate!="")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }

                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (ms.StartDate >= startDate && ms.EndDate <= endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (ms.MembershipId == membershipId
                                  || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferredMembership()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();

        }

        public JsonResult TransfferdMembershipList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId && s.Status == "Transfered"

                               select new
                               {
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   EndDate = s.EndDate.ToString(),
                                   Months = s.Months,
                                   Amount = s.Amount,
                                   PaymentAmount = p.PaymentAmount
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberslist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchTransfferdMembershipList(int? page, float pagesize,string membername,string startdate,string enddate ,int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            long phoneNumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                phoneNumber = long.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (phoneNumber > 0)
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && m.MobileNumber == phoneNumber && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (memberName != "")
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(memberName.ToLower()) && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if(startdate!="")
                {
                     if(enddate=="")
                     {
                         endDate=DateTime.Today.Date;
                     }
                     var memberslist = (from m in db.MemberInfoes
                                        join s in db.MemberShips on m.MemberID equals s.MemberID
                                        join p in db.Payments on s.MembershipId equals p.MembershipId
                                        where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= startDate && s.EndDate <= endDate && s.Status == "Transfered" 

                                        select new
                                        {
                                            FirstName = m.FirstName + " " + m.LastName,
                                            MobileNumber = m.MobileNumber,
                                            Package = s.MembershipType,
                                            StartDate = s.StartDate.ToString(),
                                            EndDate = s.EndDate.ToString(),
                                            Months = s.Months,
                                            Amount = s.Amount,
                                            PaymentAmount = p.PaymentAmount
                                        }).ToList();

                     int pageIndex = 1;
                     pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                     double pages = Math.Ceiling(memberslist.Count / pagesize);
                     if (pageIndex > pages)
                     {
                         if (pages > 0)
                         {
                             pageIndex = (int)pages;
                         }
                         else
                         {
                             pageIndex = 1;
                         }
                     }

                     return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                }
            else
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid  && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }




            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportsFreezeMembership()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();

        }


        public JsonResult FrrezeMembershipList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from f in db.MemberFreezes
                               join m in db.MemberInfoes on f.Member_Id equals m.MemberID
                               join s in db.MemberShips on f.MemberShipId equals s.MembershipId
                               join p in db.Payments on f.MemberShipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId
                               group new { f,m,s,p} by new
                               {
                                   MemberShipId = f.MemberShipId,

                               } into lstFreeze
                               select new
                               {
                                    MemberShipId = lstFreeze.Key.MemberShipId,
                                   FirstName = lstFreeze.Select(x => x.m.FirstName),
                                   LastName = lstFreeze.Select(x => x.m.LastName),
                                   MobileNumber = lstFreeze.Select(x => x.m.MobileNumber),
                                   Package = lstFreeze.Select(x => x.s.MembershipType),
                                   StartDate = lstFreeze.Select(x => x.f.StartDate),
                                   EndDate = lstFreeze.Select(x => x.f.EndDate),
                                   Months = lstFreeze.Select(x => x.s.Months),
                                   Amount = lstFreeze.Select(x => x.s.Amount),
                                   PaymentAmount = lstFreeze.Sum(x => x.p.PaymentAmount)

                               }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberslist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchFreezeMembershipList(int? page, float pagesize, string membername, string startdate, string enddate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            long phoneNumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                phoneNumber = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (phoneNumber > 0)
            {
                var memberslist = (from f in db.MemberFreezes
                                   join m in db.MemberInfoes on f.Member_Id equals m.MemberID
                                   join s in db.MemberShips on f.MemberShipId equals s.MembershipId
                                   join p in db.Payments on f.MemberShipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && m.MobileNumber==phoneNumber
                                   group new { f, m, s, p } by new
                                   {
                                       MemberShipId = f.MemberShipId,

                                   } into lstFreeze
                                   select new
                                   {
                                       MemberShipId = lstFreeze.Key.MemberShipId,
                                       FirstName = lstFreeze.Select(x => x.m.FirstName),
                                       LastName = lstFreeze.Select(x => x.m.LastName),
                                       MobileNumber = lstFreeze.Select(x => x.m.MobileNumber),
                                       Package = lstFreeze.Select(x => x.s.MembershipType),
                                       StartDate = lstFreeze.Select(x => x.f.StartDate),
                                       EndDate = lstFreeze.Select(x => x.f.EndDate),
                                       Months = lstFreeze.Select(x => x.s.Months),
                                       Amount = lstFreeze.Select(x => x.s.Amount),
                                       PaymentAmount = lstFreeze.Sum(x => x.p.PaymentAmount)

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (memberName != "")
            {
                var memberslist = (from f in db.MemberFreezes
                                   join m in db.MemberInfoes on f.Member_Id equals m.MemberID
                                   join s in db.MemberShips on f.MemberShipId equals s.MembershipId
                                   join p in db.Payments on f.MemberShipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && (m.FirstName.ToLower()+" "+m.LastName.ToLower()).Contains(memberName.ToLower())
                                   group new { f, m, s, p } by new
                                   {
                                       MemberShipId = f.MemberShipId,

                                   } into lstFreeze
                                   select new
                                   {
                                       MemberShipId = lstFreeze.Key.MemberShipId,
                                       FirstName = lstFreeze.Select(x => x.m.FirstName),
                                       LastName = lstFreeze.Select(x => x.m.LastName),
                                       MobileNumber = lstFreeze.Select(x => x.m.MobileNumber),
                                       Package = lstFreeze.Select(x => x.s.MembershipType),
                                       StartDate = lstFreeze.Select(x => x.f.StartDate),
                                       EndDate = lstFreeze.Select(x => x.f.EndDate),
                                       Months = lstFreeze.Select(x => x.s.Months),
                                       Amount = lstFreeze.Select(x => x.s.Amount),
                                       PaymentAmount = lstFreeze.Sum(x => x.p.PaymentAmount)

                                   }).ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (startdate != "")
            {
                if (enddate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberslist = (from f in db.MemberFreezes
                                   join m in db.MemberInfoes on f.Member_Id equals m.MemberID
                                   join s in db.MemberShips on f.MemberShipId equals s.MembershipId
                                   join p in db.Payments on f.MemberShipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && f.StartDate>=startDate && f.EndDate<=endDate
                                   group new { f, m, s, p } by new
                                   {
                                       MemberShipId = f.MemberShipId,

                                   } into lstFreeze
                                   select new
                                   {
                                       MemberShipId = lstFreeze.Key.MemberShipId,
                                       FirstName = lstFreeze.Select(x => x.m.FirstName),
                                       LastName = lstFreeze.Select(x => x.m.LastName),
                                       MobileNumber = lstFreeze.Select(x => x.m.MobileNumber),
                                       Package = lstFreeze.Select(x => x.s.MembershipType),
                                       StartDate = lstFreeze.Select(x => x.f.StartDate),
                                       EndDate = lstFreeze.Select(x => x.f.EndDate),
                                       Months = lstFreeze.Select(x => x.s.Months),
                                       Amount = lstFreeze.Select(x => x.s.Amount),
                                       PaymentAmount = lstFreeze.Sum(x => x.p.PaymentAmount)

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var memberslist = (from f in db.MemberFreezes
                                   join m in db.MemberInfoes on f.Member_Id equals m.MemberID
                                   join s in db.MemberShips on f.MemberShipId equals s.MembershipId
                                   join p in db.Payments on f.MemberShipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId
                                   group new { f, m, s, p } by new
                                   {
                                       MemberShipId = f.MemberShipId,

                                   } into lstFreeze
                                   select new
                                   {
                                       MemberShipId = lstFreeze.Key.MemberShipId,
                                       FirstName = lstFreeze.Select(x => x.m.FirstName),
                                       LastName = lstFreeze.Select(x => x.m.LastName),
                                       MobileNumber = lstFreeze.Select(x => x.m.MobileNumber),
                                       Package = lstFreeze.Select(x => x.s.MembershipType),
                                       StartDate = lstFreeze.Select(x => x.f.StartDate),
                                       EndDate = lstFreeze.Select(x => x.f.EndDate),
                                       Months = lstFreeze.Select(x => x.s.Months),
                                       Amount = lstFreeze.Select(x => x.s.Amount),
                                       PaymentAmount = lstFreeze.Sum(x => x.p.PaymentAmount)

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }




            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportsMeasurements()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();

        }


        public JsonResult MeasurementsList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from m in db.MemberInfoes
                               join me in db.Measurements on m.MemberID equals me.MemberID
                             
                               where m.GymId == gymid && m.BranchId == BranchId
                               group new { m,me} by new
                               {
                                   MemberID = m.MemberID,

                               } into lstMeasurement
                               select new
                               {
                                   MemberID = lstMeasurement.Key.MemberID,
                                   FirstName = lstMeasurement.Select(x => x.m.FirstName),
                                   LastName = lstMeasurement.Select(x => x.m.LastName),
                                   MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber),
                                   NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate),
                                   MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate),

                               }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberslist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchMeasurementsList(int? page, float pagesize, string membername, string startdate, string enddate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            long phoneNumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                phoneNumber = long.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (phoneNumber > 0)
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID
                                   where m.GymId == gymid && m.BranchId == BranchId && m.MobileNumber==phoneNumber
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName),
                                       LastName = lstMeasurement.Select(x => x.m.LastName),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate),

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (memberName != "")
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId && (m.FirstName.ToLower()+" "+m.LastName.ToLower()).Contains(memberName.ToLower())
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName),
                                       LastName = lstMeasurement.Select(x => x.m.LastName),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate),

                                   }).ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (startdate != "")
            {
                if (enddate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId && me.MeasurementDate >=startDate && me.MeasurementDate<=endDate
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName),
                                       LastName = lstMeasurement.Select(x => x.m.LastName),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate),

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName),
                                       LastName = lstMeasurement.Select(x => x.m.LastName),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate),

                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberslist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }




            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReportExpiredMembership()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult ExpiredMembersList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               where m.GymId == gymid && m.BranchId == BranchId && s.EndDate < DateTime.Now

                               select new
                               {
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   EndDate = s.EndDate.ToString(),
                                   Months = s.Months
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberslist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchExpiredMemberList(int? page, float pagesize, string membername, string membership, string startdate, string todate, int branchid)
        {
            int memberId = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            MembershipExpriyReports expiryReports =new  MembershipExpriyReports();
            List<MemberViewModel> lstExpiryReport = new List<MemberViewModel>();

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            //memberid
            if (memberId > 0 && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && membership==MembershipExpriyReports.MembershipType)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberId, gymid, branchid);
            }
                //membername
            else if (!string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && membership == MembershipExpriyReports.MembershipType)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberName, gymid, branchid);
            }
                //membership
            else if (!string.IsNullOrEmpty(membership) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId == 0 && string.IsNullOrEmpty(memberName))
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(gymid, membership, branchid);
            }
                //date
            else if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership == MembershipExpriyReports.MembershipType && memberId == 0 && string.IsNullOrEmpty(memberName))
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(startDate,endDate,gymid,branchid);
            }
            //membername --date
            else if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership == MembershipExpriyReports.MembershipType)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberName, startDate, endDate, gymid, branchid);
            }
            //memberid--date

            else if (memberId > 0 && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership == MembershipExpriyReports.MembershipType)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberId,startDate,endDate,gymid,branchid);
            }
            //mebername --- mtype
            else if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membership) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId == 0)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberName,membership, gymid, branchid);
            }
            //mebernid--- mtype
            else if (string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membership) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId > 0)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberId,membership, gymid, branchid);
            }

            //mtype--- date
            else if (string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membership) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId == 0)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(membership,gymid,branchid, startDate, endDate);
            }
            //mambername--mtype--- date
            else if (!string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membership) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId == 0)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberName,membership, startDate, endDate, gymid, branchid);
            }
            //mamberid--mtype--- date
            else if (string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(membership) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && membership != MembershipExpriyReports.MembershipType && memberId > 0)
            {
                lstExpiryReport = expiryReports.ExpiredMembershipReports(memberId, membership, startDate, endDate, gymid, branchid);
            }







            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstExpiryReport.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }


            return Json(new { Pages = pages, Result = lstExpiryReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportMembershipPayments()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult CategoerySale()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult CategoryWiseSale()
        {

            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberspaymentlist = (from m in db.MemberShips
                                     select new
                                      {
                                          MembershipType = m.MembershipType,
                                          Amount = m.Amount
                                      }).ToList();

                                   


            //int pageIndex = 1;
            //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //double pages = Math.Ceiling(memberspaymentlist.Count / pagesize);
            //if (pageIndex > pages)
            //{
            //    if (pages > 0)
            //    {
            //        pageIndex = (int)pages;
            //    }
            //    else
            //    {
            //        pageIndex = 1;
            //    }
            //}

            return Json(memberspaymentlist, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]

        public JsonResult SearchCategoerySale(string startDate,String endDate)
        {
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate != "")
            {
                StartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                EndDate = DateTime.Now.Date;
            }

            if (startDate != "" && endDate != "")
            {
                var lstcategoerywise = (from mi in db.MemberShips
                                        where mi.StartDate >= StartDate && mi.EndDate <= EndDate

                                        select new
                                        {
                                            MembershipType = mi.MembershipType,
                                            StartDate = mi.StartDate,
                                            EndDate = mi.EndDate,
                                            Amount = mi.Amount
                                        }).ToList();

                return Json(lstcategoerywise, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var memberspaymentlist = (from m in db.MemberShips
                                          select new
                                          {
                                              MembershipType = m.MembershipType,
                                              Amount = m.Amount
                                          }).ToList();
                return Json(memberspaymentlist, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
            
        }


        public ActionResult DuePayments()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult DuePaymentList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId

                               select new
                               {
                                   MemberID = m.MemberID,
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   EnrollDate = m.EnrollDate.ToString(),
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   EndDate = s.EndDate.ToString(),
                                   Amount = s.Amount,
                                   PaymentAmount = p.PaymentAmount,
                                   PaymentDueDate = p.PaymentDueDate.ToString()
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberslist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult PendingPayments(int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var pendingList = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId

                               select new
                               {
                                   MemberID = m.MemberID,
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   EnrollDate = m.EnrollDate.ToString(),
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   EndDate = s.EndDate.ToString(),
                                   Amount = s.Amount,
                                   PaymentAmount = p.PaymentAmount,
                                   PaymentDueDate = p.PaymentDueDate.ToString()
                               }).ToList();

            return Json(pendingList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchDuePaymentList(int? page, float pagesize, string membername,string startdate, string todate,int branchid)
        {

            int memberId = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (memberName != "" && memberName != null)
            {

                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where ((m.FirstName + " " + m.LastName).Contains(memberName) 
                                  || (s.StartDate >= startDate && s.EndDate == endDate)) && m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate.ToString(),
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate.ToString(),
                                      EndDate = s.EndDate.ToString(),
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate.ToString()

                                  }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            else if (memberId != 0)
            {

                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where (m.MemberID == memberId ||(s.StartDate >= startDate && s.EndDate <= endDate)) && m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate.ToString(),
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate.ToString(),
                                      EndDate = s.EndDate.ToString(),
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate.ToString()

                                  }).ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where ( s.StartDate >= startDate && s.EndDate <= endDate) && m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate.ToString(),
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate.ToString(),
                                      EndDate = s.EndDate.ToString(),
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate.ToString()

                                  }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(memberlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = memberlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }


            return Json("", JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult MemberPaymentsList(int? page, float pagesize, int BranchId)
        {
           
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberspaymentlist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId 

                               select new
                               {
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   Package = s.MembershipType,
                                   Months = s.Months,
                                   RecieptNumber = p.RecieptNumber,
                                   Amount = s.Amount,
                                   PaymentDate = p.PaymentDate.ToString(),
                                   PaymentAmount = p.PaymentAmount,
                                   PaymentType = p.PaymentType,
                                   PaymentDueDate = p.PaymentDueDate.ToString(),
                                   Note = s.Note.ToString()
                                  

                               }).ToList();


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(memberspaymentlist.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = memberspaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public JsonResult SearchMembersPaymentList(int? page, float pagesize, string membername, string startdate, string todate,string membership,string category,string paymentMode, int branchid)
        {
            List<MemberViewModel> lstMembership = new List<MemberViewModel>();
            MembershipPaymentReport report = new MembershipPaymentReport();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            int memberid = 0;
            string memberName="";

            try
            {
                memberid = Convert.ToInt32(membername);
            }
            catch (Exception ex)
            {
               memberName=membername;
            }

            //memberid
            if(memberid>0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) &&  membership.Equals(MembershipPaymentReport.MembershipType))
            {
               lstMembership= report.MembershipReports(memberid, gymid, branchid);
            }
                //membername
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, gymid, branchid);
            }
            //membership
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid, branchid, membership);
            }
            //date
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(startDate,endDate,gymid, branchid);
            }
            //pmode
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid,paymentMode, branchid);
            }
                //mid -- date
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid,startDate,endDate,gymid,branchid);
            }
            //mid -- mship
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid, membership, gymid, branchid);
            }
            //mid -- pmode
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid,gymid,paymentMode, branchid);
            }
            //mname -- date
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, startDate, endDate, gymid, branchid);
            }
            //mname -- mship
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, membership, gymid, branchid);
            }
            //mname -- pmode
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid, gymid,paymentMode, branchid);
            }
            //mship -- date
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid, branchid,membership,startDate,endDate);
            }

            //mship -- pmode
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid, branchid,membership,paymentMode);
            }

            //date -- pmode
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid, branchid,startDate, endDate,paymentMode );
            }

             //mid mship date
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid,membership, startDate, endDate, gymid, branchid);
            }

             //mname mship date
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, membership, startDate, endDate, gymid, branchid);
            }

              //mid--mship--pmode
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid,membership,paymentMode, gymid, branchid);
            }
            //mname--mship--pmode
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, membership, paymentMode, gymid, branchid);
            }

               //mid-date--pmode
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid, startDate, endDate,paymentMode, gymid, branchid);
            }

             //mname-date--pmode
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, startDate, endDate, paymentMode, gymid, branchid);
            }
            //mship-date--pmode
            else if (memberid == 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(gymid,membership, startDate, endDate, paymentMode, branchid);
            }
            //mid-mship-date--pmode
            else if (memberid > 0 && string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberid,membership, startDate, endDate,paymentMode, gymid, branchid);
            }
            //mname-mship-date--pmode
            else if (memberid == 0 && !string.IsNullOrEmpty(memberName) && !string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !paymentMode.Equals(MembershipPaymentReport.PaymentType) && !membership.Equals(MembershipPaymentReport.MembershipType))
            {
                lstMembership = report.MembershipReports(memberName, membership, startDate, endDate, paymentMode, gymid, branchid);
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMembership.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstMembership.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            
          

         
        }

        public ActionResult MonthWisePaymnetReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        

        public JsonResult MonthWiseReport(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstMonthWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId
                                group p by new { month = p.PaymentDate.Value.Month, year = p.PaymentDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.month + " " + lstSale.Key.year,
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                    PaymentType = lstSale.Select(x => new {x.PaymentType,x.PaymentAmount })
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMonthWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstMonthWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SearchMonthWiseReport(int? page, float pagesize,string startDate,string endDate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startdate=new DateTime();
            DateTime enddate=new DateTime();
                   if (startDate != "")
            {
                startdate = DateTime.ParseExact(startDate, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
               enddate  = DateTime.ParseExact(endDate, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            var lstMonthWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId && p.PaymentDate>= startdate && p.PaymentDate<=enddate 
                                group p by new { month = p.PaymentDate.Value.Month, year = p.PaymentDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.month + " " + lstSale.Key.year,
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                    PaymentType = lstSale.Select(x => new { x.PaymentType, x.PaymentAmount })
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMonthWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstMonthWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult DayWiseSaleReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult DayWiseReport(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstDayWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId
                              group p by new { day = p.PaymentDate} into lstSale
                                //,paymentType=p.PaymentType
                                select new
                                {

                                    dt = lstSale.Key.day.ToString(),
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                    PaymentType = lstSale.Select(x => new { x.PaymentType, x.PaymentAmount })
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstDayWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstDayWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult MembershipWiseSale()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        [HttpGet]
        public JsonResult MemberWiseSale(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
 
                var lstMemberwiseSale = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId==gymid && m.BranchId==BranchId
                                          group s by s.MembershipType  into lstPayments
                                         
                                          select new
                                          {

                                              Package = lstPayments.Key,
                                              Amount = lstPayments.Sum(x=>x.Amount),
                                              ToatlSale=lstPayments.Count()

                                          }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberwiseSale.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstMemberwiseSale.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult SearchMemberwiseList(int? page, float pagesize,string startdate, string todate, int branchid)
        {

          
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (startdate != null)
            {
                var lstMemberwiseSale = (from m in db.MemberInfoes
                                         join s in db.MemberShips on m.MemberID equals s.MemberID
                                         join p in db.Payments on s.MembershipId equals p.MembershipId
                                         where m.GymId == gymid && m.BranchId == branchid && (p.PaymentDate >= startDate && p.PaymentDate <=endDate)
                                         group s by s.MembershipType into lstPayments

                                         select new
                                         {

                                             Package = lstPayments.Key,
                                             Amount = lstPayments.Sum(x => x.Amount),
                                             ToatlSale = lstPayments.Count()

                                         }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberwiseSale.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = lstMemberwiseSale.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var lstMemberwiseSale = (from m in db.MemberInfoes
                                         join s in db.MemberShips on m.MemberID equals s.MemberID
                                         join p in db.Payments on s.MembershipId equals p.MembershipId
                                         where m.GymId == gymid && m.BranchId == branchid
                                         group s by s.MembershipType into lstPayments

                                         select new
                                         {

                                             Package = lstPayments.Key,
                                             Amount = lstPayments.Sum(x => x.Amount),
                                             ToatlSale = lstPayments.Count()

                                         }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberwiseSale.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstMemberwiseSale.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            
            }

            return Json("", JsonRequestBehavior.AllowGet);
           
        }


        [HttpGet]
        public ActionResult ProfitLoss()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult ProfitLostList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           
            List<AddProfitLoss> lstProfit = new List<AddProfitLoss>();

            var lstExpense = (from e in db.Expenses 
                                where e.GymId == gymid && e.BranchId == BranchId
                                group e by new { day = e.ExpenseDate.Value.Month+" "+e.ExpenseDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.day,
                                    expensemount = lstSale.Sum(x => x.ExpenseAmount),
                                }).ToList();

            var lstMonthWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId
                                group p by new { day = p.PaymentDate.Value.Month + " " + p.PaymentDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.day,
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                }).ToList();

            int thisYear=DateTime.Today.Year;

            List<string> dateYear = new List<string>();
            dateYear.Add("1" + " " + thisYear);
            dateYear.Add("2" + " " + thisYear);
            dateYear.Add("3" + " " + thisYear);
            dateYear.Add("4" + " " + thisYear);
            dateYear.Add("5" + " " + thisYear);
            dateYear.Add("6" + " " + thisYear);
            dateYear.Add("7" + " " + thisYear);
            dateYear.Add("8" + " " + thisYear);
            dateYear.Add("9" + " " + thisYear);
            dateYear.Add("10" + " " + thisYear);
            dateYear.Add("11" + " " + thisYear);
            dateYear.Add("12" + " " + thisYear);

            foreach (string date in dateYear)
            {
                var payment = (from p in lstMonthWise
                              where p.dt == date
                              select new
                              {totalpayment= p.PaymentAmount}).ToList();
                var expense = (from e in lstExpense
                               where e.dt == date
                               select new {totalExpense= e.expensemount }).ToList();

                lstProfit.Add(new AddProfitLoss() {DateYear=date,Payment=payment.Count>0 ? payment[0].totalpayment.ToString():"0",Expense=expense.Count>0?expense[0].totalExpense.ToString() :"0" });
            
            }


          

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMonthWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstProfit.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchProfitLostList(int? page, float pagesize,string startMonth,string endMonth, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startMonth != "")
            {
                StartDate = DateTime.ParseExact(startMonth, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endMonth != "")
            {
                EndDate = DateTime.ParseExact(endMonth, "MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            List<AddProfitLoss> lstProfit = new List<AddProfitLoss>();

            var lstExpense = (from e in db.Expenses
                              where e.GymId == gymid && e.BranchId == BranchId && e.ExpenseDate>=StartDate && e.ExpenseDate<=EndDate
                              group e by new { day = e.ExpenseDate.Value.Month + " " + e.ExpenseDate.Value.Year } into lstSale
                              select new
                              {
                                  dt = lstSale.Key.day,
                                  expensemount = lstSale.Sum(x => x.ExpenseAmount),
                              }).ToList();

            var lstMonthWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == BranchId && p.PaymentDate>=StartDate && p.PaymentDate<=EndDate
                                group p by new { day = p.PaymentDate.Value.Month + " " + p.PaymentDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.day,
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                }).ToList();

            int thisYear = EndDate.Year;
            int month = 0;
            if (endMonth != "")
            {
                month = EndDate.Month;
            }
           


            List<string> dateYear = new List<string>();
            if(month>0)
            {


                for (int i = 1; i <= month; i++)
                {
                    dateYear.Add(i + " " + thisYear);
                }
            foreach (string date in dateYear)
            {
                var payment = (from p in lstMonthWise
                               where p.dt == date
                               select new { totalpayment = p.PaymentAmount }).ToList();
                var expense = (from e in lstExpense
                               where e.dt == date
                               select new { totalExpense = e.expensemount }).ToList();

                lstProfit.Add(new AddProfitLoss() { DateYear = date, Payment = payment.Count > 0 ? payment[0].totalpayment.ToString() : "0", Expense = expense.Count > 0 ? expense[0].totalExpense.ToString() : "0" });

            }

                }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMonthWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstProfit.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult BranchWiseGymSales()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult BranchWiseSale(int? page, float pagesize)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lsBranchWise = (from m in db.MemberInfoes
                              join s in db.MemberShips on m.MemberID equals s.MemberID
                              join p in db.Payments on s.MembershipId equals p.MembershipId
                              join b in db.Branches on m.BranchId equals b.BranchId
                              where m.GymId == gymid
                              group p by new { day = p.PaymentDate,branch=b.BranchName } into lstSale
                              //,paymentType=p.PaymentType
                              select new
                              {

                                  dt = lstSale.Key.day.ToString(),
                                  PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                  BranchName=lstSale.Key.branch,
                                  membershipCount = lstSale.Select(x => x.MembershipId)
                              }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lsBranchWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lsBranchWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchBranchWiseSale(int? page, float pagesize,string startDate, String endDate)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate != "")
            {
                StartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                EndDate = DateTime.Now.Date;
            }

            if (startDate != "" && endDate != "")
            {
                 var lsBranchWise = (from m in db.MemberInfoes
                              join s in db.MemberShips on m.MemberID equals s.MemberID
                              join p in db.Payments on s.MembershipId equals p.MembershipId
                              join b in db.Branches on m.BranchId equals b.BranchId
                              where m.GymId == gymid &&  p.PaymentDate>=StartDate && p.PaymentDate<=EndDate
                              group p by new { day = p.PaymentDate,branch=b.BranchName } into lstSale
                              //,paymentType=p.PaymentType
                              select new
                              {

                                  dt = lstSale.Key.day.ToString(),
                                  PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                  BranchName=lstSale.Key.branch,
                                  membershipCount = lstSale.Select(x => x.MembershipId)
                              }).ToList();

                 int pageIndex = 1;
                 pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                 double pages = Math.Ceiling(lsBranchWise.Count / pagesize);
                 if (pageIndex > pages)
                 {
                     if (pages > 0)
                     {
                         pageIndex = (int)pages;
                     }
                     else
                     {
                         pageIndex = 1;
                     }
                 }

                 return Json(new { Pages = pages, Result = lsBranchWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lsBranchWise = (from m in db.MemberInfoes
                                    join s in db.MemberShips on m.MemberID equals s.MemberID
                                    join p in db.Payments on s.MembershipId equals p.MembershipId
                                    join b in db.Branches on m.BranchId equals b.BranchId
                                    where m.GymId == gymid
                                    group p by new { day = p.PaymentDate, branch = b.BranchName } into lstSale
                                    //,paymentType=p.PaymentType
                                    select new
                                    {

                                        dt = lstSale.Key.day.ToString(),
                                        PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                        BranchName = lstSale.Key.branch,
                                        membershipCount = lstSale.Select(x => x.MembershipId)
                                    }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lsBranchWise.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lsBranchWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult PosSalesReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult PosSaleReport(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstPosReport = (from po in db.PurchaseOrders
                                join o in db.Orders on po.PurchaseOrderId equals o.PurchaseOrderId
                                where po.GymId == gymid && po.BranchId==BranchId
                                group new { po,o} by new {day = po.OrderDate,total=o.Total} into lstPos
                                //group po by new { day = po.OrderDate} into lstPos
                                select new
                                {
                                    dt = lstPos.Key.day.ToString(),
                                    //PaymentVia = lstPos.Select(x=>x.po.PaymentVia),
                                    PaymentVia = lstPos.Select(x => new { x.po.PaymentVia, x.o.Total })
                                    //Total = lstPos.Sum(x=>x.o.Total)
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstPosReport.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstPosReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchPosSale(int? page, float pagesize, string startDate, String endDate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate != "")
            {
                StartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                EndDate = DateTime.Now.Date;
            }

            if (startDate != "" && endDate != "")
            {
                var lstPosReport = (from po in db.PurchaseOrders
                                    join o in db.Orders on po.PurchaseOrderId equals o.PurchaseOrderId
                                    where po.GymId == gymid && po.BranchId == BranchId && po.OrderDate>=StartDate && po.OrderDate<=EndDate
                                    group new { po, o } by new { day = po.OrderDate, total = o.Total } into lstPos
                                    select new
                                    {
                                        dt = lstPos.Key.day.ToString(),
                                        //PaymentVia = lstPos.Select(x=>x.po.PaymentVia),
                                        PaymentVia = lstPos.Select(x => new { x.po.PaymentVia, x.o.Total })
                                        //Total = lstPos.Sum(x=>x.o.Total)
                                    }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstPosReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstPosReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstPosReport = (from po in db.PurchaseOrders
                                    join o in db.Orders on po.PurchaseOrderId equals o.PurchaseOrderId
                                    where po.GymId == gymid && po.BranchId == BranchId
                                    group new { po, o } by new { day = po.OrderDate, total = o.Total } into lstPos
                                    //group po by new { day = po.OrderDate} into lstPos
                                    select new
                                    {
                                        dt = lstPos.Key.day.ToString(),
                                        //PaymentVia = lstPos.Select(x=>x.po.PaymentVia),
                                        PaymentVia = lstPos.Select(x => new { x.po.PaymentVia, x.o.Total })
                                        //Total = lstPos.Sum(x=>x.o.Total)
                                    }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstPosReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstPosReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult StockReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult StockReportList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstStockReport = (from p in db.Products
                                join s in db.Stocks on p.ProductId equals s.ProductId
                                where p.GymId == gymid && p.BranchId == BranchId
                                group new {p,s} by new { product = p.ProductName,manufacture=p.BrandName,stockin=s.StockIn,
                                stockout=s.StockOut,date=s.StockinDate,price=p.Price} into lstProductDetails
                                select new
                                {
                                    Product = lstProductDetails.Key.product,
                                    Manufacture=lstProductDetails.Key.manufacture.ToString(),
                                    StockIn = lstProductDetails.Key.stockin.ToString(),
                                    StockOut = lstProductDetails.Key.stockout.ToString(),
                                    Date = lstProductDetails.Key.date.ToString(),
                                    TotalCharges = lstProductDetails.Key.price.ToString()
                                   
                                    //Total = lstPos.Sum(x=>x.o.Total)
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstStockReport.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstStockReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult SearchStockReportList(int? page, float pagesize, string productName, string startDate, string endDate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate != "")
            {
                StartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                EndDate = DateTime.Now.Date;
            }

            if (startDate != "" && endDate != "" && productName!="")
            {
                var lstStockReport = (from p in db.Products
                                      join s in db.Stocks on p.ProductId equals s.ProductId
                                      where p.GymId == gymid && p.BranchId == BranchId && p.ProductName==productName && s.StockinDate>=StartDate && s.StockinDate<=EndDate
                                      group new { p, s } by new
                                      {
                                          product = p.ProductName,
                                          manufacture = p.BrandName,
                                          stockin = s.StockIn,
                                          stockout = s.StockOut,
                                          date = s.StockinDate,
                                          price = p.Price
                                      } into lstProductDetails
                                      select new
                                      {
                                          Product = lstProductDetails.Key.product,
                                          Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                          StockIn = lstProductDetails.Key.stockin.ToString(),
                                          StockOut = lstProductDetails.Key.stockout.ToString(),
                                          Date = lstProductDetails.Key.date.ToString(),
                                          TotalCharges = lstProductDetails.Key.price.ToString()

                                          //Total = lstPos.Sum(x=>x.o.Total)
                                      }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstStockReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstStockReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if(productName!="" && (startDate=="" && endDate==""))
            {
                var lstStockReport = (from p in db.Products
                                      join s in db.Stocks on p.ProductId equals s.ProductId
                                      where p.GymId == gymid && p.BranchId == BranchId && p.ProductName.Trim().ToUpper().Contains(productName.Trim()) 
                                      group new { p, s } by new
                                      {
                                          product = p.ProductName,
                                          manufacture = p.BrandName,
                                          stockin = s.StockIn,
                                          stockout = s.StockOut,
                                          date = s.StockinDate,
                                          price = p.Price
                                      } into lstProductDetails
                                      select new
                                      {
                                          Product = lstProductDetails.Key.product,
                                          Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                          StockIn = lstProductDetails.Key.stockin.ToString(),
                                          StockOut = lstProductDetails.Key.stockout.ToString(),
                                          Date = lstProductDetails.Key.date.ToString(),
                                          TotalCharges = lstProductDetails.Key.price.ToString()

                                          //Total = lstPos.Sum(x=>x.o.Total)
                                      }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstStockReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstStockReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (productName == "" && startDate != "" && endDate != "")
            {
                var lstStockReport = (from p in db.Products
                                      join s in db.Stocks on p.ProductId equals s.ProductId
                                      where p.GymId == gymid && p.BranchId == BranchId && s.StockinDate >= StartDate && s.StockinDate <= EndDate
                                      group new { p, s } by new
                                      {
                                          product = p.ProductName,
                                          manufacture = p.BrandName,
                                          stockin = s.StockIn,
                                          stockout = s.StockOut,
                                          date = s.StockinDate,
                                          price = p.Price
                                      } into lstProductDetails
                                      select new
                                      {
                                          Product = lstProductDetails.Key.product,
                                          Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                          StockIn = lstProductDetails.Key.stockin.ToString(),
                                          StockOut = lstProductDetails.Key.stockout.ToString(),
                                          Date = lstProductDetails.Key.date.ToString(),
                                          TotalCharges = lstProductDetails.Key.price.ToString()

                                          //Total = lstPos.Sum(x=>x.o.Total)
                                      }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstStockReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstStockReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstStockReport = (from p in db.Products
                                      join s in db.Stocks on p.ProductId equals s.ProductId
                                      where p.GymId == gymid && p.BranchId == BranchId
                                      group new { p, s } by new
                                      {
                                          product = p.ProductName,
                                          manufacture = p.BrandName,
                                          stockin = s.StockIn,
                                          stockout = s.StockOut,
                                          date = s.StockinDate,
                                          price = p.Price
                                      } into lstProductDetails
                                      select new
                                      {
                                          Product = lstProductDetails.Key.product,
                                          Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                          StockIn = lstProductDetails.Key.stockin.ToString(),
                                          StockOut = lstProductDetails.Key.stockout.ToString(),
                                          Date = lstProductDetails.Key.date.ToString(),
                                          TotalCharges = lstProductDetails.Key.price.ToString()

                                          //Total = lstPos.Sum(x=>x.o.Total)
                                      }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstStockReport.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstStockReport.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

       

        [HttpGet]
        public ActionResult SourceWiseEnquiry()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult SourceEnquiryList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstSourceEnq = (from e in db.Enquiries
                                  where e.GymId == gymid && e.BranchId == BranchId
                                group e by new { source = e.HowDidYouKnow} into lstenq
                                  
                                  select new
                                  {
                                      Source = lstenq.Key.source,
                                      Total=lstenq.Select(x=>x.HowDidYouKnow)
                                  }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstSourceEnq.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstSourceEnq.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchSourceWiseEnqReportList(int? page, float pagesize, string startDate, string endDate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();
            if (startDate != "")
            {
                StartDate = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endDate != "")
            {
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                EndDate = DateTime.Now.Date;
            }

            if (startDate != "" && endDate != "")
            {
                var lstSourceEnq = (from e in db.Enquiries
                                    where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate >=StartDate && e.EnquiryDate <=EndDate
                                    group e by new { source = e.HowDidYouKnow } into lstenq

                                    select new
                                    {
                                        Source = lstenq.Key.source,
                                        Total = lstenq.Select(x => x.HowDidYouKnow)
                                    }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstSourceEnq.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstSourceEnq.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstSourceEnq = (from e in db.Enquiries
                                    where e.GymId == gymid && e.BranchId == BranchId
                                    group e by new { source = e.HowDidYouKnow } into lstenq

                                    select new
                                    {
                                        Source = lstenq.Key.source,
                                        Total = lstenq.Select(x => x.HowDidYouKnow)
                                    }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstSourceEnq.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstSourceEnq.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult TrailDateCompletionReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult TrailDateCompletionList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstTrailOffered = (from e in db.Enquiries
                                where e.GymId == gymid && e.BranchId == BranchId
                                   group e by new { enqid = e.EnquiryId, membername = e.FirstName+" "+ e.LastName,mobilenumber=e.MobileNumber,
                                       enqDate=e.EnquiryDate,trailedOffer = e.TrailOffered, traildate = e.TrailDate } into lstenq

                                select new
                                {
                                    EnquiryId = lstenq.Key.enqid,
                                    MemberName = lstenq.Key.membername,
                                    MobileNumber = lstenq.Key.mobilenumber,
                                    EnquiryDate = lstenq.Key.enqDate.ToString(),
                                    TrailOffered = lstenq.Key.trailedOffer,
                                    TrailDate = lstenq.Key.traildate.ToString()
                                }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstTrailOffered.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstTrailOffered.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEnquiryDetails(int EnquiryId)
        {
            if (EnquiryId != 0)
            {
                var enquiryDetails = (from e in db.Enquiries
                                    where e.EnquiryId==EnquiryId
                                    select new
                                    {
                                        FirstName = e.FirstName,
                                        LastName = e.LastName,
                                        MobileNumber = e.MobileNumber,
                                        DOB = e.DOB.ToString(),
                                        EnquiryDate = e.EnquiryDate.ToString(),
                                        Email = e.EmailId,
                                        PhoneNumberResidence = e.PhoneNumberResidence,
                                        PhoneNumberOffice = e.PhoneNumberOffice,
                                        Gender = e.Gender,
                                        Age = e.Age,
                                        ProgramSuggested = e.ProgramSuggested,
                                        HowDidYouKnow = e.HowDidYouKnow,
                                        Address = e.Address,
                                        Intention = e.Intention,
                                        RecievedBy = e.RecievedBy,
                                        TrailOffered = e.TrailOffered,
                                        TrailDate = e.TrailDate.ToString(),
                                     
                                    }).ToList();


                return Json(enquiryDetails, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
            
        }


        [HttpGet]
        public ActionResult EnquiryReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult EnqiuryReport(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstRptsEnquiry = (from e in db.Enquiries
                                  join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                   where e.GymId == gymid && e.BranchId == BranchId

                                   select new
                                   {
                                       membername = e.FirstName + " " + e.LastName,
                                       mobilenumber = e.MobileNumber,
                                       EnquiryDate = e.EnquiryDate.ToString(),
                                       LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                       NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                       EnqStatus = f.EnqStatus.ToString(),
                                       Notes = e.Notes.ToString()
                                   }).ToList();

         
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstRptsEnquiry.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstRptsEnquiry.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        
         
        }

        public JsonResult SearchEnquiryReportList(int? page, float pagesize,string member, string startdate, string enddate,string status, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int enquiryId = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
           
          
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            try
            {
                enquiryId = int.Parse(member);
            }
            catch (Exception ex)
            {
                memberName = member;
            }

            if (memberName != "" && memberName != null)
            {

                var enqlist = (from e in db.Enquiries
                                  join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where ((e.FirstName.ToLower().Trim() + " " + e.LastName.ToLower().Trim()).Contains(memberName.ToLower().Trim())) && e.GymId == gymid && e.BranchId == BranchId

                                  select new
                                  {
                                      membername = e.FirstName + " " + e.LastName,
                                      mobilenumber = e.MobileNumber,
                                      EnquiryDate = e.EnquiryDate.ToString(),
                                      LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                      NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                      EnqStatus = f.EnqStatus.ToString(),
                                      Notes = e.Notes.ToString()
                                  }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enqlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = enqlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            else if (enquiryId != 0)
            {

                var enqlist = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where (e.EnquiryId == enquiryId) && e.GymId == gymid && e.BranchId == BranchId

                               select new
                               {
                                   membername = e.FirstName + " " + e.LastName,
                                   mobilenumber = e.MobileNumber,
                                   EnquiryDate = e.EnquiryDate.ToString(),
                                   LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                   NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                   EnqStatus = f.EnqStatus.ToString(),
                                   Notes = e.Notes.ToString()
                               }).ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enqlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = enqlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var enqlist = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where (f.EnqStatus.Contains(status)
                               || f.LastFollowUpDate >= startDate || f.LastFollowUpDate <= endDate) && e.GymId == gymid && e.BranchId == BranchId

                               select new
                               {
                                   membername = e.FirstName + " " + e.LastName,
                                   mobilenumber = e.MobileNumber,
                                   EnquiryDate = e.EnquiryDate.ToString(),
                                   LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                   NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                   EnqStatus = f.EnqStatus.ToString(),
                                   Notes = e.Notes.ToString()
                               }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enqlist.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                return Json(new { Pages = pages, Result = enqlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        public ActionResult YearPerformance()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        public JsonResult YearPerformanceReport(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           List<YearSale> lstYearWise = new List<YearSale>();
            var getYear = (from m in db.MemberInfoes
                           join ms in db.MemberShips on m.MemberID equals ms.MemberID
                           where m.GymId == gymid && m.BranchId == BranchId
                           group new { ms } by new { Yr = (int ?)ms.StartDate.Value.Year ?? 0 } into lstYear
                           select new { Year = lstYear.Key.Yr,MembershipSale=lstYear.Sum(x=>x.ms.Amount),SoldMembership=lstYear.Select(x=>x.ms.Amount) }).ToList();
            
            foreach (var year in getYear)
            {
                if (year.Year > 0)
                {
                   
                    int soldMembership = 0;

                   soldMembership = year.SoldMembership.Count();
                   var expenseLsit = (from e in db.Expenses where e.GymId == gymid && e.BranchId == BranchId && e.ExpenseDate.Value.Year == year.Year select new { expenseAmount= e.ExpenseAmount }).ToList();
                      int enquiry=(from e in db.Enquiries where e.GymId==gymid && e.BranchId==BranchId && e.EnquiryDate.Value.Year==year.Year select e).Count();
                      int Pos = (from e in db.PurchaseOrders where e.GymId == gymid && e.BranchId == BranchId && e.OrderDate.Value.Year == year.Year select e).Count();


                      var lstMonthWise = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId == gymid && m.BranchId == BranchId && p.PaymentDate.Value.Year==year.Year
                                          select p.PaymentAmount).Sum();

                      int expense = expenseLsit.Count();
                      decimal? profit = (from e in expenseLsit select e.expenseAmount).Sum();
                      int netProfit =  Convert.ToInt32(profit) - expense;

                    //int soldMembership = year.SoldMembership.Count;

                    lstYearWise.Add(new YearSale
                    {
                        Year=year.Year.ToString(),
                        MembershipSale=year.MembershipSale.ToString(),
                        Expense=expense.ToString(),
                       NetProfit=netProfit.ToString(),
                    POSSale=Pos.ToString(),
                    Enquiry=enquiry.ToString(),
                    SoldMembership=soldMembership.ToString()
                    });


                }
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstYearWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstYearWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);



        }

        public JsonResult SearchYearPerformanceReport(int? page, float pagesize,string startYear,string endYear, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startyear = new DateTime();
            DateTime endyear = new DateTime();
            
            if (startYear != "")
            {
                startyear = DateTime.ParseExact(startYear, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (endYear != "")
            {
                endyear = DateTime.ParseExact(endYear, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            List<YearSale> lstYearWise = new List<YearSale>();
            var getYear = (from m in db.MemberInfoes
                           join ms in db.MemberShips on m.MemberID equals ms.MemberID
                           where m.GymId == gymid && m.BranchId == BranchId
                           group new { ms } by new { Yr = (int?)ms.StartDate.Value.Year ?? 0 } into lstYear
                           select new { Year = lstYear.Key.Yr, MembershipSale = lstYear.Sum(x => x.ms.Amount), SoldMembership = lstYear.Select(x => x.ms.Amount) }).ToList();

            foreach (var year in getYear)
            {
                if (year.Year > 0)
                {

                    int soldMembership = 0;

                    soldMembership = year.SoldMembership.Count();
                    var expenseLsit = (from e in db.Expenses where e.GymId == gymid && e.BranchId == BranchId && e.ExpenseDate.Value.Year == year.Year select new { expenseAmount = e.ExpenseAmount }).ToList();
                    int enquiry = (from e in db.Enquiries where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate.Value.Year == year.Year select e).Count();
                    int Pos = (from e in db.PurchaseOrders where e.GymId == gymid && e.BranchId == BranchId && e.OrderDate.Value.Year == year.Year select e).Count();


                    var lstMonthWise = (from m in db.MemberInfoes
                                        join s in db.MemberShips on m.MemberID equals s.MemberID
                                        join p in db.Payments on s.MembershipId equals p.MembershipId
                                        where m.GymId == gymid && m.BranchId == BranchId && p.PaymentDate.Value.Year == year.Year
                                        select p.PaymentAmount).Sum();

                    int expense = expenseLsit.Count();
                    decimal? profit = (from e in expenseLsit select e.expenseAmount).Sum();
                    int netProfit = Convert.ToInt32(profit) - expense;

                    //int soldMembership = year.SoldMembership.Count;

                    lstYearWise.Add(new YearSale
                    {
                        Year = year.Year.ToString(),
                        MembershipSale = year.MembershipSale.ToString(),
                        Expense = expense.ToString(),
                        NetProfit = netProfit.ToString(),
                        POSSale = Pos.ToString(),
                        Enquiry = enquiry.ToString(),
                        SoldMembership = soldMembership.ToString()
                    });


                }
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstYearWise.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstYearWise.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);



        }

        [HttpGet]
        public ActionResult TotalAttendance()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult ListTotalAttendance(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<TotalAttendance> lstAttendance = new List<TotalAttendance>();
            var lstMemberAttendancedate = (from M in db.MemberTimeSheets
                                           where M.GymId == gymid && M.BranchId == BranchId
                                           select new
                                           {
                                               dt = M.AttendenceDate,
                                           }).Distinct()
                                           .ToList();

            var lstStaffAttendancedate = (from T in db.TimeSheets
                                          where T.GymId == gymid && T.BranchId == BranchId
                                          select new
                                          {
                                              dt = T.AttendenceDate,
                                          }).Distinct()
                                           .ToList();

            List<string> lstDates = new List<string>();
            var result1 = lstMemberAttendancedate.Concat(lstStaffAttendancedate);
            foreach (var val in result1)
            {
                if(val.dt!=null)
                { 
                lstDates.Add(val.dt.ToString());
                }
            }
            if (lstDates.Count > 0)
            {
                foreach (var date in lstDates.Distinct())
                {
                    DateTime getDate=Convert.ToDateTime(date);
                      var lstMemberAttendancedate1 = (from M in db.MemberTimeSheets
                                           where M.GymId == gymid && M.BranchId == BranchId && M.AttendenceDate==getDate 
                                           select new
                                           {
                                               dt = M.IsPresent,
                                           }).ToList();
                     var lststaffAttendance = (from M in db.TimeSheets
                                           where M.GymId == gymid && M.BranchId == BranchId && M.AttendenceDate==getDate 
                                           select new
                                           {
                                               dt = M.Present,
                                           }).ToList();
                     lstAttendance.Add(new TotalAttendance() { DateYear = date, PresentMembers = lstMemberAttendancedate1.Count.ToString(), PresentStaff = lststaffAttendance.Count.ToString() });
                }
            }
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstAttendance.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MemberAttendence()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult MemberAttendanceList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                where MT.GymId == gymid && MT.BranchId == BranchId
                                group new { MT, M } by new {MT.MemberID,M.FirstName,M.LastName,M.MobileNumber,MT.IsPresent} into lstMT
                                select new
                                {
                                    MemberId = lstMT.Key.MemberID,
                                    Name = lstMT.Key.FirstName+" "+lstMT.Key.LastName,
                                    MobileNumber = lstMT.Key.MobileNumber,
                                    IsPresent = lstMT.Key.IsPresent,
                                }).ToList();
          
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchMemberAttendanceList(int? page, float pagesize,string membername,string fromdate,string todate,string status, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();


            if (fromdate != "")
            {
                startDate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if(membername!="")
            {
                if (status == "Present")
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && (M.FirstName.ToLower() + " " + M.LastName.ToLower()).Contains(membername) && MT.IsPresent=="Yes"
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else if (status == "Absent")
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && (M.FirstName.ToLower() + " " + M.LastName.ToLower()).Contains(membername) && MT.IsPresent == "No"
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && (M.FirstName.ToLower() + " " + M.LastName.ToLower()).Contains(membername)
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
            
            }
            else if (fromdate != "")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                if (status == "Present")
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && MT.AttendenceDate>=startDate &&MT.AttendenceDate<=endDate && MT.IsPresent == "Yes"
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else if (status == "Absent")
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && MT.AttendenceDate >= startDate && MT.AttendenceDate <= endDate && MT.IsPresent == "No"
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                               join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                               where MT.GymId == gymid && MT.BranchId == BranchId && MT.AttendenceDate >= startDate && MT.AttendenceDate <= endDate
                                               group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                               select new
                                               {
                                                   MemberId = lstMT.Key.MemberID,
                                                   Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                                   MobileNumber = lstMT.Key.MobileNumber,
                                                   IsPresent = lstMT.Key.IsPresent,
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                    if (pageIndex > pages)
                    {
                        if (pages > 0)
                        {
                            pageIndex = (int)pages;
                        }
                        else
                        {
                            pageIndex = 1;
                        }
                    }

                    return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var lstMemberAttendance = (from MT in db.MemberTimeSheets
                                           join M in db.MemberInfoes on MT.MemberID equals M.MemberID
                                           where MT.GymId == gymid && MT.BranchId == BranchId && MT.AttendenceDate >= startDate
                                           group new { MT, M } by new { MT.MemberID, M.FirstName, M.LastName, M.MobileNumber, MT.IsPresent } into lstMT
                                           select new
                                           {
                                               MemberId = lstMT.Key.MemberID,
                                               Name = lstMT.Key.FirstName + " " + lstMT.Key.LastName,
                                               MobileNumber = lstMT.Key.MobileNumber,
                                               IsPresent = lstMT.Key.IsPresent,
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberAttendance.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstMemberAttendance.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult TotalPaymentAmount(int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var totalpayment= (from m in db.MemberInfoes
                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                      where m.GymId == gymid && m.BranchId == BranchId

                                      select new
                                      {
                                          PaymentAmount = p.PaymentAmount,
                                      }).ToList();
            return Json(totalpayment, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult PosRepresentativeSale()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult ReportsPOSRepresentative(int? page, float pagesize, int BranchId)
        {
            string gymid =System.Web.HttpContext.Current.Session["LoggedGym"].ToString();
            var lstPos = (from P in db.PurchaseOrderHeaders
                          join Po in db.PurchaseOrderDetails on P.PurchaseId equals Po.PurchaseId
                                       where P.GymId == gymid && P.BranchId == BranchId
                          group new {P,Po} by new
                          {
                              Representative = P.Representative,
                          } into lstPosRep
                                       select new
                                       {
                                           Representative = lstPosRep.Key.Representative,
                                           TotalAmount = lstPosRep.Sum(x=>x.P.TotalAmount),
                                           Qty = lstPosRep.Sum(x => x.Po.Qty),
                                          
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstPos.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstPos.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult SearchReportsPOSRepresentative(int? page, float pagesize, string startdate, string enddate, int BranchId)
        {
            string gymid = System.Web.HttpContext.Current.Session["LoggedGym"].ToString();
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (enddate == "")
            {
                endDate = DateTime.Today.Date;
            }
            if (startdate != "")
            {
                var lstPos = (from P in db.PurchaseOrderHeaders
                              join Po in db.PurchaseOrderDetails on P.PurchaseId equals Po.PurchaseId
                              where P.GymId == gymid && P.BranchId == BranchId && P.OrderDate >= startDate && P.OrderDate <= endDate
                              group new { P, Po } by new
                              {
                                  Representative = P.Representative,
                              } into lstPosRep
                              select new
                              {
                                  Representative = lstPosRep.Key.Representative,
                                  TotalAmount = lstPosRep.Sum(x => x.P.TotalAmount),
                                  Qty = lstPosRep.Sum(x => x.Po.Qty),

                              }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstPos.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstPos.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstPos = (from P in db.PurchaseOrderHeaders
                              join Po in db.PurchaseOrderDetails on P.PurchaseId equals Po.PurchaseId
                              where P.GymId == gymid && P.BranchId == BranchId
                              group new { P, Po } by new
                              {
                                  Representative = P.Representative,
                              } into lstPosRep
                              select new
                              {
                                  Representative = lstPosRep.Key.Representative,
                                  TotalAmount = lstPosRep.Sum(x => x.P.TotalAmount),
                                  Qty = lstPosRep.Sum(x => x.Po.Qty),

                              }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstPos.Count / pagesize);
                if (pageIndex > pages)
                {
                    if (pages > 0)
                    {
                        pageIndex = (int)pages;
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }

                return Json(new { Pages = pages, Result = lstPos.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MemberReferenceReport()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult ReportsMemberReference(int? page, float pagesize, int BranchId)
        {
            int  gymid =Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"].ToString());
            List<MemberReference> lstMemberReference = new List<MemberReference>();
            var lstReference = (from P in db.MemberInfoes
                                //join M in db.MemberShips on P.MemberID equals M.MemberID
                          where P.GymId == gymid && P.BranchId == BranchId
                          group new { P} by new
                          {
                              member = P.MemberReference,
                          } into lstMemberRef
                          select new
                          {
                              Representative = lstMemberRef.Key.member,
                              MemberName = (from s in db.MemberInfoes where s.MemberID == lstMemberRef.Key.member select s.FirstName + " " + s.LastName).FirstOrDefault(),
                              //ReferedId=(from m in lstMemberRef where )
                              ////ReferenceName = lstMemberRef.Select((x => x.P.FirstName + " " + x.P.LastName + " " + x.P.MemberID + " " + " " + x.P.EnrollDate + " " + x.M.MembershipType + " " + x.M.Amount)),
                              ////Joineddate=lstMemberRef.Select(x=>x.P.EnrollDate),
                              ////Membership=lstMemberRef.Select(x=>x.M.MembershipType),
                              ////Amount =lstMemberRef.Select(x=>x.M.Amount),
                              //ReffererId=lstMemberRef.Select(x=>x.P.MemberID)
                          }).ToList();

            if (lstReference.Count > 0)
            {
                foreach (var memberref in lstReference)
                {
                    if (memberref.Representative != null && memberref.Representative > 0)
                    {
                        var lstMemberInfo = (from s in db.MemberInfoes
                                             join m in db.MemberShips on s.MemberID equals m.MemberID
                                             where s.GymId == gymid && s.BranchId == BranchId && s.MemberReference == memberref.Representative
                                             select new { 
                                                MemberId=s.MemberID,
                                                MemberName=s.FirstName+" "+s.LastName,
                                                JoinedDate=s.EnrollDate.ToString(),
                                                Membership=m.MembershipType,
                                                Amount=m.Amount
                                             }
                                               );
                var lstmember= lstMemberInfo.GroupBy(x => x.MemberId).Select(x => x.FirstOrDefault()).ToList();

                        List<MemberReferenceList> lstMemberreferenecelist=new List<MemberReferenceList>();
                        foreach(var member in lstmember)
                        {
                           lstMemberreferenecelist.Add(new MemberReferenceList()
                               {
                                   MemberId=member.MemberId.ToString(),
                                   MemberName=member.MemberName,
                                   Membership=member.Membership,
                                   Amount=member.Amount,
                                   JoinedDate = member.JoinedDate,
                              } );
                        }


                lstMemberReference.Add(new MemberReference()
                {
                    MemberName = memberref.MemberName,
                    MemberId = memberref.Representative.ToString(),
                    ReferenceName =lstMemberreferenecelist
                    //ReferenceName = memberref.ReferenceName.Distinct().ToList(),
                    ////JoinedDate = memberref.Joineddate.Distinct().ToList(),
                    ////Membership = memberref.Membership.Distinct().ToList(),
                    ////Amount = memberref.Amount.Distinct().ToList(),
                    //ReferenceId = memberref.ReffererId.ElementAt(0).ToString(),

                });

                        //lstMemberReference = (from meber in db.MemberInfoes
                        //                      where meber.MemberReference == memberref.Representative
                        //                      select new lstMemberReference<MemberReference>
                        //                      {


                        //                      });
                    }
                }


            
            }

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMemberReference.Count / pagesize);
            if (pageIndex > pages)
            {
                if (pages > 0)
                {
                    pageIndex = (int)pages;
                }
                else
                {
                    pageIndex = 1;
                }
            }

            return Json(new { Pages = pages, Result = lstMemberReference.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        //Generating Pdf and Excel reports for all tables 

         [HttpPost]
        public JsonResult GenarateExcelReportsDueMemberList(string membername, string startdate, string todate, int branchid)
        {
            DataTable dt=new DataTable();
            dt = DueMembershipTable(membername, startdate, todate, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Due Memberslist");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

          [HttpPost]
         public JsonResult GenaratePdfReportsDueMemberList(string membername, string startdate, string todate, int branchid)
         {
             DataTable dt = new DataTable();
             dt = DueMembershipTable(membername, startdate, todate, branchid);
             if (dt.Rows.Count > 0)
             {
                 var result = GeneratePdf(dt);
                 return result;
             }
             return Json("", JsonRequestBehavior.AllowGet);
         }

        public DataTable DueMembershipTable(string membername, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            int memberId = 0;
            string memberName = "";
            int membershipId = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("MemberID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("Mobile Number");
            dt.Columns.Add("Dob");
            dt.Columns.Add("Email");
            dt.Columns.Add("Enroll Date");
            dt.Columns.Add("Package");
            dt.Columns.Add("Start Date");
            dt.Columns.Add("End Date");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Payment Amount");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }
            if (memberName != "" && memberName != null)
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where ((mi.FirstName + " " + mi.LastName).Contains(memberName) || ms.MembershipId == membershipId
                                  || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();

                foreach (var item in memberlist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
    item.EnrollDate, item.Package, item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                }

                //if (dt.Rows.Count > 0)
                //{
                //    var result = ExportListUsingEPPlus(dt, "Report Due Memberslist");
                //    return result;

                //}
            }
            else if (memberId != 0)
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (mi.MemberID == memberId || ms.MembershipId == membershipId
                                  || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                foreach (var item in memberlist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
    item.EnrollDate, item.Package, item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                }

                //if (dt.Rows.Count > 0)
                //{
                //    var result = ExportListUsingEPPlus(dt, "Report Due Memberslist");
                //    return result;

                //}
            }
            else
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                 where mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      Address = mi.MemberAddress.ToString(),
                                      MobileNumber = mi.MobileNumber,
                                      Dob = mi.Dob.ToString(),
                                      Email = mi.Email,
                                      EnrollDate = mi.EnrollDate.ToString(),
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount

                                  }).ToList();
                foreach (var item in memberlist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
    item.EnrollDate, item.Package, item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                }

                //if (dt.Rows.Count > 0)
                //{
                //    var result = ExportListUsingEPPlus(dt, "Report Due Memberslist");
                //    return result;

                //}
            }



            return dt;
        }

        //Download reports for Transffered membership
        [HttpPost]
        public JsonResult GenarateExcelReportsTransferredMemberList(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            dt = TransferedMembershipTable(membername, startdate, enddate, BranchId);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Transferred Memberslist");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsTransferredMemberList(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            dt = TransferedMembershipTable(membername, startdate, enddate, BranchId);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable TransferedMembershipTable(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            long phoneNumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Name");
            dt.Columns.Add("Mobile Number");
            dt.Columns.Add("Package");
            dt.Columns.Add("Start Date");
            dt.Columns.Add("End Date");
            dt.Columns.Add("Months");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Payment Amount");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                phoneNumber = long.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (phoneNumber > 0)
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && m.MobileNumber == phoneNumber && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package, item.StartDate, item.EndDate, item.Months, item.Amount, item.PaymentAmount);

                }
            }
            else if (memberName != "")
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(memberName.ToLower()) && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package, item.StartDate, item.EndDate, item.Months, item.Amount, item.PaymentAmount);

                }
            }
            else if (startdate != "")
            {
                if (enddate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= startDate && s.EndDate <= endDate && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package, item.StartDate, item.EndDate, item.Months, item.Amount, item.PaymentAmount);

                }

            }
            else
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && s.Status == "Transfered"

                                   select new
                                   {
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       EndDate = s.EndDate.ToString(),
                                       Months = s.Months,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount
                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package, item.StartDate, item.EndDate, item.Months, item.Amount, item.PaymentAmount);

                }
            }


            return dt;
        }

        [HttpPost]
        public JsonResult GenarateExcelReportsMeasurementList(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            dt = MeasurementTable(membername, startdate, enddate, BranchId);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Measurements");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsMeasurementList(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            dt = MeasurementTable(membername, startdate, enddate, BranchId);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable MeasurementTable(string membername, string startdate, string enddate, int BranchId)
        {
            DataTable dt = new DataTable();
            long phoneNumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Member Id");
            dt.Columns.Add("Member Name");
            dt.Columns.Add("Mobile No");
            dt.Columns.Add("Last Measurement Date");
            dt.Columns.Add("Next Measurement Date");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (enddate != "")
            {
                endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                phoneNumber = long.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (phoneNumber > 0)
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID
                                   where m.GymId == gymid && m.BranchId == BranchId && m.MobileNumber == phoneNumber
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName).ToList(),
                                       LastName = lstMeasurement.Select(x => x.m.LastName).ToList(),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber).ToList(),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate).ToList(),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate).ToList(),

                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName[0] + " " + item.LastName[0], item.MobileNumber[0], item.NextMeasurementDate[0], item.MeasurementDate[0]);

                }
            }
            else if (memberName != "")
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId && (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(memberName.ToLower())
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName).ToList(),
                                       LastName = lstMeasurement.Select(x => x.m.LastName).ToList(),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber).ToList(),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate).ToList(),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate).ToList(),

                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName[0] + " " + item.LastName[0], item.MobileNumber[0], item.NextMeasurementDate[0], item.MeasurementDate[0]);

                }
            }
            else if (startdate != "")
            {
                if (enddate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId && me.MeasurementDate >= startDate && me.MeasurementDate <= endDate
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       FirstName = lstMeasurement.Select(x => x.m.FirstName).ToList(),
                                       LastName = lstMeasurement.Select(x => x.m.LastName).ToList(),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber).ToList(),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate).ToList(),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate).ToList(),

                                   }).ToList();

                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName[0] + " " + item.LastName[0], item.MobileNumber[0], item.NextMeasurementDate[0], item.MeasurementDate[0]);

                }

            }
            else
            {
                var memberslist = (from m in db.MemberInfoes
                                   join me in db.Measurements on m.MemberID equals me.MemberID

                                   where m.GymId == gymid && m.BranchId == BranchId
                                   group new { m, me } by new
                                   {
                                       MemberID = m.MemberID,

                                   } into lstMeasurement
                                   select new
                                   {
                                       MemberID = lstMeasurement.Key.MemberID,
                                       //FirstName = lstMeasurement.Select(x => x.m.FirstName.ToString()),
                                       FirstName = lstMeasurement.Select(x=>x.m.FirstName).ToList(),
                                       LastName = lstMeasurement.Select(x => x.m.LastName).ToList(),
                                       MobileNumber = lstMeasurement.Select(x => x.m.MobileNumber).ToList(),
                                       NextMeasurementDate = lstMeasurement.Select(x => x.me.NextMeasurementDate).ToList(),
                                       MeasurementDate = lstMeasurement.Select(x => x.me.MeasurementDate).ToList(),

                                   }).ToList();


                foreach (var item in memberslist)
                {
                    dt.Rows.Add(item.MemberID, item.FirstName[0] + " " + item.LastName[0], item.MobileNumber[0],
                        item.NextMeasurementDate[0]!=null?item.NextMeasurementDate[0].Value.Day + "/" + item.NextMeasurementDate[0].Value.Month + "/" + item.NextMeasurementDate[0].Value.Year:"",
                        item.MeasurementDate[0]!=null?item.MeasurementDate[0].Value.Day + "/" + item.MeasurementDate[0].Value.Month + "/" + item.MeasurementDate[0].Value.Year:"");
                   

                }
            }


            return dt;
        }

        public JsonResult GenarateExcelReportsExpiredMembershipList(string membername, string membership, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            dt = ExpiredMembershipTable(membername, membership, startdate, todate, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Expired Membership");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsExpiredMembershipList(string membername, string membership, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            dt = ExpiredMembershipTable(membername, membership, startdate, todate, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable ExpiredMembershipTable(string membername, string membership, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            int memberId = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Customer Name");
            dt.Columns.Add("Mobile No");
            dt.Columns.Add("Membership Name");
            dt.Columns.Add("Start Date");
            dt.Columns.Add("End Date");
            dt.Columns.Add("Month");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (memberId > 0)
            {

                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where mi.MemberID == memberId && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate,
                                      EndDate = ms.EndDate,

                                  }).ToList();

                foreach (var item in memberlist)
                {
                    var month = "";
                    int Month = 0;
                    if (item.Package != "" || item.Package != null)
                    {
                        string[] package = item.Package.Split(' ');
                        try
                        {
                            Month = Convert.ToInt32(package[1]);
                        }
                        catch (Exception ex)
                        {
                            Month = Convert.ToInt32(package[2]);
                        }

                        if (Month > 1)
                        {
                            month = Month + " " + "Months";
                        }
                        else if (Month == 1)
                        {
                            month = Month + " " + "Month";
                        }

                        dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package,
                                       item.StartDate != null ? item.StartDate.Value.Day + "/" + item.StartDate.Value.Month + "/" + item.StartDate.Value.Year : "",
                                         item.EndDate != null ? item.EndDate.Value.Day + "/" + item.EndDate.Value.Month + "/" + item.EndDate.Value.Year : "", month);
                    }


                }
            }
            else if (memberName != "")
            {

                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (mi.FirstName + " " + mi.LastName).Contains(memberName)  && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate,
                                      EndDate = ms.EndDate,
                                  }).ToList();

                foreach (var item in memberlist)
                {
                    var month = "";
                    int Month = 0;
                    if (item.Package != "" || item.Package != null)
                    {
                        string[] package = item.Package.Split(' ');
                        try
                        {
                            Month = Convert.ToInt32(package[1]);
                        }
                        catch (Exception ex)
                        {
                            Month = Convert.ToInt32(package[2]);
                        }

                        if (Month > 1)
                        {
                            month = Month + " " + "Months";
                        }
                        else if (Month == 1)
                        {
                            month = Month + " " + "Month";
                        }

                        dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package,
                                       item.StartDate != null ? item.StartDate.Value.Day + "/" + item.StartDate.Value.Month + "/" + item.StartDate.Value.Year : "",
                                         item.EndDate != null ? item.EndDate.Value.Day + "/" + item.EndDate.Value.Month + "/" + item.EndDate.Value.Year : "", month);
                    }


                }
            }
            else if (startdate != "")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (ms.MembershipType.Contains(membership)||(ms.StartDate>=startDate && ms.EndDate<=endDate)) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate,
                                      EndDate = ms.EndDate,
                                  }).ToList();


                foreach (var item in memberlist)
                {
                    var month = "";
                    int Month = 0;
                    if (item.Package != "" || item.Package != null)
                    {
                        string[] package = item.Package.Split(' ');
                        try
                        {
                            Month = Convert.ToInt32(package[1]);
                        }
                        catch (Exception ex)
                        {
                            Month = Convert.ToInt32(package[2]);
                        }

                        if (Month > 1)
                        {
                            month = Month + " " + "Months";
                        }
                        else if (Month == 1)
                        {
                            month = Month + " " + "Month";
                        }

                        dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package,
                                       item.StartDate != null ? item.StartDate.Value.Day + "/" + item.StartDate.Value.Month + "/" + item.StartDate.Value.Year : "",
                                         item.EndDate != null ? item.EndDate.Value.Day + "/" + item.EndDate.Value.Month + "/" + item.EndDate.Value.Year : "", month);
                    }


                }

            }
            else
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where  mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate,
                                      EndDate = ms.EndDate,
                                  }).ToList();

 foreach (var item in memberlist)
                {
     var month="";
      int Month=0;
     if (item.Package != "" || item.Package != null)
     {
         string[] package = item.Package.Split(' ');
         try
         {
           Month  = Convert.ToInt32(package[1]);
         }
         catch (Exception ex)
         {
             Month = Convert.ToInt32(package[2]);
         }

         if (Month > 1)
         {
             month = Month + " " + "Months";
         }
         else if(Month==1)
         {
             month = Month + " " + "Month";
         }

         dt.Rows.Add(item.FirstName, item.MobileNumber, item.Package,
                        item.StartDate != null ? item.StartDate.Value.Day + "/" + item.StartDate.Value.Month + "/" + item.StartDate.Value.Year : "",
                          item.EndDate != null ? item.EndDate.Value.Day + "/" + item.EndDate.Value.Month + "/" + item.EndDate.Value.Year : "",month);
     }
     

                }
            }


            return dt;
        }

        public JsonResult GenarateExcelReportsMembershipPayments(string membername, string startdate, string todate, string membership, string category, string paymentMode, int branchid)
        {
            DataTable dt = new DataTable();
            dt = MembershipPaymentTable(membername, startdate, todate, membership, category, paymentMode, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Membership Payment");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsMembershipPayments(string membername, string startdate, string todate, string membership, string category, string paymentMode, int branchid)
        {
            DataTable dt = new DataTable();
            dt = MembershipPaymentTable(membername, startdate, todate, membership,category, paymentMode, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable MembershipPaymentTable(string membername, string startdate, string todate, string membership, string category, string paymentMode, int branchid)
        {
            DataTable dt = new DataTable();
            int memberId = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Receipt No");
            dt.Columns.Add("Member Name");
            dt.Columns.Add("Mobile No");
            dt.Columns.Add("Membership Name");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Payment Date");
            dt.Columns.Add("Paid Amount");
            dt.Columns.Add("Mode of Payment");
            dt.Columns.Add("Due Amount");
            dt.Columns.Add("Due Date");
            dt.Columns.Add("Remarks");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }

            if (memberId > 0)
            {

                var memberspaymentlist = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId == gymid && m.BranchId == branchid && m.MemberID == memberId
                                          select new
                                          {
                                              FirstName = m.FirstName + " " + m.LastName,
                                              MobileNumber = m.MobileNumber,
                                              Package = s.MembershipType,
                                              Months = s.Months,
                                              RecieptNumber = p.RecieptNumber,
                                              Amount = s.Amount,
                                              PaymentDate = p.PaymentDate,
                                              PaymentAmount = p.PaymentAmount,
                                              PaymentType = p.PaymentType,
                                              PaymentDueDate = p.PaymentDueDate,
                                              Note = s.Note.ToString()


                                          }).ToList();
                
                 int  pendingamt =0;
                 foreach (var item in memberspaymentlist)
                 {
                    if (item.Amount != 0) {
                        pendingamt =Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    }

                        dt.Rows.Add(item.RecieptNumber,item.FirstName, item.MobileNumber, item.Package,item.Amount,
                                       item.PaymentDate != null ? item.PaymentDate.Value.Day + "/" + item.PaymentDate.Value.Month + "/" + item.PaymentDate.Value.Year : "",
                                       item.PaymentAmount,item.PaymentType,pendingamt,
                                         item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "",item.Note);
                }
            }
            else if (memberName != "")
            {


                var memberspaymentlist = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId == gymid && m.BranchId == branchid && (m.FirstName.ToLower() + " " + m.LastName.ToLower()).Contains(membername.ToLower())
                                          select new
                                          {
                                              FirstName = m.FirstName + " " + m.LastName,
                                              MobileNumber = m.MobileNumber,
                                              Package = s.MembershipType,
                                              Months = s.Months,
                                              RecieptNumber = p.RecieptNumber,
                                              Amount = s.Amount,
                                              PaymentDate = p.PaymentDate,
                                              PaymentAmount = p.PaymentAmount,
                                              PaymentType = p.PaymentType,
                                              PaymentDueDate = p.PaymentDueDate,
                                              Note = s.Note.ToString()


                                          }).ToList();

                int pendingamt = 0;
                foreach (var item in memberspaymentlist)
                {
                    if (item.Amount != 0)
                    {
                        pendingamt = Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    }

                    dt.Rows.Add(item.RecieptNumber, item.FirstName, item.MobileNumber, item.Package, item.Amount,
                                   item.PaymentDate != null ? item.PaymentDate.Value.Day + "/" + item.PaymentDate.Value.Month + "/" + item.PaymentDate.Value.Year : "",
                                   item.PaymentAmount, item.PaymentType, pendingamt,
                                     item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Note);
                }
            }
            else if (startdate != "")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var memberspaymentlist = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId == gymid && m.BranchId == branchid && (p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                                          || s.MembershipType.Contains(membership) || p.PaymentType.ToLower().Contains(paymentMode.ToLower())
                                          select new
                                          {
                                              FirstName = m.FirstName + " " + m.LastName,
                                              MobileNumber = m.MobileNumber,
                                              Package = s.MembershipType,
                                              Months = s.Months,
                                              RecieptNumber = p.RecieptNumber,
                                              Amount = s.Amount,
                                              PaymentDate = p.PaymentDate,
                                              PaymentAmount = p.PaymentAmount,
                                              PaymentType = p.PaymentType,
                                              PaymentDueDate = p.PaymentDueDate,
                                              Note = s.Note.ToString()


                                          }).ToList();

                int pendingamt = 0;
                foreach (var item in memberspaymentlist)
                {
                    if (item.Amount != 0)
                    {
                        pendingamt = Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    }

                    dt.Rows.Add(item.RecieptNumber, item.FirstName, item.MobileNumber, item.Package, item.Amount,
                                   item.PaymentDate != null ? item.PaymentDate.Value.Day + "/" + item.PaymentDate.Value.Month + "/" + item.PaymentDate.Value.Year : "",
                                   item.PaymentAmount, item.PaymentType, pendingamt,
                                     item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Note);
                }

            }
            else
            {
                var memberspaymentlist = (from m in db.MemberInfoes
                                          join s in db.MemberShips on m.MemberID equals s.MemberID
                                          join p in db.Payments on s.MembershipId equals p.MembershipId
                                          where m.GymId == gymid && m.BranchId == branchid

                                          select new
                                          {
                                              FirstName = m.FirstName + " " + m.LastName,
                                              MobileNumber = m.MobileNumber,
                                              Package = s.MembershipType,
                                              Months = s.Months,
                                              RecieptNumber = p.RecieptNumber,
                                              Amount = s.Amount,
                                              PaymentDate = p.PaymentDate,
                                              PaymentAmount = p.PaymentAmount,
                                              PaymentType = p.PaymentType,
                                              PaymentDueDate = p.PaymentDueDate,
                                              Note = s.Note.ToString()


                                          }).ToList();
                int pendingamt = 0;
                foreach (var item in memberspaymentlist)
                {
                    if (item.Amount != 0)
                    {
                        pendingamt = Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    }

                    dt.Rows.Add(item.RecieptNumber, item.FirstName, item.MobileNumber, item.Package, item.Amount,
                                   item.PaymentDate != null ? item.PaymentDate.Value.Day + "/" + item.PaymentDate.Value.Month + "/" + item.PaymentDate.Value.Year : "",
                                   item.PaymentAmount, item.PaymentType, pendingamt,
                                     item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Note);
                }
            }


            return dt;
        }

        public JsonResult GenarateExcelReportsCategoryWiseSale(string startDate, string endDate)
        {
            DataTable dt = new DataTable();
            dt = CategoryWiseTable(startDate, endDate);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Category Wise sale");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsCategoryWiseSale(string startDate, string endDate)
        {
            DataTable dt = new DataTable();
            dt = CategoryWiseTable(startDate, endDate);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable CategoryWiseTable(string startdate, string todate)
        {
            DataTable dt = new DataTable();
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Category");
            dt.Columns.Add("Total Sale");
            dt.Columns.Add("Sale Amount");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
           
            if (startdate != "")
            {
                 if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var lstcategoerywise = (from mi in db.MemberShips
                                        where mi.StartDate >= startDate && mi.EndDate <= endDate

                                        select new
                                        {
                                            MembershipType = mi.MembershipType,
                                            StartDate = mi.StartDate,
                                            EndDate = mi.EndDate,
                                            Amount = mi.Amount
                                        }).ToList();

                var totalSale = 0;
                var salAmount = 0;
                foreach (var item in lstcategoerywise)
                {
                    totalSale = totalSale + 1;
                salAmount = salAmount + Convert.ToInt32(item.Amount);

                  
                }
                dt.Rows.Add("Gym", totalSale, salAmount);
                         }
            else
            {
                var lstcategoerywise = (from mi in db.MemberShips
                                        select new
                                        {
                                            MembershipType = mi.MembershipType,
                                            StartDate = mi.StartDate,
                                            EndDate = mi.EndDate,
                                            Amount = mi.Amount
                                        }).ToList();
                var totalSale = 0;
                var salAmount = 0;
                foreach (var item in lstcategoerywise)
                {
                    totalSale = totalSale + 1;
                    salAmount = salAmount + Convert.ToInt32(item.Amount);
                }
                dt.Rows.Add("Gym", totalSale, salAmount);
            }
           

            return dt;
        }

        public JsonResult GenarateExcelReportsDuepayments(string membername, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            dt = DuepaymentsTable(membername, startdate, todate, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Due Payments");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsDuepayments(string membername, string startdate, string todate, int branchid)
        {
            DataTable dt = new DataTable();
            dt = DuepaymentsTable(membername, startdate, todate, branchid);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable DuepaymentsTable(string membername, string startdate, string todate, int branchid)
        {
            int memberId = 0;
            string memberName = "";
            DataTable dt = new DataTable();
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Member Id");
            dt.Columns.Add("Member Name");
            dt.Columns.Add("Mobile No.");
            dt.Columns.Add("Membership Name");
            dt.Columns.Add("Membership Date");
            dt.Columns.Add("Payment Due Date");
            dt.Columns.Add("Membership Amount");
            dt.Columns.Add("Paid Amount");
            dt.Columns.Add("Pending Amount");

            if (startdate != "")
            {
                startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "")
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                memberId = int.Parse(membername);
            }
            catch (Exception ex)
            {
                memberName = membername;
            }



            if (memberName != "" && memberName != null)
            {

                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where ((m.FirstName + " " + m.LastName).Contains(memberName)
                                  || (s.StartDate >= startDate && s.EndDate == endDate)) && m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate,
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate,
                                      EndDate = s.EndDate,
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate

                                  }).ToList();

                foreach (var item in memberlist)
                {
                   int pendingAmount=0;
                    pendingAmount=Convert.ToInt32(item.Amount)-Convert.ToInt32(item.PaymentAmount);
                    dt.Rows.Add(item.MemberID, item.FirstName, item.MobileNumber,item.Package,
                        item.EnrollDate != null ? item.EnrollDate.Value.Day + "/" + item.EnrollDate.Value.Month + "/" + item.EnrollDate.Value.Year : "",
                        item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Amount,
                        item.PaymentAmount, pendingAmount
                        
                       );
                }
              
            }

            else if (memberId != 0)
            {

                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where (m.MemberID == memberId || (s.StartDate >= startDate && s.EndDate <= endDate)) && m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate,
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate,
                                      EndDate = s.EndDate,
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate

                                  }).ToList();


                foreach (var item in memberlist)
                {
                    int pendingAmount = 0;
                    pendingAmount = Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    dt.Rows.Add(item.MemberID, item.FirstName, item.MobileNumber, item.Package,
                        item.EnrollDate != null ? item.EnrollDate.Value.Day + "/" + item.EnrollDate.Value.Month + "/" + item.EnrollDate.Value.Year : "",
                        item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Amount,
                        item.PaymentAmount, pendingAmount

                       );
                }
            }
            else
            {
                var memberlist = (from m in db.MemberInfoes
                                  join s in db.MemberShips on m.MemberID equals s.MemberID
                                  join p in db.Payments on s.MembershipId equals p.MembershipId
                                  where  m.GymId == gymid && m.BranchId == branchid

                                  select new
                                  {
                                      MemberID = m.MemberID,
                                      FirstName = m.FirstName + " " + m.LastName,
                                      MobileNumber = m.MobileNumber,
                                      EnrollDate = m.EnrollDate,
                                      Package = s.MembershipType,
                                      StartDate = s.StartDate,
                                      EndDate = s.EndDate,
                                      Amount = s.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      PaymentDueDate = p.PaymentDueDate

                                  }).ToList();

                foreach (var item in memberlist)
                {
                    int pendingAmount = 0;
                    pendingAmount = Convert.ToInt32(item.Amount) - Convert.ToInt32(item.PaymentAmount);
                    dt.Rows.Add(item.MemberID, item.FirstName, item.MobileNumber, item.Package,
                        item.EnrollDate != null ? item.EnrollDate.Value.Day + "/" + item.EnrollDate.Value.Month + "/" + item.EnrollDate.Value.Year : "",
                        item.PaymentDueDate != null ? item.PaymentDueDate.Value.Day + "/" + item.PaymentDueDate.Value.Month + "/" + item.PaymentDueDate.Value.Year : "", item.Amount,
                        item.PaymentAmount, pendingAmount

                       );
                }
            }

            return dt;
        }

        public JsonResult GenarateExcelReportsMonthWisePayment( int branchid)
        {
            DataTable dt = new DataTable();
            dt = MonthWisePaymentTable(branchid);
            if (dt.Rows.Count > 0)
            {
                var result = ExportListUsingEPPlus(dt, "Report Month Wise Payments");
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GenaratePdfReportsMonthWisePayment( int branchid)
        {
            DataTable dt = new DataTable();
            dt = MonthWisePaymentTable(branchid);
            if (dt.Rows.Count > 0)
            {
                var result = GeneratePdf(dt);
                return result;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public DataTable MonthWisePaymentTable(int branchid)
        {
           
            DataTable dt = new DataTable();
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            dt.Columns.Add("Month-Year");
            dt.Columns.Add("Cash");
            dt.Columns.Add("Cheque");
            dt.Columns.Add("Card");
            dt.Columns.Add("BankTransfer");
            dt.Columns.Add("Total");
           
            //if (startdate != "")
            //{
            //    startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //}
            //if (todate != "")
            //{
            //    endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            //}

            var lstMonthWise = (from m in db.MemberInfoes
                                join s in db.MemberShips on m.MemberID equals s.MemberID
                                join p in db.Payments on s.MembershipId equals p.MembershipId
                                where m.GymId == gymid && m.BranchId == branchid
                                group p by new { month = p.PaymentDate.Value.Month, year = p.PaymentDate.Value.Year } into lstSale
                                select new
                                {
                                    dt = lstSale.Key.month + " " + lstSale.Key.year,
                                    PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                    PaymentType = lstSale.Select(x => new {x.PaymentType,x.PaymentAmount })
                                }).ToList();

                foreach (var item in lstMonthWise)
                {
                    string monthYear = item.dt.ToString();
                    if (monthYear != " ")
                    { 
                    
                   
                    string[] mY = monthYear.Split(' ');
                    DateTime tdate = new DateTime();
                    tdate = DateTime.Today;
                    string thisYear = tdate.Year.ToString();
                    string month = mY[0];
                    string year = mY[1];
                    if (year.ToLower().Equals(thisYear))
                    {
                        month = getMonth(month);
                        var totalAmount = 0;
                        var cash = 0;
                        var cheque = 0;
                        var card = 0;
                        var bankTransfer = 0;
                        foreach (var paymentType in item.PaymentType)
                        {
                            if (paymentType.PaymentType == "Cash")
                            {
                                cash = cash + Convert.ToInt32(paymentType.PaymentAmount);
                                totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                            }
                            else if (paymentType.PaymentType == "Cheque")
                            {
                                cheque = cheque + Convert.ToInt32(paymentType.PaymentAmount);
                                totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                            }
                            else if (paymentType.PaymentType == "Card")
                            {
                                card = card + Convert.ToInt32(paymentType.PaymentAmount);
                                totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                            }
                            else if (paymentType.PaymentType == "Bank Transfer")
                            {
                                bankTransfer = bankTransfer + Convert.ToInt32(paymentType.PaymentAmount);
                                totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                            }

                        }



                        dt.Rows.Add(month + '-' + year, cash, cheque, card, bankTransfer, totalAmount);
                    }
                    }
            }

            return dt;
        }

        private string getMonth(string month)
{
    string day = "";
    switch (month) {
       
        case "1":
            day = "January";
            break;
        case "2":
            day = "February";
            break;
        case "3":
            day = "March";
            break;
        case "4":
            day = "April";
            break;
        case "5":
            day = "May";
            break;
        case "6":
            day = "June";
            break;
        case "7":
            day = "July";
            break;
        case "8":
            day = "August";
            break;
        case "9":
            day = "September";
            break;
        case "10":
            day = "October";
            break;
        case "11":
            day = "November";
            break;
        case "12":
            day = "December";
            break;
    }
    return day;
}

public JsonResult GenarateExcelReportsDayWisePayment(int branchid)
{
    DataTable dt = new DataTable();
    dt = DayWisePaymentTable(branchid);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Day Wise Payments");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsDayWisePayment(int branchid)
{
    DataTable dt = new DataTable();
    dt = DayWisePaymentTable(branchid);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable DayWisePaymentTable(int branchid)
{

    DataTable dt = new DataTable();
    //DateTime startDate = new DateTime();
    //DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Month-Year");
    dt.Columns.Add("Cash");
    dt.Columns.Add("Cheque");
    dt.Columns.Add("Card");
    dt.Columns.Add("BankTransfer");
    dt.Columns.Add("Total");

    //if (startdate != "")
    //{
    //    startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    //}
    //if (todate != "")
    //{
    //    endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    //}

    var lstDayWise = (from m in db.MemberInfoes
                      join s in db.MemberShips on m.MemberID equals s.MemberID
                      join p in db.Payments on s.MembershipId equals p.MembershipId
                      where m.GymId == gymid && m.BranchId == branchid
                      group p by new { day = p.PaymentDate } into lstSale
                      //,paymentType=p.PaymentType
                      select new
                      {

                          dt = lstSale.Key.day.ToString(),
                          PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                          PaymentType = lstSale.Select(x => new { x.PaymentType, x.PaymentAmount })
                      }).ToList();

    foreach (var item in lstDayWise)
    {
                var totalAmount = 0;
                var cash = 0;
                var cheque = 0;
                var card = 0;
                var bankTransfer = 0;
                foreach (var paymentType in item.PaymentType)
                {
                    if (paymentType.PaymentType == "Cash")
                    {
                        cash = cash + Convert.ToInt32(paymentType.PaymentAmount);
                        totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                    }
                    else if (paymentType.PaymentType == "Cheque")
                    {
                        cheque = cheque + Convert.ToInt32(paymentType.PaymentAmount);
                        totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                    }
                    else if (paymentType.PaymentType == "Card")
                    {
                        card = card + Convert.ToInt32(paymentType.PaymentAmount);
                        totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                    }
                    else if (paymentType.PaymentType == "Bank Transfer")
                    {
                        bankTransfer = bankTransfer + Convert.ToInt32(paymentType.PaymentAmount);
                        totalAmount = totalAmount + Convert.ToInt32(paymentType.PaymentAmount);
                    }

                }
                dt.Rows.Add( item.dt, cash, cheque, card, bankTransfer, totalAmount);
            
        
    }

    return dt;
}

public JsonResult GenarateExcelReportsProfitLoss(int branchid)
{
    DataTable dt = new DataTable();
    dt = ProfitLossTable(branchid);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Profit Loss");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsProfitLoss(int branchid)
{
    DataTable dt = new DataTable();
    dt = ProfitLossTable(branchid);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable ProfitLossTable(int branchid)
{

    DataTable dt = new DataTable();
    //DateTime startDate = new DateTime();
    //DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    List<AddProfitLoss> lstProfit = new List<AddProfitLoss>();
    dt.Columns.Add("Month-Year");
    dt.Columns.Add("Sales");
    dt.Columns.Add("Expense");
    dt.Columns.Add("Total");


    var lstExpense = (from e in db.Expenses
                      where e.GymId == gymid && e.BranchId == branchid
                      group e by new { day = e.ExpenseDate.Value.Month + " " + e.ExpenseDate.Value.Year } into lstSale
                      select new
                      {
                          dt = lstSale.Key.day,
                          expensemount = lstSale.Sum(x => x.ExpenseAmount),
                      }).ToList();

    var lstMonthWise = (from m in db.MemberInfoes
                        join s in db.MemberShips on m.MemberID equals s.MemberID
                        join p in db.Payments on s.MembershipId equals p.MembershipId
                        where m.GymId == gymid && m.BranchId == branchid
                        group p by new { day = p.PaymentDate.Value.Month + " " + p.PaymentDate.Value.Year } into lstSale
                        select new
                        {
                            dt = lstSale.Key.day,
                            PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                        }).ToList();

    int thisYear = DateTime.Today.Year;

    List<string> dateYear = new List<string>();
    dateYear.Add("1" + " " + thisYear);
    dateYear.Add("2" + " " + thisYear);
    dateYear.Add("3" + " " + thisYear);
    dateYear.Add("4" + " " + thisYear);
    dateYear.Add("5" + " " + thisYear);
    dateYear.Add("6" + " " + thisYear);
    dateYear.Add("7" + " " + thisYear);
    dateYear.Add("8" + " " + thisYear);
    dateYear.Add("9" + " " + thisYear);
    dateYear.Add("10" + " " + thisYear);
    dateYear.Add("11" + " " + thisYear);
    dateYear.Add("12" + " " + thisYear);

    foreach (string date in dateYear)
    {
        var payment = (from p in lstMonthWise
                       where p.dt == date
                       select new { totalpayment = p.PaymentAmount }).ToList();
        var expense = (from e in lstExpense
                       where e.dt == date
                       select new { totalExpense = e.expensemount }).ToList();

        lstProfit.Add(new AddProfitLoss() { DateYear = date, Payment = payment.Count > 0 ? payment[0].totalpayment.ToString() : "0", Expense = expense.Count > 0 ? expense[0].totalExpense.ToString() : "0" });

    }


    foreach (var item in lstProfit)
    {
        string monthYear = item.DateYear.ToString();
        if (monthYear != " ")
        {


            string[] mY = monthYear.Split(' ');
            DateTime tdate = new DateTime();
            tdate = DateTime.Today;
            string currentYear = tdate.Year.ToString();
            string month = mY[0];
            string year = mY[1];
            if (year.ToLower().Equals(currentYear))
            {
                month = getMonth(month);
                decimal payment = Convert.ToDecimal(item.Payment);
                decimal expense = Convert.ToDecimal(item.Expense);
                var total = payment - expense;
                dt.Rows.Add(month + '-' + year, item.Payment,item.Expense, total);
            }
        }
    }

    return dt;
}

public JsonResult GenarateExcelReportsMembershipwiseSale(string startdate, string todate, int branchid)
{
    DataTable dt = new DataTable();
    dt = MembershipwiseSaleTable(startdate, todate, branchid);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report MembershipWise Sale");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsMembershipwiseSale(string startdate, string todate, int branchid)
{
    DataTable dt = new DataTable();
    dt = MembershipwiseSaleTable(startdate, todate, branchid);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable MembershipwiseSaleTable(string startdate, string todate, int branchid)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Membership Name");
    dt.Columns.Add("Total Sale");
    dt.Columns.Add("Sale Amount");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (todate != "")
    {
        endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
     if (startdate != "")
    {
        if (todate == "")
        {
            endDate = DateTime.Today.Date;
        }
        var lstMemberwiseSale = (from m in db.MemberInfoes
                                 join s in db.MemberShips on m.MemberID equals s.MemberID
                                 join p in db.Payments on s.MembershipId equals p.MembershipId
                                 where m.GymId == gymid && m.BranchId == branchid && (p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                                 group s by s.MembershipType into lstPayments
                                 select new
                                 {
                                     Package = lstPayments.Key,
                                     Amount = lstPayments.Sum(x => x.Amount),
                                     ToatlSale = lstPayments.Count()

                                 }).ToList();
        foreach (var item in lstMemberwiseSale)
        {
            dt.Rows.Add(item.Package, item.ToatlSale, item.Amount);
        }
    }
    else
    {
        var lstMemberwiseSale = (from m in db.MemberInfoes
                                 join s in db.MemberShips on m.MemberID equals s.MemberID
                                 join p in db.Payments on s.MembershipId equals p.MembershipId
                                 where m.GymId == gymid && m.BranchId == branchid
                                 group s by s.MembershipType into lstPayments

                                 select new
                                 {

                                     Package = lstPayments.Key,
                                     Amount = lstPayments.Sum(x => x.Amount),
                                     ToatlSale = lstPayments.Count()

                                 }).ToList();
        foreach (var item in lstMemberwiseSale)
        {
            dt.Rows.Add(item.Package, item.ToatlSale, item.Amount);
        }
    }


    return dt;
}

public JsonResult GenarateExcelReportsBranchwiseSale(string startdate, string todate)
{
    DataTable dt = new DataTable();
    dt = BranchwiseSaleTable(startdate, todate);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Branchwise Sale");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsBranchwiseSale(string startdate, string todate)
{
    DataTable dt = new DataTable();
    dt = BranchwiseSaleTable(startdate, todate);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable BranchwiseSaleTable(string startdate, string todate)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Date");
    dt.Columns.Add("Branch Name");
    dt.Columns.Add("Membership Count");
    dt.Columns.Add("Membership Sales");
    dt.Columns.Add("Total Sales");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (todate != "")
    {
        endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (startdate != "")
    {
        if (todate == "")
        {
            endDate = DateTime.Today.Date;
        }
        var lsBranchWise = (from m in db.MemberInfoes
                            join s in db.MemberShips on m.MemberID equals s.MemberID
                            join p in db.Payments on s.MembershipId equals p.MembershipId
                            join b in db.Branches on m.BranchId equals b.BranchId
                            where m.GymId == gymid && p.PaymentDate >= startDate && p.PaymentDate <= endDate
                            group p by new { day = p.PaymentDate, branch = b.BranchName } into lstSale
                            //,paymentType=p.PaymentType
                            select new
                            {

                                dt = lstSale.Key.day.ToString(),
                                PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                                BranchName = lstSale.Key.branch,
                                membershipCount = lstSale.Select(x => x.MembershipId).ToList()
                            }).ToList();
        foreach (var item in lsBranchWise)
        {
            if (item.dt != "")
            {
                dt.Rows.Add(item.dt, item.BranchName, item.membershipCount.Count, item.PaymentAmount, item.PaymentAmount);
            }
           
        }
    }
    else
    {
        var lsBranchWise = (from m in db.MemberInfoes
                           join s in db.MemberShips on m.MemberID equals s.MemberID
                           join p in db.Payments on s.MembershipId equals p.MembershipId
                           join b in db.Branches on m.BranchId equals b.BranchId
                           where m.GymId == gymid
                           group p by new { day = p.PaymentDate, branch = b.BranchName } into lstSale
                           //,paymentType=p.PaymentType
                           select new
                           {

                               dt = lstSale.Key.day.ToString(),
                               PaymentAmount = lstSale.Sum(x => x.PaymentAmount),
                               BranchName = lstSale.Key.branch,
                               membershipCount = lstSale.Select(x => x.MembershipId).ToList()
                           }).ToList();
        foreach (var item in lsBranchWise)
        {
            if(item.dt!="")
            {
                dt.Rows.Add(item.dt, item.BranchName, item.membershipCount.Count, item.PaymentAmount, item.PaymentAmount);
            }
           
        }
    }


    return dt;
}


public JsonResult GenarateExcelReportsPOSSale(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = POSSaleTable(startdate, todate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report POS Sale");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsPOSSale(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = POSSaleTable(startdate, todate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable POSSaleTable(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Order Date");
    dt.Columns.Add("Cash");
    dt.Columns.Add("Card");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (todate != "")
    {
        endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (startdate != "")
    {
        if (todate == "")
        {
            endDate = DateTime.Today.Date;
        }

        var lstPosReport = (from po in db.PurchaseOrders
                            join o in db.Orders on po.PurchaseOrderId equals o.PurchaseOrderId
                            where po.GymId == gymid && po.BranchId == BranchId
                            group new { po, o } by new { day = po.OrderDate, total = o.Total } into lstPos
                            select new
                            {
                                dt = lstPos.Key.day.ToString(),
                                PaymentVia = lstPos.Select(x => new { x.po.PaymentVia, x.o.Total })
                            }).ToList();
        foreach (var item in lstPosReport)
        {
            int cash = 0;
            int card = 0;
            foreach(var paymentMode in item.PaymentVia)
            {
                if (paymentMode.PaymentVia == "Cash")
                {
                    cash = cash + Convert.ToInt32(paymentMode.Total);
                }
                else if (paymentMode.PaymentVia == "Card")
                {
                    card = card + Convert.ToInt32(paymentMode.Total);
                }
            }
            if (item.dt != "")
            {
                dt.Rows.Add(item.dt, cash,card);
            }

        }
    }
    else
    {
        var lstPosReport = (from po in db.PurchaseOrders
                            join o in db.Orders on po.PurchaseOrderId equals o.PurchaseOrderId
                            where po.GymId == gymid && po.BranchId == BranchId
                            group new { po, o } by new { day = po.OrderDate, total = o.Total } into lstPos
                            select new
                            {
                                dt = lstPos.Key.day.ToString(),
                                PaymentVia = lstPos.Select(x => new { x.po.PaymentVia, x.o.Total })
                            }).ToList();
        foreach (var item in lstPosReport)
        {
            int cash = 0;
            int card = 0;
            foreach (var paymentMode in item.PaymentVia)
            {
                if (paymentMode.PaymentVia == "Cash")
                {
                    cash = cash + Convert.ToInt32(paymentMode.Total);
                }
                else if (paymentMode.PaymentVia == "Card")
                {
                    card = card + Convert.ToInt32(paymentMode.Total);
                }
            }
            if (item.dt != "")
            {
                dt.Rows.Add(item.dt, cash, card);
            }

        }
    }


    return dt;
}

public JsonResult GenarateExcelReportsPOSRepresentativeSale(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = POSRepresentativSaleTable(startdate, todate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report POS Representative Sale");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsPOSRepresentativSale(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = POSRepresentativSaleTable(startdate, todate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable POSRepresentativSaleTable(string startdate, string todate, int BranchId)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    string gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]).ToString();
    dt.Columns.Add("Name");
    dt.Columns.Add("Quantity");
    dt.Columns.Add("Total Sales");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (todate != "")
    {
        endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (startdate != "")
    {
        if (todate == "")
        {
            endDate = DateTime.Today.Date;
        }

        var lstPos = (from P in db.PurchaseOrderHeaders
                      join Po in db.PurchaseOrderDetails on P.PurchaseId equals Po.PurchaseId
                      where P.GymId == gymid && P.BranchId == BranchId && P.OrderDate >= startDate && P.OrderDate <= endDate
                      group new { P, Po } by new
                      {
                          Representative = P.Representative,
                      } into lstPosRep
                      select new
                      {
                          Representative = lstPosRep.Key.Representative,
                          TotalAmount = lstPosRep.Sum(x => x.P.TotalAmount),
                          Qty = lstPosRep.Sum(x => x.Po.Qty),

                      }).ToList();
        foreach (var item in lstPos)
        {
            dt.Rows.Add(item.Representative, item.Qty, item.TotalAmount);
        }
     

      
    }
    else
    {
        var lstPos = (from P in db.PurchaseOrderHeaders
                      join Po in db.PurchaseOrderDetails on P.PurchaseId equals Po.PurchaseId
                      where P.GymId == gymid && P.BranchId == BranchId
                      group new { P, Po } by new
                      {
                          Representative = P.Representative,
                      } into lstPosRep
                      select new
                      {
                          Representative = lstPosRep.Key.Representative,
                          TotalAmount = lstPosRep.Sum(x => x.P.TotalAmount),
                          Qty = lstPosRep.Sum(x => x.Po.Qty),

                      }).ToList();
        foreach (var item in lstPos)
        {
            dt.Rows.Add(item.Representative, item.Qty, item.TotalAmount);
        }
    }


    return dt;
}

public JsonResult GenarateExcelReportsStock(string productName, string startDate, string endDate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = StockTable(productName, startDate, endDate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Stock");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsStock(string productName, string startDate, string endDate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = StockTable(productName, startDate, endDate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable StockTable(string productName, string startdate, string enddate, int BranchId)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Manufacture Name");
    dt.Columns.Add("Product Name");
    dt.Columns.Add("Stock In");
    dt.Columns.Add("Stock Out");
    dt.Columns.Add("Date");
    dt.Columns.Add("Total Charges");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (enddate != "")
    {
        endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

    if (startdate != "" && enddate != "" && productName != "")
    {
        var lstStockReport = (from p in db.Products
                              join s in db.Stocks on p.ProductId equals s.ProductId
                              where p.GymId == gymid && p.BranchId == BranchId && p.ProductName == productName && s.StockinDate >= startDate && s.StockinDate <= endDate
                              group new { p, s } by new
                              {
                                  product = p.ProductName,
                                  manufacture = p.BrandName,
                                  stockin = s.StockIn,
                                  stockout = s.StockOut,
                                  date = s.StockinDate,
                                  price = p.Price
                              } into lstProductDetails
                              select new
                              {
                                  Product = lstProductDetails.Key.product,
                                  Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                  StockIn = lstProductDetails.Key.stockin.ToString(),
                                  StockOut = lstProductDetails.Key.stockout.ToString(),
                                  Date = lstProductDetails.Key.date.ToString(),
                                  TotalCharges = lstProductDetails.Key.price.ToString()

                                  //Total = lstPos.Sum(x=>x.o.Total)
                              }).ToList();

        foreach (var item in lstStockReport)
        {
            dt.Rows.Add(item.Manufacture, item.Product, item.StockIn, item.StockOut, item.Date, item.TotalCharges);
        }
     
    }
    else if (productName != "" && (startdate == "" && enddate == ""))
    {
        var lstStockReport = (from p in db.Products
                              join s in db.Stocks on p.ProductId equals s.ProductId
                              where p.GymId == gymid && p.BranchId == BranchId && p.ProductName.Trim().ToUpper().Contains(productName.Trim())
                              group new { p, s } by new
                              {
                                  product = p.ProductName,
                                  manufacture = p.BrandName,
                                  stockin = s.StockIn,
                                  stockout = s.StockOut,
                                  date = s.StockinDate,
                                  price = p.Price
                              } into lstProductDetails
                              select new
                              {
                                  Product = lstProductDetails.Key.product,
                                  Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                  StockIn = lstProductDetails.Key.stockin.ToString(),
                                  StockOut = lstProductDetails.Key.stockout.ToString(),
                                  Date = lstProductDetails.Key.date.ToString(),
                                  TotalCharges = lstProductDetails.Key.price.ToString()

                                  //Total = lstPos.Sum(x=>x.o.Total)
                              }).ToList();

        foreach (var item in lstStockReport)
        {
            dt.Rows.Add(item.Manufacture, item.Product, item.StockIn, item.StockOut, item.Date, item.TotalCharges);
        }
    }
    else if (productName == "" && startdate != "" && enddate != "")
    {
        var lstStockReport = (from p in db.Products
                              join s in db.Stocks on p.ProductId equals s.ProductId
                              where p.GymId == gymid && p.BranchId == BranchId && s.StockinDate >= startDate && s.StockinDate <= endDate
                              group new { p, s } by new
                              {
                                  product = p.ProductName,
                                  manufacture = p.BrandName,
                                  stockin = s.StockIn,
                                  stockout = s.StockOut,
                                  date = s.StockinDate,
                                  price = p.Price
                              } into lstProductDetails
                              select new
                              {
                                  Product = lstProductDetails.Key.product,
                                  Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                  StockIn = lstProductDetails.Key.stockin.ToString(),
                                  StockOut = lstProductDetails.Key.stockout.ToString(),
                                  Date = lstProductDetails.Key.date.ToString(),
                                  TotalCharges = lstProductDetails.Key.price.ToString()

                                  //Total = lstPos.Sum(x=>x.o.Total)
                              }).ToList();

        foreach (var item in lstStockReport)
        {
            dt.Rows.Add(item.Manufacture, item.Product, item.StockIn, item.StockOut, item.Date, item.TotalCharges);
        }
    }
    else
    {
        var lstStockReport = (from p in db.Products
                              join s in db.Stocks on p.ProductId equals s.ProductId
                              where p.GymId == gymid && p.BranchId == BranchId
                              group new { p, s } by new
                              {
                                  product = p.ProductName,
                                  manufacture = p.BrandName,
                                  stockin = s.StockIn,
                                  stockout = s.StockOut,
                                  date = s.StockinDate,
                                  price = p.Price
                              } into lstProductDetails
                              select new
                              {
                                  Product = lstProductDetails.Key.product,
                                  Manufacture = lstProductDetails.Key.manufacture.ToString(),
                                  StockIn = lstProductDetails.Key.stockin.ToString(),
                                  StockOut = lstProductDetails.Key.stockout.ToString(),
                                  Date = lstProductDetails.Key.date.ToString(),
                                  TotalCharges = lstProductDetails.Key.price.ToString()

                                  //Total = lstPos.Sum(x=>x.o.Total)
                              }).ToList();
        foreach (var item in lstStockReport)
        {
            dt.Rows.Add(item.Manufacture, item.Product, item.StockIn, item.StockOut, item.Date, item.TotalCharges);
        }
    }


    return dt;
}

public JsonResult GenarateExcelReportSourceWiseEnquiry(string startDate, string endDate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = SourceWiseEnquiryTable( startDate, endDate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Source Wise Enquiry");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportSourceWiseEnquiry(string startDate, string endDate, int BranchId)
{
    DataTable dt = new DataTable();
    dt = SourceWiseEnquiryTable( startDate, endDate, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable SourceWiseEnquiryTable(string startdate, string enddate, int BranchId)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Source Name");
    dt.Columns.Add("Total Enquiries");

    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (enddate != "")
    {
        endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }

   if (startdate != "" && enddate != "")
            {
                var lstSourceEnq = (from e in db.Enquiries
                                    where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate >=startDate && e.EnquiryDate <=endDate
                                    group e by new { source = e.HowDidYouKnow } into lstenq

                                    select new
                                    {
                                        Source = lstenq.Key.source,
                                        Total = lstenq.Select(x => x.HowDidYouKnow).ToList()
                                    }).ToList();

                foreach (var item in lstSourceEnq)
                {
                    if (item.Source != null && item.Source != "--Select--")
                    {
                        dt.Rows.Add(item.Source, item.Total.Count);

                    }
                }
            }
            else
            {
                var lstSourceEnq = (from e in db.Enquiries
                                    where e.GymId == gymid && e.BranchId == BranchId
                                    group e by new { source = e.HowDidYouKnow } into lstenq

                                    select new
                                    {
                                        Source = lstenq.Key.source,
                                        Total = lstenq.Select(x => x.HowDidYouKnow).ToList()
                                    }).ToList();

                foreach (var item in lstSourceEnq)
                {
                    if (item.Source != null && item.Source != "--Select--")
                    {
                        dt.Rows.Add(item.Source, item.Total.Count);

                    }
                                    }
            }

    return dt;
}

public JsonResult GenarateExcelReportEnquiry(string member, string startdate, string enddate, string status, int BranchId)
{
    DataTable dt = new DataTable();
    dt = EnquiryTable(member, startdate, enddate, status, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Enquiry");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportEnquiry(string member, string startdate, string enddate, string status, int BranchId)
{
    DataTable dt = new DataTable();
    dt = EnquiryTable(member, startdate, enddate, status, BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable EnquiryTable(string member, string startdate, string enddate, string status, int BranchId)
{
    DataTable dt = new DataTable();
    DateTime startDate = new DateTime();
    DateTime endDate = new DateTime();
    int enquiryId = 0;
    string memberName = "";
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
    dt.Columns.Add("Member Name");
    dt.Columns.Add("Mobile No.");
    dt.Columns.Add("Enquiry Date");
    dt.Columns.Add("Last FollowUp Date");
    dt.Columns.Add("Next FollowUp Date");
    dt.Columns.Add("Status");
    dt.Columns.Add("Follow up");
    if (startdate != "")
    {
        startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    if (enddate != "")
    {
        endDate = DateTime.ParseExact(enddate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
    }
    try
    {
        enquiryId = int.Parse(member);
    }
    catch (Exception ex)
    {
        memberName = member;
    }

    if (memberName != "" && memberName != null)
    {

        var enqlist = (from e in db.Enquiries
                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                       where ((e.FirstName.ToLower().Trim() + " " + e.LastName.ToLower().Trim()).Contains(memberName.ToLower().Trim())) && e.GymId == gymid && e.BranchId == BranchId

                       select new
                       {
                           membername = e.FirstName + " " + e.LastName,
                           mobilenumber = e.MobileNumber,
                           EnquiryDate = e.EnquiryDate,
                           LastFollowUpDate = f.LastFollowUpDate,
                           NextFollowUpDate = f.NextFollowUpDate,
                           EnqStatus = f.EnqStatus.ToString(),
                           Notes = e.Notes.ToString()
                       }).ToList();

        foreach (var item in enqlist)
        {
            dt.Rows.Add(item.membername, item.mobilenumber,
                  item.EnquiryDate != null ? item.EnquiryDate.Value.Day + "/" + item.EnquiryDate.Value.Month + "/" + item.EnquiryDate.Value.Year : "",
                    item.LastFollowUpDate != null ? item.LastFollowUpDate.Value.Day + "/" + item.LastFollowUpDate.Value.Month + "/" + item.LastFollowUpDate.Value.Year : "",
                      item.NextFollowUpDate != null ? item.NextFollowUpDate.Value.Day + "/" + item.NextFollowUpDate.Value.Month + "/" + item.NextFollowUpDate.Value.Year : "",
                     item.EnqStatus, item.Notes);
        }
    }

    else if (enquiryId != 0)
    {

        var enqlist = (from e in db.Enquiries
                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                       where (e.EnquiryId == enquiryId) && e.GymId == gymid && e.BranchId == BranchId
                       select new
                       {
                           membername = e.FirstName + " " + e.LastName,
                           mobilenumber = e.MobileNumber,
                           EnquiryDate = e.EnquiryDate,
                           LastFollowUpDate = f.LastFollowUpDate,
                           NextFollowUpDate = f.NextFollowUpDate,
                           EnqStatus = f.EnqStatus.ToString(),
                           Notes = e.Notes.ToString()
                       }).ToList();
        foreach (var item in enqlist)
        {
            dt.Rows.Add(item.membername, item.mobilenumber,
                  item.EnquiryDate != null ? item.EnquiryDate.Value.Day + "/" + item.EnquiryDate.Value.Month + "/" + item.EnquiryDate.Value.Year : "",
                    item.LastFollowUpDate != null ? item.LastFollowUpDate.Value.Day + "/" + item.LastFollowUpDate.Value.Month + "/" + item.LastFollowUpDate.Value.Year : "",
                      item.NextFollowUpDate != null ? item.NextFollowUpDate.Value.Day + "/" + item.NextFollowUpDate.Value.Month + "/" + item.NextFollowUpDate.Value.Year : "",
                     item.EnqStatus, item.Notes);
        }
    }
    else
    {
        var enqlist = (from e in db.Enquiries
                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                       where (f.EnqStatus.Contains(status)
                       || f.LastFollowUpDate >= startDate || f.LastFollowUpDate <= endDate) && e.GymId == gymid && e.BranchId == BranchId
                       select new
                       {
                           membername = e.FirstName + " " + e.LastName,
                           mobilenumber = e.MobileNumber,
                           EnquiryDate = e.EnquiryDate,
                           LastFollowUpDate = f.LastFollowUpDate,
                           NextFollowUpDate = f.NextFollowUpDate,
                           EnqStatus = f.EnqStatus.ToString(),
                           Notes = e.Notes.ToString()
                       }).ToList();
        foreach (var item in enqlist)
        {
            dt.Rows.Add(item.membername, item.mobilenumber,
                  item.EnquiryDate != null ? item.EnquiryDate.Value.Day + "/" + item.EnquiryDate.Value.Month + "/" + item.EnquiryDate.Value.Year : "",
                    item.LastFollowUpDate != null ? item.LastFollowUpDate.Value.Day + "/" + item.LastFollowUpDate.Value.Month + "/" + item.LastFollowUpDate.Value.Year : "",
                      item.NextFollowUpDate != null ? item.NextFollowUpDate.Value.Day + "/" + item.NextFollowUpDate.Value.Month + "/" + item.NextFollowUpDate.Value.Year : "",
                     item.EnqStatus, item.Notes);
        }
    }


    return dt;
}

public JsonResult GenarateExcelReportsMemberReference(int BranchId)
{
    DataTable dt = new DataTable();
    dt = GenerateReportsMemberReference(BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = ExportListUsingEPPlus(dt, "Report Member Reference");
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

[HttpPost]
public JsonResult GenaratePdfReportsMemberReference(int BranchId)
{
    DataTable dt = new DataTable();
    dt = GenerateReportsMemberReference(BranchId);
    if (dt.Rows.Count > 0)
    {
        var result = GeneratePdf(dt);
        return result;
    }
    return Json("", JsonRequestBehavior.AllowGet);
}

public DataTable GenerateReportsMemberReference(int BranchId)
{
    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"].ToString());
    List<MemberReference> lstMemberReference = new List<MemberReference>();
    DataTable dt = new DataTable();
    dt.Columns.Add("Member Name");
    dt.Columns.Add("Reference Name Joined Date Membership Amount");
    var lstReference = (from P in db.MemberInfoes
                        //join M in db.MemberShips on P.MemberID equals M.MemberID
                        where P.GymId == gymid && P.BranchId == BranchId
                        group new { P } by new
                        {
                            member = P.MemberReference,
                        } into lstMemberRef
                        select new
                        {
                            Representative = lstMemberRef.Key.member,
                            MemberName = (from s in db.MemberInfoes where s.MemberID == lstMemberRef.Key.member select s.FirstName + " " + s.LastName).FirstOrDefault(),
                            //ReferedId=(from m in lstMemberRef where )
                            ////ReferenceName = lstMemberRef.Select((x => x.P.FirstName + " " + x.P.LastName + " " + x.P.MemberID + " " + " " + x.P.EnrollDate + " " + x.M.MembershipType + " " + x.M.Amount)),
                            ////Joineddate=lstMemberRef.Select(x=>x.P.EnrollDate),
                            ////Membership=lstMemberRef.Select(x=>x.M.MembershipType),
                            ////Amount =lstMemberRef.Select(x=>x.M.Amount),
                            //ReffererId=lstMemberRef.Select(x=>x.P.MemberID)
                        }).ToList();

    if (lstReference.Count > 0)
    {
        foreach (var memberref in lstReference)
        {
            if (memberref.Representative != null && memberref.Representative > 0)
            {
                var lstMemberInfo = (from s in db.MemberInfoes
                                     join m in db.MemberShips on s.MemberID equals m.MemberID
                                     where s.GymId == gymid && s.BranchId == BranchId && s.MemberReference == memberref.Representative
                                     select new
                                     {
                                         MemberId = s.MemberID,
                                         MemberName = s.FirstName + " " + s.LastName,
                                         JoinedDate = s.EnrollDate.ToString(),
                                         Membership = m.MembershipType,
                                         Amount = m.Amount
                                     }
                                       );
                var lstmember = lstMemberInfo.GroupBy(x => x.MemberId).Select(x => x.FirstOrDefault()).ToList();

                List<MemberReferenceList> lstMemberreferenecelist = new List<MemberReferenceList>();
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                foreach (var member in lstmember)
                {
                    

                    str.AppendLine(member.MemberId.ToString() + member.MemberName + member.Membership + member.Amount + member.JoinedDate);
                }
                dt.Rows.Add(memberref.MemberName+" (# "+memberref.Representative+" )",str.ToString());
            }
        }

       

    }


    return dt;

}

        [HttpPost]
        public JsonResult ExportExcel(string membername, string membershipid, string startdate, string todate, string category, string active,int BranchId)
        {
            DataTable dt = new DataTable();
            var result = new { Success = "False", Message = "Unable To Save Information." };
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int memberId = 0;
            string memberName = "";
            int membershipId = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int Category = 0;
            
            try
            {

                if (membershipid != "")
                {
                    membershipId = Convert.ToInt32(membershipid);
                }
                if (category != "")
                {
                    Category = Convert.ToInt32(category);
                }
                if (startdate != "")
                {
                    startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (todate != "")
                {
                    endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (membername != "")
                {
                    try
                    {
                        memberId = int.Parse(membername);
                    }
                    catch (Exception ex)
                    {
                        memberName = membername;
                    }
                }


                dt.Columns.Add("MemberID");
                dt.Columns.Add("FirstName");
                dt.Columns.Add("Address");
                dt.Columns.Add("MobileNumber");
                dt.Columns.Add("Dob");
                dt.Columns.Add("Email");
                dt.Columns.Add("EnrollDate");
                dt.Columns.Add("Package");
                dt.Columns.Add("EndDate");
                dt.Columns.Add("Amount");
                dt.Columns.Add("PaymentAmount");



                if (memberName != "" && memberName != null)
                {

                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where ((mi.FirstName + " " + mi.LastName).Contains(memberName) || ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          Address = mi.MemberAddress.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Dob = mi.Dob.ToString(),
                                          Email = mi.Email,
                                          EnrollDate = mi.EnrollDate.ToString(),
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount

                                      }).ToList();

                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package,item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        var result1 = ExportListUsingEPPlus(dt,"Report Memberslist");
                        return result1;

                    }
                }

                else if (memberId != 0)
                {

                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where (mi.MemberID == memberId || ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          Address = mi.MemberAddress.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Dob = mi.Dob.ToString(),
                                          Email = mi.Email,
                                          EnrollDate = mi.EnrollDate.ToString(),
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount

                                      }).ToList();

                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package,item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        var result1 = ExportListUsingEPPlus(dt,"Report Memberslist");
                        return result1;
                    }
                }
                else
                {
                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where (ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId

                                      select new
                                 {
                                     MemberID = mi.MemberID,
                                     FirstName = mi.FirstName + " " + mi.LastName,
                                     Address = mi.MemberAddress.ToString(),
                                     MobileNumber = mi.MobileNumber,
                                     Dob = mi.Dob.ToString(),
                                     Email = mi.Email,
                                     EnrollDate = mi.EnrollDate.ToString(),
                                     Package = ms.MembershipType,
                                     StartDate = ms.StartDate.ToString(),
                                     EndDate = ms.EndDate.ToString(),
                                     Amount = ms.Amount,
                                     PaymentAmount = p.PaymentAmount

                                 }).ToList();
                    if (memberlist.Count == 0)
                    {
                        memberlist = (from m in db.MemberInfoes
                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                      where m.GymId == gymid && m.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = m.MemberID,
                                          FirstName = m.FirstName + " " + m.LastName,
                                          Address = m.MemberAddress.ToString(),
                                          MobileNumber = m.MobileNumber,
                                          Dob = m.Dob.ToString(),
                                          Email = m.Email,
                                          EnrollDate = m.EnrollDate.ToString(),
                                          Package = s.MembershipType,
                                          StartDate = s.StartDate.ToString(),
                                          EndDate = s.EndDate.ToString(),
                                          Amount = s.Amount,
                                          PaymentAmount = p.PaymentAmount
                                      }).ToList();

                    }

                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package, item.StartDate, item.EndDate, item.Amount, item.PaymentAmount);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        var result1 = ExportListUsingEPPlus(dt, "Report Memberslist");
               return result1;

                    }
                }




            }
            catch (Exception ex)
            {
                result =  new { Success = "False", Message = ex.ToString() };
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportListUsingEPPlus(DataTable dt,string reportname)
        {
                if (dt != null)
                    {
                        using (ExcelPackage xp = new ExcelPackage())
                        {
                            ExcelWorksheet ws = xp.Workbook.Worksheets.Add(reportname);

                                int rowstart = 2;
                                int colstart = 2;
                                int rowend = rowstart;
                                int colend = colstart + dt.Columns.Count;

                                ws.Cells[rowstart, colstart, rowend, colend].Merge = true;
                                ws.Cells[rowstart, colstart, rowend, colend].Value = reportname;
                                ws.Cells[rowstart, colstart, rowend, colend].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Font.Bold = true;
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                ws.Cells[rowstart, colstart, rowend, colend].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                                rowstart += 2;
                                rowend = rowstart + dt.Rows.Count;
                                ws.Cells[rowstart, colstart].LoadFromDataTable(dt, true);
                                int i = 1;
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    i++;
                                    if (dc.DataType == typeof(decimal))
                                        ws.Column(i).Style.Numberformat.Format = "#0.00";
                                }
                                ws.Cells[ws.Dimension.Address].AutoFitColumns();



                                ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Top.Style =
                                   ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Bottom.Style =
                                   ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Left.Style =
                                   ws.Cells[rowstart, colstart, rowend, colend].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            //    Response.Clear();
                            //    Response.ClearHeaders();
                            //    Response.ClearContent();
                            //    Response.Buffer = true;
                            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            //Response.AddHeader("content-disposition", "attachment;filename=" + "rpts" + ".xlsx");
                           
                            //Response.BinaryWrite(xp.GetAsByteArray());
                            //Response.End();

                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    xp.SaveAs(memoryStream);
                                    memoryStream.Position = 0;
                                    Session["ExcelResult"] = memoryStream.ToArray();
                                }

                                return new JsonResult()
                                {
                                    Data = new { FileName =reportname+".xlsx" }
                                };
                                
                        }
                    }
               
            
           

           return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual ActionResult Download(string filename)
        {

            if (Session["ExcelResult"] != null)
            {
                byte[] fileBytes = (byte[])Session["ExcelResult"];
                return File(fileBytes, "application/vnd.ms-excel", filename);
          
            }
            
            //string fullPath = Path.Combine(Server.MapPath("~/MyFiles"), fileGuid);
            //return File(fullPath, "application/vnd.ms-excel", filename);
            return View();
        }

        [HttpPost]
        public JsonResult ExportPDF(string membername, string membershipid, string startdate, string todate, string category, string active, int BranchId)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                Log.Info("ExportPDF");
                DataTable dt = new DataTable();
              
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                int memberId = 0;
                string memberName = "";
                int membershipId = 0;
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                int Category = 0;
                if (membershipid != "")
                {
                    membershipId = Convert.ToInt32(membershipid);
                }
                if (category != "")
                {
                    Category = Convert.ToInt32(category);
                }
                if (startdate != "")
                {
                    startDate = DateTime.ParseExact(startdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (todate != "")
                {
                    endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (membername != "")
                {
                    try
                    {
                        memberId = int.Parse(membername);
                    }
                    catch (Exception ex)
                    {
                        memberName = membername;
                    }
                }
                dt.Columns.Add("MemberID");
                dt.Columns.Add("FirstName");
                dt.Columns.Add("Address");
                dt.Columns.Add("MobileNumber");
                dt.Columns.Add("Dob");
                dt.Columns.Add("Email");
                dt.Columns.Add("EnrollDate");
                dt.Columns.Add("Package");
                dt.Columns.Add("EndDate");
                dt.Columns.Add("Amount");
                dt.Columns.Add("PaymentAmount");
                if (memberName != "" && memberName != null)
                {
                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where ((mi.FirstName + " " + mi.LastName).Contains(memberName) || ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId
                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          Address = mi.MemberAddress.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Dob = mi.Dob.ToString(),
                                          Email = mi.Email,
                                          EnrollDate = mi.EnrollDate.ToString(),
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount
                                      }).ToList();
                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package, item.EndDate, item.Amount, item.PaymentAmount);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        //bool issuccess = ExportToPdf(dt,pathMyDocuments+"\\Reports Membership.pdf");
                        //if (issuccess)
                        //{
                        //    result = new { Success = "True", Message = "PDF Downloaded Successfully" };
                        //}
                        //bool issuccess = GeneratePdf(dt);
                        //if (issuccess)
                        //{
                        //    result = new { Success = "True", Message = "Excel Downloaded Successfully" };
                        //}
                        var result1 = GeneratePdf(dt);
                        return result1;

                    }
                }

                else if (memberId != 0)
                {

                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where (mi.MemberID == memberId || ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          Address = mi.MemberAddress.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Dob = mi.Dob.ToString(),
                                          Email = mi.Email,
                                          EnrollDate = mi.EnrollDate.ToString(),
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount

                                      }).ToList();

                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package, item.EndDate, item.Amount, item.PaymentAmount);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        //bool issuccess = ExportToPdf(dt, pathMyDocuments + "\\Reports Membership.pdf");
                        //if (issuccess)
                        //{
                        //    result = new { Success = "True", Message = "Excel Downloaded Successfully" };
                        //}
                        //bool issuccess = GeneratePdf(dt);
                        //if (issuccess)
                        //{
                        //    result = new { Success = "True", Message = "Excel Downloaded Successfully" };
                        //}
                        var result1 = GeneratePdf(dt);
                        return result1;

                    }

                }
                else
                {
                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where (ms.MembershipId == membershipId
                                      || ms.StartDate == startDate || ms.EndDate == endDate) && mi.GymId == gymid && mi.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          Address = mi.MemberAddress.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Dob = mi.Dob.ToString(),
                                          Email = mi.Email,
                                          EnrollDate = mi.EnrollDate.ToString(),
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount

                                      }).ToList();
                    if (memberlist.Count == 0)
                    {
                        memberlist = (from m in db.MemberInfoes
                                      join s in db.MemberShips on m.MemberID equals s.MemberID
                                      join p in db.Payments on s.MembershipId equals p.MembershipId
                                      where m.GymId == gymid && m.BranchId == BranchId

                                      select new
                                      {
                                          MemberID = m.MemberID,
                                          FirstName = m.FirstName + " " + m.LastName,
                                          Address = m.MemberAddress.ToString(),
                                          MobileNumber = m.MobileNumber,
                                          Dob = m.Dob.ToString(),
                                          Email = m.Email,
                                          EnrollDate = m.EnrollDate.ToString(),
                                          Package = s.MembershipType,
                                          StartDate = s.StartDate.ToString(),
                                          EndDate = s.EndDate.ToString(),
                                          Amount = s.Amount,
                                          PaymentAmount = p.PaymentAmount
                                      }).ToList();

                    }

                    foreach (var item in memberlist)
                    {
                        dt.Rows.Add(item.MemberID, item.FirstName, item.Address, item.MobileNumber, item.Dob, item.Email,
        item.EnrollDate, item.Package, item.EndDate, item.Amount, item.PaymentAmount);

                    }

                    if (dt.Rows.Count > 0)
                    {
                        //bool issuccess = GeneratePdf(dt);
                        ////bool issuccess = ExportToPdf(dt, pathMyDocuments + "\\Reports Membership.pdf");
                        //if (issuccess)
                        //{
                        //    result = new { Success = "True", Message = "Excel Downloaded Successfully" };
                        //}
                        var result1 = GeneratePdf(dt);
                        return result1;
                    }
                }
            }
            catch (Exception ex)
            {
                result = new { Success = "False", Message = ex.ToString() };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

   [HttpPost]
   public JsonResult GeneratePdf(DataTable dt)
   {
       bool issuccess = false;
       Log.Info("Generate PDF method Started");
       Utility.Logger.WriteLog("Generate PDF method Started");
       // write code here to save the data in database. 
       //var fName = string.Format("MYPDF-{0}.pdf", DateTime.Now.ToString("s"));
       try
       {
           using (var ms = new MemoryStream())
           {
               Log.Info("Memmory Stream");
               Utility.Logger.WriteLog("Memmory Stream");
               using (var document = new Document(PageSize.A4, 50, 50, 15, 15))
               {
                   Log.Info("Document");
                   Utility.Logger.WriteLog("Document");
                   PdfWriter.GetInstance(document, ms);
                   document.Open();
                   Log.Info("Document opened");
                   Utility.Logger.WriteLog("Document opened");
                   iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
                   PdfPTable table = new PdfPTable(dt.Columns.Count);
                   PdfPRow row = null;
                   int cols = dt.Columns.Count;
                   //float[] widths = new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f };
                   int n = cols;
                   float[] widths = new float[n];
                   for (int i = 0; i <= cols - 1; i++)
                   {
                       widths[i] = 4f;
                   }
                   table.SetWidths(widths);
                   table.WidthPercentage = 100;
                   int iCol = 0;
                   string colname = "";
                   PdfPCell cell = new PdfPCell(new Phrase("Reports"));
                   cell.Colspan = dt.Columns.Count;
                   foreach (DataColumn c in dt.Columns)
                   {
                       table.AddCell(new Phrase(c.ColumnName, font5));
                   }
                   foreach (DataRow r in dt.Rows)
                   {
                       if (dt.Rows.Count > 0)
                       {
                           for (int i = 0; i <= cols - 1; i++)
                           {
                               table.AddCell(new Phrase(r[i].ToString(), font5));
                           }
                       }
                   }
                   document.Add(table);
                   document.Close();
                   Log.Info("Document Closed");
                   Utility.Logger.WriteLog("Document Closed");
               }

               var bytes = ms.ToArray();
               Session["DownLoadPdf"] = bytes;
               Log.Info("bytes converted");
               Utility.Logger.WriteLog(bytes.ToString());
               return new JsonResult()
               {
                   Data = new { FileName = bytes.ToString() }
               };
           }
       }
       catch (Exception ex)
       {
           Utility.Logger.WriteLog(ex.ToString());
           throw ex;
       }

       return Json("", JsonRequestBehavior.AllowGet);
   }
   public  ActionResult DownloadPDF(string filename)
   {
       Log.Info("DownloadPDF");
       if (Session["DownLoadPdf"] != null)
       {
           Log.Info("Session value");
           byte[] fileBytes = (byte[])Session["DownLoadPdf"];
           return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

       }
       Log.Info("Session value null");
       return new EmptyResult();
   }

   //private static void releaseObject(object obj)
   //{
   //    try
   //    {
   //        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
   //        obj = null;
   //    }
   //    catch
   //    {
   //        obj = null;
   //    }
   //    finally
   //    {
   //        GC.Collect();
   //    }
   //}

        //    public bool CreateExcel(DataTable dt, string excelPath)
        //{
        //    bool isSuccess = false;
        //    Excel.Application xlApp;
        //    Excel.Workbook xlWorkBook;
        //    Excel.Worksheet xlWorkSheet;
        //    object misValue = System.Reflection.Missing.Value;
        //    string filePath = "C:\\";
        //    string fileName = "Reprts";

        //    try
        //    {
        //        //Previous code was referring to the wrong class, throwing an exception
        //        xlApp = new Excel.Application();
        //        xlWorkBook = xlApp.Workbooks.Add(misValue);
               

        //        //string folderPath = "C:\\Justbok Reports\\";
        //        //if (!Directory.Exists(folderPath))
        //        //{
        //        //    Directory.CreateDirectory(folderPath);
        //        //}

        //        //using (xlWorkBook wb = new xlWorkBook())
        //        //{
        //        //    xlWorkBook.Worksheets.Add(dt, "Customers");
        //        //    xlWorkBook.SaveAs(folderPath + "DataGridViewExport.xlsx");
        //        //}


        //        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

        //        for (int i = 1; i <= dt.Rows.Count - 1; i++)
        //        {
        //            for (int j = 0; j <= dt.Columns.Count - 1; j++)
        //            {
        //                if (i == 1)
        //                {
        //                    xlWorkSheet.Cells[1, j + 1] = dt.Columns[j].ToString();
        //                    xlWorkSheet.Cells[1, j + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);
        //                }
        //                xlWorkSheet.Cells[i + 1, j + 1] = dt.Rows[i].ItemArray[j].ToString();
        //            }
        //        }



        //        //xlWorkBook.Worksheets.Add(dt, "Customers");
        //        //using (xlWorkBook wb = new xlWorkBook())
        //        //{

        //        //Response.Clear();
        //        //Response.Buffer = true;
        //        //Response.Charset = "";
        //        //Response.ContentType = "application/vnd.ms-excel";
        //        //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
        //        //using (MemoryStream MyMemoryStream = new MemoryStream())
        //        //{
        //        //    xlWorkBook.SaveAs(MyMemoryStream);
        //        //    MyMemoryStream.WriteTo(Response.OutputStream);
        //        //    Response.Flush();
        //        //    Response.End();
        //        //}
        //        //}
                
        //        //Excel.Range last = xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
        //        //Excel.Range range = xlWorkSheet.get_Range("A1", last);
        //        //range.Style.Color = Color.LightBlue;



        //        xlWorkBook.SaveAs(Path.Combine(filePath, fileName), Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //        xlWorkBook.Close(true, misValue, misValue);
        //        xlApp.Quit();
        //        isSuccess = true;
        //        releaseObject(xlApp);
        //        releaseObject(xlWorkBook);
        //        releaseObject(xlWorkSheet);



        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return isSuccess;
        //}

   //public bool ExportToPdf(DataTable dt, string path)
   //{
   //    bool isSuccess = false;

   //    Document document = new Document();
   //    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
   //    document.Open();
   //    iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

   //    PdfPTable table = new PdfPTable(dt.Columns.Count);
   //    PdfPRow row = null;
   //    int cols = dt.Columns.Count;
   //    //float[] widths = new float[] { 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f };
   //    int n = cols;
   //    float[] widths = new float[n];
   //    for (int i = 0; i <= cols - 1; i++)
   //    {
   //        widths[i] = 4f;
   //    }
   //    table.SetWidths(widths);
   //    table.WidthPercentage = 100;
   //    int iCol = 0;
   //    string colname = "";
   //    PdfPCell cell = new PdfPCell(new Phrase("Reports"));

   //    cell.Colspan = dt.Columns.Count;

   //    foreach (DataColumn c in dt.Columns)
   //    {

   //        table.AddCell(new Phrase(c.ColumnName, font5));
   //    }

   //    foreach (DataRow r in dt.Rows)
   //    {
   //        if (dt.Rows.Count > 0)
   //        {
   //            for (int i = 0; i <= cols - 1; i++)
   //            {
   //                table.AddCell(new Phrase(r[i].ToString(), font5));
   //            }

   //            //table.AddCell(new Phrase(r[0].ToString(), font5));
   //            //table.AddCell(new Phrase(r[1].ToString(), font5));
   //            //table.AddCell(new Phrase(r[2].ToString(), font5));
   //            //table.AddCell(new Phrase(r[3].ToString(), font5));
   //            //table.AddCell(new Phrase(r[4].ToString(), font5));
   //            //table.AddCell(new Phrase(r[5].ToString(), font5));
   //            //table.AddCell(new Phrase(r[6].ToString(), font5));
   //            //table.AddCell(new Phrase(r[7].ToString(), font5));
   //            //table.AddCell(new Phrase(r[8].ToString(), font5));
   //            //table.AddCell(new Phrase(r[9].ToString(), font5));
   //            //table.AddCell(new Phrase(r[10].ToString(), font5));
   //        }
   //    } document.Add(table);
   //    document.Close();
   //    isSuccess = true;

   //    return isSuccess;
   //}

    }
}
