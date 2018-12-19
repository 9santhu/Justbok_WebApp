using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using Justbok.ADModel;
using PagedList;
using System.Web.UI;
using System.Threading.Tasks;


namespace Justbok.Controllers
{
    public class AddBranch
    {
        public string BranchName { get; set; }
        public string BranchId { get; set; }

    }

    public class QuickPrintDetails
    {
         public string MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string MembershipType { get; set; }
        public string RecieptNumber { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentDate { get; set; }

    }

        
    public class DashboardController : LayoutBaseModel
    {

     
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        public ActionResult AdminDashboard()
        {

            return View();
        }

        

        [HttpGet]
        [OutputCache(CacheProfile = "hour")]
        public JsonResult GetServiceList()
        {

            JustbokEntities objMembersdata = new JustbokEntities();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var Services = (from services in db.GymServiceLists
                            where services.GymId == gymid && services.Enable==true

                            select new
                            {
                                ServiceName = services.ServiceName
                            }).ToList();
            return Json(Services, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult TodayDateMembershipSold(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var branchId = Convert.ToInt32(BranchId);
            DateTime tdayDate = DateTime.Now.Date;


            try
            {
                var lstMembershipSold = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         where mi.GymId == gymid && ms.StartDate == tdayDate && mi.BranchId == branchId

                                         select new
                                         {
                                             MembershipId = ms.MembershipId,
                                             StartDate = ms.StartDate,
                                             Amount = ms.Amount

                                         }).ToList();


                return Json(lstMembershipSold, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult TodayDateDuePayment(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);

            try
            {
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate == tdayDate
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();


                return Json(lstDuePayments, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult WeekDuePayment(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
            var branchId = Convert.ToInt32(BranchId);

            try
            {
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate >= weekdays
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();


                return Json(lstDuePayments, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
      [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult MonthDuePayment(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
            var branchId = Convert.ToInt32(BranchId);

            try
            {
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate >= month
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();


                return Json(lstDuePayments, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
       [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult WeekDateMembershipSold(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstWeekMembershipSold = (from mi in db.MemberInfoes
                                             join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                             where mi.GymId == gymid && mi.BranchId == branchId && ms.StartDate >= weekdays

                                             select new
                                             {
                                                 MembershipId = ms.MembershipId,
                                                 StartDate = ms.StartDate,
                                                 Amount = ms.Amount

                                             }).ToList();

                return Json(lstWeekMembershipSold, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult MonthDateMembershipSold(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstWeekMembershipSold = (from mi in db.MemberInfoes
                                             join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                             where mi.GymId == gymid && mi.BranchId == branchId && ms.StartDate >= month

                                             select new
                                             {
                                                 MembershipId = ms.MembershipId,
                                                 StartDate = ms.StartDate,
                                                 Amount = ms.Amount

                                             }).ToList();

                return Json(lstWeekMembershipSold, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")]
             
        public JsonResult MonthDateEnquriy(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstWeekEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate >= month

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();

                return Json(lstWeekEnquiry, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult WeekDateEnquriy(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstWeekEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate >= weekdays

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();

                return Json(lstWeekEnquiry, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult ListOfMeasurement(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstMeasurement = (from mi in db.MemberInfoes
                                      join ms in db.Measurements on mi.MemberID equals ms.MemberID
                                      where mi.GymId == gymid && mi.BranchId == branchId 

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MobileNumber = mi.MobileNumber,
                                          NextMeasurementDate = ms.NextMeasurementDate.ToString(),
                                          MeasurementDate = ms.MeasurementDate.ToString()
                                      }).ToList();

                return Json(lstMeasurement, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult TodayDateEnquriy(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lsttdayEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate == tdayDate

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();

                return Json(lsttdayEnquiry, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult LatestMembers(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstLatestMembers = (from mi in db.MemberInfoes
                                        where mi.GymId == gymid && mi.BranchId == branchId
                                        orderby mi.MemberID descending

                                        select new
                                        {
                                            MemberID = mi.MemberID,
                                            FirstName = mi.FirstName,
                                            LastName = mi.LastName,
                                            Email = mi.Email,
                                            MobileNumber = mi.MobileNumber,
                                            Gender = mi.Gender
                                        }).ToList();

                return Json(lstLatestMembers, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
          [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult FollowupTillToday(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstFollowups = (from e in db.Enquiries
                                    join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                    where e.GymId == gymid && f.NextFollowUpDate <= tdayDate
                                    orderby f.NextFollowUpDate descending

                                    select new
                                    {
                                        EnquiryId = e.EnquiryId,
                                        FollowupId = f.FollowupId,
                                        NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                        FirstName = e.FirstName,
                                        LastName = e.LastName,
                                        MobileNumber = e.MobileNumber,
                                        Gender = e.Gender
                                    }).ToList();

                return Json(lstFollowups, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult MembershipsDue(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstMembershipsDue = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                         where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount
                                         orderby ms.StartDate descending

                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate.ToString(),
                                             EndDate = ms.EndDate.ToString(),
                                             Amount = ms.Amount,
                                             PaymentAmount = pa.PaymentAmount

                                         }).ToList();

                return Json(lstMembershipsDue, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
          [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult MembershipExpired(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstMembershipsDue = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         where mi.GymId == gymid && mi.BranchId == branchId && ms.EndDate < tdayDate
                                         orderby ms.EndDate descending

                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate.ToString(),
                                             EndDate = ms.EndDate.ToString(),
                                             Amount = ms.Amount,


                                         }).ToList();

                return Json(lstMembershipsDue, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet] 
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult MembershipsDueAmount(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstMembershipsDue = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                         where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount
                                         orderby ms.StartDate descending

                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate.ToString(),
                                             EndDate = ms.EndDate.ToString(),
                                             Amount = ms.Amount,
                                             PaymentAmount = pa.PaymentAmount,
                                             PaymentDueDate = pa.PaymentDueDate.ToString()

                                         }).ToList();

                return Json(lstMembershipsDue, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]   
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult ListOFDateOFBirth(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstbday = (from mi in db.MemberInfoes
                               where mi.GymId == gymid && mi.BranchId == branchId && (mi.Dob.Value.Month == tdayDate.Month && mi.Dob.Value.Day >= tdayDate.Day)

                               select new
                               {
                                   MemberID = mi.MemberID,
                                   FirstName = mi.FirstName,
                                   LastName = mi.LastName,
                                   Dob = mi.Dob.ToString(),
                                   MobileNumber = mi.MobileNumber,
                                   Gender = mi.Gender

                               }).ToList();

                return Json(lstbday, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult ListOfAnniversary(string BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            try
            {
                var lstAnniversary = (from mi in db.MemberInfoes
                                      where mi.GymId == gymid && mi.BranchId == branchId && (mi.AnniversaryDate.Value.Month == tdayDate.Month && mi.AnniversaryDate.Value.Day >= tdayDate.Day)

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          AnniversaryDate = mi.AnniversaryDate.ToString(),
                                          MobileNumber = mi.MobileNumber,
                                          Gender = mi.Gender

                                      }).ToList();

                return Json(lstAnniversary, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public JsonResult QuickPrintSearchMemberList(int? page, float pagesize,String Member)
        {
            long mobilenumber = 0;
            string memberName = "";

            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            try
            {
                mobilenumber = long.Parse(Member);
            }
            catch (Exception ex)
            {
                memberName = Member;
            }


            if (memberName != "")
            {

                var lstMemberList = (from members in db.MemberInfoes
                                     join ms in db.MemberShips on members.MemberID equals ms.MemberID
                                     join p in db.Payments on ms.MembershipId equals p.MembershipId
                                     where members.GymId == gymid && (members.FirstName.ToLower() + " " + members.LastName.ToLower()).Contains(memberName.Trim().ToLower())

                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         MembershipId = ms.MembershipId,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         MembershipType = ms.MembershipType,
                                         RecieptNumber = p.RecieptNumber,
                                         PaymentAmount = p.PaymentAmount,
                                         PaymentDate = p.PaymentDate.ToString()
                                     })
                                       .Distinct()
                                     .ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                //return Json(lstMemberList, JsonRequestBehavior.AllowGet);
            }
            else if (mobilenumber != 0)
            {
                var lstMemberList = (from members in db.MemberInfoes
                                     join ms in db.MemberShips on members.MemberID equals ms.MemberID
                                     join p in db.Payments on ms.MembershipId equals p.MembershipId
                                     where members.GymId == gymid && members.MobileNumber.ToString().Contains(mobilenumber.ToString().Trim())
                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         MembershipId = ms.MembershipId,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         MembershipType = ms.MembershipType,
                                         RecieptNumber = p.RecieptNumber,
                                         PaymentAmount = p.PaymentAmount,
                                         PaymentDate = p.PaymentDate.ToString()
                                         
                                     }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                //return Json(lstMemberList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstMemberList = (from members in db.MemberInfoes
                                     join ms in db.MemberShips on members.MemberID equals ms.MemberID
                                     join p in db.Payments on ms.MembershipId equals p.MembershipId
                                     where members.GymId == gymid

                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         MembershipId = ms.MembershipId,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         MembershipType = ms.MembershipType,
                                         RecieptNumber = p.RecieptNumber,
                                         PaymentAmount = p.PaymentAmount,
                                         PaymentDate = p.PaymentDate.ToString()
                                        
                                     }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);


                //return Json(lstMemberList, JsonRequestBehavior.AllowGet);
            }



            return Json("", JsonRequestBehavior.AllowGet);
        }
          //[OutputCache(CacheProfile = "tenMin")] 
        public JsonResult GetInvoice(string membershipid, string receiptnumber,string Memberid)
        {

            JustbokEntities objPaymentdata = new JustbokEntities();
            int memberId = Convert.ToInt32(Memberid);
            int membership = Convert.ToInt32(membershipid);
            int receiptno = Convert.ToInt32(receiptnumber);
            var invoice = (from m in db.MemberInfoes
                           join s in db.MemberShips on m.MemberID equals s.MemberID
                           join p in db.Payments on s.MembershipId equals p.MembershipId

                           select new
                           {
                               FirstName = m.FirstName + " " + m.LastName,
                               Address = m.MemberAddress,
                               Package = s.MembershipType,
                               StartDate = s.StartDate.ToString(),
                               EndDate = s.EndDate.ToString(),
                               Amount = s.Amount,
                               Membershipid = s.MembershipId,
                               RecieptNo = p.RecieptNumber
                           }).ToList();

            var result = invoice.Find(s => (s.Membershipid == membership) && (s.RecieptNo == receiptno));


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnrolledMembershipDay()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledMembers(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdate=DateTime.Now;
            var EnrolledMemberslist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= tdate && s.EndDate<=tdate

                               select new
                               {
                                   MemberID = m.MemberID,
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   MembershipId = s.MembershipId,
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   EndDate = s.EndDate.ToString(),
                                   Amount = s.Amount,
                                   PaymentAmount = p.PaymentAmount,
                                   Note = s.Note
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledMemberslist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledMemberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult MembershipPaymentsDay()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult MembershipPaymentsWeek()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult MembershipPaymentsMonth()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult BindMembership()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberShip = (from gp in db.MembershipOffers
                              where gp.GymId == gymid
                              select new
                              {
                                  MembershipOfferId = gp.MembershipOfferId,
                                  //MemershipType = gp.OfferName + " " + gp.Amount + " Months " + "(" + gp.Amount + ")"
                                  MemershipType = gp.OfferName 
                              }).ToList();
            return Json(memberShip, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffList(int branchid)
        {

            var staffList = (from s in db.Staffs
                             join sb in db.StaffBranches on s.StaffId equals sb.StaffId
                             where sb.BranchId == branchid
                             select new {
                                 StaffId = s.StaffId,
                               FirstName=  s.FirstName,
                               LastName = s.LastName

                             }).ToList();
                            

            return Json(staffList, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnrolledMembershipWeek()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledMembersWeek(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            var EnrolledMemberslist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisWeekStart && s.EndDate <= thisWeekEnd

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           StartDate = s.StartDate.ToString(),
                                           EndDate = s.EndDate.ToString(),
                                           Amount = s.Amount,
                                           PaymentAmount = p.PaymentAmount,
                                           Note = s.Note
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledMemberslist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledMemberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnrolledMembershipMonth()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledMembersMonth(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            var EnrolledMemberslist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisMonthStart && s.EndDate <= thisMonthEnd

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           StartDate = s.StartDate.ToString(),
                                           EndDate = s.EndDate.ToString(),
                                           Amount = s.Amount,
                                           PaymentAmount = p.PaymentAmount,
                                           Note = s.Note
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledMemberslist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledMemberslist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledPaymentsDay(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdate = DateTime.Now;
            var EnrolledPaymentlist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= tdate && s.EndDate <= tdate

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           RecieptNumber = p.RecieptNumber,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           Representative = m.Representative.ToString(),
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           Amount = s.Amount.ToString(),
                                           PaymentDate = p.PaymentDate.ToString(),
                                           PaymentAmount = p.PaymentAmount,
                                           PaymentType = p.PaymentType,
                                           PaymentDueDate = p.PaymentDueDate.ToString(),
                                           Note = s.Note
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledPaymentsWeek(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            var EnrolledPaymentlist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisWeekStart && s.EndDate <= thisWeekEnd

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           RecieptNumber = p.RecieptNumber,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           Representative = m.Representative.ToString(),
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           Amount = s.Amount.ToString(),
                                           PaymentDate = p.PaymentDate.ToString(),
                                           PaymentAmount = p.PaymentAmount,
                                           PaymentType = p.PaymentType,
                                           PaymentDueDate = p.PaymentDueDate.ToString(),
                                           Note = s.Note
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnrolledPaymentsMonth(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            var EnrolledPaymentlist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisMonthStart && s.EndDate <= thisMonthEnd

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           RecieptNumber = p.RecieptNumber,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           Representative = m.Representative.ToString(),
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           Amount = s.Amount.ToString(),
                                           PaymentDate = p.PaymentDate.ToString(),
                                           PaymentAmount = p.PaymentAmount,
                                           PaymentType = p.PaymentType.ToString(),
                                           PaymentDueDate = p.PaymentDueDate.ToString(),
                                           Note = s.Note
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult SearchMemberList(int? page, float pagesize, string membername, string membershipid, string startdate, string todate, string representative, string category,int branchid)
        {

            int memberId = 0;
            string memberName = "";
            int membershipId = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            //if (membershipid != "")
            //{
            //    membershipId = Convert.ToInt32(membershipid);
            //}
           
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
                                  where ((mi.FirstName + " " + mi.LastName).Contains(memberName)) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      MembershipId = ms.MembershipId,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      Note = ms.Note

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
                                  where mi.MemberID == memberId && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      MembershipId = ms.MembershipId,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      Note = ms.Note

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
                //Membership type or representative not null
                if (membershipid != "---Select---" || representative != "---Select---")
                {
                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where ms.StartDate >= startDate && ms.EndDate <= endDate && mi.GymId == gymid && mi.BranchId == branchid && (ms.MembershipType.Contains(membershipid) || mi.Representative.ToLower().Equals(representative.ToLower()))

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          MobileNumber = mi.MobileNumber,
                                          MembershipId = ms.MembershipId,
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount,
                                          Note = ms.Note,
                                          Representative = mi.Representative

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
                    //if only date available 
                else
                {

                    var memberlist = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join p in db.Payments on ms.MembershipId equals p.MembershipId
                                      where ms.StartDate >= startDate && ms.EndDate <= endDate && mi.GymId == gymid && mi.BranchId == branchid 

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName + " " + mi.LastName,
                                          MobileNumber = mi.MobileNumber,
                                          MembershipId = ms.MembershipId,
                                          Package = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = p.PaymentAmount,
                                          Note = ms.Note,
                                          Representative = mi.Representative

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

             
            }
            

            else
            {
                var memberlist = (from mi in db.MemberInfoes
                                  join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                  join p in db.Payments on ms.MembershipId equals p.MembershipId
                                  where (ms.MembershipId == membershipId
                                  || ms.StartDate >= startDate || ms.EndDate <= endDate ||mi.Representative==representative) && mi.GymId == gymid && mi.BranchId == branchid

                                  select new
                                  {
                                      MemberID = mi.MemberID,
                                      FirstName = mi.FirstName + " " + mi.LastName,
                                      MobileNumber = mi.MobileNumber,
                                      MembershipId = ms.MembershipId,
                                      Package = ms.MembershipType,
                                      StartDate = ms.StartDate.ToString(),
                                      EndDate = ms.EndDate.ToString(),
                                      Amount = ms.Amount,
                                      PaymentAmount = p.PaymentAmount,
                                      Note = ms.Note

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
        public JsonResult SearchSalesList(int? page, float pagesize, string membername, string membershipid, string startdate, string todate, string paymentmode, string category, int branchid)
        {

            int memberId = 0;
            string memberName = "";
            int membershipId = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            //if (membershipid != "")
            //{
            //    membershipId = Convert.ToInt32(membershipid);
            //}

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

                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where ((m.FirstName + " " + m.LastName).Contains(memberName)) && m.GymId == gymid && m.BranchId == branchid
                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               RecieptNumber = p.RecieptNumber,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               Representative = m.Representative.ToString(),
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               Amount = s.Amount.ToString(),
                                               PaymentDate = p.PaymentDate.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentType = p.PaymentType,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                               Note = s.Note
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }

            else if (memberId != 0)
            {

                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where m.GymId == gymid && m.BranchId == branchid && m.MemberID==memberId
                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               RecieptNumber = p.RecieptNumber,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               Representative = m.Representative.ToString(),
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               Amount = s.Amount.ToString(),
                                               PaymentDate = p.PaymentDate.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentType = p.PaymentType,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                               Note = s.Note
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            else if (startdate != "")
            {

                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }

                if (membershipid != "---Select---" || paymentmode != "---Select---")
                {

                    var EnrolledPaymentlist = (from m in db.MemberInfoes
                                               join s in db.MemberShips on m.MemberID equals s.MemberID
                                               join p in db.Payments on s.MembershipId equals p.MembershipId
                                               where s.StartDate >= startDate && s.EndDate <= endDate && m.GymId == gymid && m.BranchId == branchid && (s.MembershipType.Contains(membershipid) || p.PaymentType.ToLower().Equals(paymentmode.ToLower()))

                                               select new
                                               {
                                                   MemberID = m.MemberID,
                                                   RecieptNumber = p.RecieptNumber,
                                                   FirstName = m.FirstName + " " + m.LastName,
                                                   MobileNumber = m.MobileNumber,
                                                   Representative = m.Representative.ToString(),
                                                   MembershipId = s.MembershipId,
                                                   Package = s.MembershipType,
                                                   Amount = s.Amount.ToString(),
                                                   PaymentDate = p.PaymentDate.ToString(),
                                                   PaymentAmount = p.PaymentAmount,
                                                   PaymentType = p.PaymentType,
                                                   PaymentDueDate = p.PaymentDueDate.ToString(),
                                                   Note = s.Note
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var EnrolledPaymentlist = (from m in db.MemberInfoes
                                               join s in db.MemberShips on m.MemberID equals s.MemberID
                                               join p in db.Payments on s.MembershipId equals p.MembershipId
                                               where s.StartDate >= startDate && s.EndDate <= endDate && m.GymId == gymid && m.BranchId == branchid 

                                               select new
                                               {
                                                   MemberID = m.MemberID,
                                                   RecieptNumber = p.RecieptNumber,
                                                   FirstName = m.FirstName + " " + m.LastName,
                                                   MobileNumber = m.MobileNumber,
                                                   Representative = m.Representative.ToString(),
                                                   MembershipId = s.MembershipId,
                                                   Package = s.MembershipType,
                                                   Amount = s.Amount.ToString(),
                                                   PaymentDate = p.PaymentDate.ToString(),
                                                   PaymentAmount = p.PaymentAmount,
                                                   PaymentType = p.PaymentType,
                                                   PaymentDueDate = p.PaymentDueDate.ToString(),
                                                   Note = s.Note
                                               }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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
                    return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where m.GymId == gymid && m.BranchId == branchid && ((m.FirstName + " " + m.LastName).ToLower().Trim().Contains(memberName) || s.StartDate >= startDate || s.EndDate <= endDate || s.MembershipId == membershipId || p.PaymentType.Contains(paymentmode))

                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               RecieptNumber = p.RecieptNumber,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               Representative = m.Representative.ToString(),
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               Amount = s.Amount.ToString(),
                                               PaymentDate = p.PaymentDate.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentType = p.PaymentType,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                               Note = s.Note
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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
                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnquiryDetailsDay()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

          //[OutputCache(Duration = 60)]
        public JsonResult EnquiryDayList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdate = DateTime.Now;
            var enquirylist = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate >= tdate && e.EnquiryDate <= tdate

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName+" "+e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnquiryDetailsMonth()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
          //[OutputCache(Duration = 60)]
        public JsonResult EnquiryMonthList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            var enquirylist = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate >= thisMonthStart && e.EnquiryDate <= thisMonthEnd

                               select new
                               {
                                   EnquiryId = e.EnquiryId,
                                   Name = e.FirstName + " " + e.LastName,
                                   MobileNumber = e.MobileNumber,
                                   EnquiryDate = e.EnquiryDate.ToString(),
                                   NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                   LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                   EnqStatus = f.EnqStatus.ToString(),
                                   Note = e.Notes.ToString()
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult EnquiryDetailsWeek()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

//[OutputCache(CacheProfile = "tenMin")] 
        public JsonResult EnquiryWeekList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            var enquirylist = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where e.GymId == gymid && e.BranchId == BranchId && e.EnquiryDate >= thisWeekStart && e.EnquiryDate <= thisWeekEnd

                               select new
                               {
                                   EnquiryId = e.EnquiryId,
                                   Name = e.FirstName + " " + e.LastName,
                                   MobileNumber = e.MobileNumber,
                                   EnquiryDate = e.EnquiryDate.ToString(),
                                   NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                   LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                   EnqStatus = f.EnqStatus.ToString(),
                                   Note = e.Notes.ToString()
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult DuePaymentsDay()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

         //[OutputCache(CacheProfile = "tenMin")] 
        public JsonResult DuePaymentDayList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdate = DateTime.Now;
            var Paymentlist = (from m in db.MemberInfoes
                                       join s in db.MemberShips on m.MemberID equals s.MemberID
                                       join p in db.Payments on s.MembershipId equals p.MembershipId
                                       where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= tdate && s.EndDate <= tdate

                                       select new
                                       {
                                           MemberID = m.MemberID,
                                           FirstName = m.FirstName + " " + m.LastName,
                                           MobileNumber = m.MobileNumber,
                                           MembershipId = s.MembershipId,
                                           Package = s.MembershipType,
                                           StartDate = s.StartDate.ToString(),
                                           Amount = s.Amount.ToString(),
                                           PaymentAmount = p.PaymentAmount,
                                           PaymentDueDate = p.PaymentDueDate.ToString(),
                                       }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(Paymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = Paymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult DuePaymentsWeek()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        //[OutputCache(CacheProfile = "tenMin")] 
        public JsonResult DuePaymentWeekList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            var Paymentlist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisWeekStart && s.EndDate <= thisWeekEnd

                               select new
                               {
                                   MemberID = m.MemberID,
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   MembershipId = s.MembershipId,
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   Amount = s.Amount.ToString(),
                                   PaymentAmount = p.PaymentAmount,
                                   PaymentDueDate = p.PaymentDueDate.ToString(),
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(Paymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = Paymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        //[OutputCache(CacheProfile = "tenMin")] 
        public ActionResult DuePaymentsMonth()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

          //[OutputCache(Duration = 60)]
        public JsonResult DuePaymentMonthList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime baseDate = DateTime.Today;
            var thisMonthStart = baseDate.AddDays(1 - baseDate.Day);
            var thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            var Paymentlist = (from m in db.MemberInfoes
                               join s in db.MemberShips on m.MemberID equals s.MemberID
                               join p in db.Payments on s.MembershipId equals p.MembershipId
                               where m.GymId == gymid && m.BranchId == BranchId && s.StartDate >= thisMonthStart && s.EndDate <= thisMonthEnd

                               select new
                               {
                                   MemberID = m.MemberID,
                                   FirstName = m.FirstName + " " + m.LastName,
                                   MobileNumber = m.MobileNumber,
                                   MembershipId = s.MembershipId,
                                   Package = s.MembershipType,
                                   StartDate = s.StartDate.ToString(),
                                   Amount = s.Amount.ToString(),
                                   PaymentAmount = p.PaymentAmount,
                                   PaymentDueDate = p.PaymentDueDate.ToString(),
                               }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(Paymentlist.Count / pagesize);
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

            return Json(new { Pages = pages, Result = Paymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
     
        public JsonResult SearchDuePaymentsList(int? page, float pagesize, string membername, string startdate, string todate, int branchid)
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

                var Paymentlist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == branchid && ( (m.FirstName.ToLower()+" "+m.LastName.ToLower()).Contains(memberName.ToLower()))

                                   select new
                                   {
                                       MemberID = m.MemberID,
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       MembershipId = s.MembershipId,
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate.ToString(),
                                       Amount = s.Amount.ToString(),
                                       PaymentAmount = p.PaymentAmount,
                                       PaymentDueDate = p.PaymentDueDate.ToString(),
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(Paymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = Paymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }

            else if (memberId != 0)
            {

                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where m.GymId == gymid && m.BranchId == branchid && m.MemberID == memberId 

                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               StartDate = s.StartDate.ToString(),
                                               Amount = s.Amount.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            else if (startdate != "")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }

                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where m.GymId == gymid && m.BranchId == branchid && s.StartDate >= startDate && s.EndDate <= endDate

                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               StartDate = s.StartDate.ToString(),
                                               Amount = s.Amount.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }


            else
            {
                var EnrolledPaymentlist = (from m in db.MemberInfoes
                                           join s in db.MemberShips on m.MemberID equals s.MemberID
                                           join p in db.Payments on s.MembershipId equals p.MembershipId
                                           where m.GymId == gymid && m.BranchId == branchid && ((m.FirstName + " " + m.LastName).ToLower().Trim().Contains(memberName) || s.StartDate >= startDate || s.EndDate <= endDate)

                                           select new
                                           {
                                               MemberID = m.MemberID,
                                               FirstName = m.FirstName + " " + m.LastName,
                                               MobileNumber = m.MobileNumber,
                                               MembershipId = s.MembershipId,
                                               Package = s.MembershipType,
                                               StartDate = s.StartDate.ToString(),
                                               Amount = s.Amount.ToString(),
                                               PaymentAmount = p.PaymentAmount,
                                               PaymentDueDate = p.PaymentDueDate.ToString(),
                                           }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(EnrolledPaymentlist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = EnrolledPaymentlist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
     
        public JsonResult SearchEnquiryDay(int? page,float pagesize,string membername,string gender,string filter,string startdate,string todate,string status,string membership,string recievedby,int branchid)
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
                if (filter.ToLower().Trim() == "enquiry date")
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid && ((e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(memberName) && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate)

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else if(filter.ToLower().Trim() == "next followUp date")
                {
                    var enquirylist = (from e in db.Enquiries
                                   join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                   where e.GymId == gymid && e.BranchId == branchid && ((e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(memberName) && f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate)

                                   select new
                                   {
                                       EnquiryId = e.EnquiryId,
                                       Name = e.FirstName + " " + e.LastName,
                                       MobileNumber = e.MobileNumber,
                                       EnquiryDate = e.EnquiryDate.ToString(),
                                       NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                       LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                       EnqStatus = f.EnqStatus.ToString(),
                                       Note = e.Notes.ToString()
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                  var enquirylist = (from e in db.Enquiries
                                   join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                   where e.GymId == gymid && e.BranchId == branchid && ((e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(memberName) )

                                   select new
                                   {
                                       EnquiryId = e.EnquiryId,
                                       Name = e.FirstName + " " + e.LastName,
                                       MobileNumber = e.MobileNumber,
                                       EnquiryDate = e.EnquiryDate.ToString(),
                                       NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                       LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                       EnqStatus = f.EnqStatus.ToString(),
                                       Note = e.Notes.ToString()
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
            }
            else if (memberId != 0)
            {
                if(filter.ToLower().Trim() == "next followUp date")
                {
                var enquirylist = (from e in db.Enquiries
                                   join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                   where e.GymId == gymid && e.BranchId == branchid && e.EnquiryId == memberId && f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate

                                   select new
                                   {
                                       EnquiryId = e.EnquiryId,
                                       Name = e.FirstName + " " + e.LastName,
                                       MobileNumber = e.MobileNumber,
                                       EnquiryDate = e.EnquiryDate.ToString(),
                                       NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                       LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                       EnqStatus = f.EnqStatus.ToString(),
                                       Note = e.Notes.ToString()
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else if (filter.ToLower().Trim() == "enquiry date")
                {
                  var enquirylist = (from e in db.Enquiries
                                   join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                     where e.GymId == gymid && e.BranchId == branchid && e.EnquiryId == memberId && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate

                                   select new
                                   {
                                       EnquiryId = e.EnquiryId,
                                       Name = e.FirstName + " " + e.LastName,
                                       MobileNumber = e.MobileNumber,
                                       EnquiryDate = e.EnquiryDate.ToString(),
                                       NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                       LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                       EnqStatus = f.EnqStatus.ToString(),
                                       Note = e.Notes.ToString()
                                   }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid && e.EnquiryId == memberId 

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                }
            }
            else if(startdate!="")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                if (gender != "---Select---" || status != "---Select---" || membership != "---Select---" || recievedby != "---Select---")
                {
                    if(filter.ToLower().Trim() == "next followUp date")
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid && f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate &&
                                       (e.Gender.ToLower().Equals(gender.ToLower()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                         e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                    }
                    else if (filter.ToLower().Trim() == "enquiry date")
                    {
                        var enquirylist = (from e in db.Enquiries
                                           join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                           where e.GymId == gymid && e.BranchId == branchid && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate &&
                                           (e.Gender.ToLower().Trim().Equals(gender.ToLower().Trim()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                             e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                           select new
                                           {
                                               EnquiryId = e.EnquiryId,
                                               Name = e.FirstName + " " + e.LastName,
                                               MobileNumber = e.MobileNumber,
                                               EnquiryDate = e.EnquiryDate.ToString(),
                                               NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                               LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                               EnqStatus = f.EnqStatus.ToString(),
                                               Note = e.Notes.ToString()
                                           }).ToList();

                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                        return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var enquirylist = (from e in db.Enquiries
                                           join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                           where e.GymId == gymid && e.BranchId == branchid && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate &&
                                           (e.Gender.ToLower().Equals(gender.ToLower()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                             e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                           select new
                                           {
                                               EnquiryId = e.EnquiryId,
                                               Name = e.FirstName + " " + e.LastName,
                                               MobileNumber = e.MobileNumber,
                                               EnquiryDate = e.EnquiryDate.ToString(),
                                               NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                               LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                               EnqStatus = f.EnqStatus.ToString(),
                                               Note = e.Notes.ToString()
                                           }).ToList();

                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                        return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                    }



                   
                }
                else
                {
                    if (filter.ToLower().Trim() == "next followUp date")
                    {
                        var enquirylist = (from e in db.Enquiries
                                           join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                           where e.GymId == gymid && e.BranchId == branchid && f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate &&
                                           (e.Gender.ToLower().Equals(gender.ToLower()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                             e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                           select new
                                           {
                                               EnquiryId = e.EnquiryId,
                                               Name = e.FirstName + " " + e.LastName,
                                               MobileNumber = e.MobileNumber,
                                               EnquiryDate = e.EnquiryDate.ToString(),
                                               NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                               LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                               EnqStatus = f.EnqStatus.ToString(),
                                               Note = e.Notes.ToString()
                                           }).ToList();

                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                        return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                    }
                    else if (filter.ToLower().Trim() == "enquiry date")
                    {
                        var enquirylist = (from e in db.Enquiries
                                           join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                           where e.GymId == gymid && e.BranchId == branchid && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate &&
                                           (e.Gender.ToLower().Equals(gender.ToLower()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                             e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                           select new
                                           {
                                               EnquiryId = e.EnquiryId,
                                               Name = e.FirstName + " " + e.LastName,
                                               MobileNumber = e.MobileNumber,
                                               EnquiryDate = e.EnquiryDate.ToString(),
                                               NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                               LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                               EnqStatus = f.EnqStatus.ToString(),
                                               Note = e.Notes.ToString()
                                           }).ToList();

                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                        return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var enquirylist = (from e in db.Enquiries
                                           join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                           where e.GymId == gymid && e.BranchId == branchid && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate &&
                                           (e.Gender.ToLower().Equals(gender.ToLower()) || f.EnqStatus.ToLower().Equals(status.ToLower()) || e.ProgramSuggested.ToLower().Contains(membership.ToLower()) ||
                                             e.RecievedBy.ToLower().Equals(recievedby.ToLower()))

                                           select new
                                           {
                                               EnquiryId = e.EnquiryId,
                                               Name = e.FirstName + " " + e.LastName,
                                               MobileNumber = e.MobileNumber,
                                               EnquiryDate = e.EnquiryDate.ToString(),
                                               NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                               LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                               EnqStatus = f.EnqStatus.ToString(),
                                               Note = e.Notes.ToString()
                                           }).ToList();

                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                        return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
                    }
                }


            
            }
            else
            {
                if (filter.ToLower().Trim() == "enquiry date")
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid || (e.Gender == gender || (e.EnquiryDate >= startDate && e.EnquiryDate <= endDate) || f.EnqStatus.ToLower().Trim().Equals(status)
                                       || e.ProgramSuggested.ToLower().Trim().Contains(membership.ToLower().Trim()) || e.RecievedBy.ToLower().Trim().Contains(recievedby.ToLower().Trim()))

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                }
                else if (filter.ToLower().Trim() == "next followup date")
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid || (e.Gender == gender || (f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate) || f.EnqStatus.ToLower().Trim().Equals(status)
                                       || e.ProgramSuggested.ToLower().Trim().Contains(membership.ToLower().Trim()) || e.RecievedBy.ToLower().Trim().Contains(recievedby.ToLower().Trim()))

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid || (e.Gender == gender || e.EnquiryDate >= startDate || e.EnquiryDate <= endDate || f.EnqStatus.ToLower().Trim().Equals(status)
                                       || e.ProgramSuggested.ToLower().Trim().Contains(membership.ToLower().Trim()) || e.RecievedBy.ToLower().Trim().Contains(recievedby.ToLower().Trim()))

                                       select new
                                       {
                                           EnquiryId = e.EnquiryId,
                                           Name = e.FirstName + " " + e.LastName,
                                           MobileNumber = e.MobileNumber,
                                           EnquiryDate = e.EnquiryDate.ToString(),
                                           NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                           LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                           EnqStatus = f.EnqStatus.ToString(),
                                           Note = e.Notes.ToString()
                                       }).ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    double pages = Math.Ceiling(enquirylist.Count / pagesize);
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

                    return Json(new { Pages = pages, Result = enquirylist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidatePassword(string pwd)
        {
            var result = "error";
            var uname = System.Web.HttpContext.Current.Session["UserName"];
            if (uname != null && pwd!=null)
            {
                var Validate = (from gymLogin in db.GymLogins
                                where gymLogin.UserName == uname && gymLogin.Password == pwd
                                select new
                                {
                                    PlaneNameId = gymLogin.UserName
                                }).ToList();
                if (Validate.Count > 0)
                {
                    result = "success";
                }
              
            }

          

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Branch(int BranchId)
        {
            System.Web.HttpContext.Current.Session["BranchId"] = BranchId;
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")]
        public ActionResult NewDashBoard(int branchId)
        {
            if (branchId > 0)
            {
                var memberMeasuremt = MeasurementList(branchId);
                var followup = FollowupTillTodayAsync(branchId);
                var latestMemberList = LatestMembersList(branchId);
                var anniversaryList = AnniverysaryList(branchId);
                var membershipDue = MembershipDueList(branchId);
                var membershipExpired = MembershipExpired(branchId);
                var membershipDuePayment = MembershipDuePayment(branchId);
                var birthdayList = BirthDayList(branchId);
                var totalDueAmount = MembershipDuePayment(branchId);
                var todayDueAmount = TodayDueAmount(branchId);
                var weekDueAmount = WeekDueAmount(branchId);
                var monthDueAmount = MonthDueAmount(branchId);
                var todaySoldMembership = TodaySoldMembership(branchId);
                var weeksoldMembership = WeekSoldMembership(branchId);
                var monthsoldMembership = MonthSoldMembership(branchId);
                var enquiryDayList = TodayEnquiryDetails(branchId);
                var enquiryWeekList = WeekEnquiryDetails(branchId);
                var enquiryMonthList = MonthEnquiryDetails(branchId);

                return Json(new { 
                    NewMeasurement = memberMeasuremt, 
                    NewFollowup = followup,
                    LatestMember=latestMemberList,
                    Anniversary=anniversaryList,
                    MembershipDue = membershipDue,
                    MembersExpired = membershipExpired,
                    MembershipDuePayment = membershipDuePayment,
                    Birthday=birthdayList,
                    TotalDueAmount = totalDueAmount,
                    TodayDueAmount = todayDueAmount,
                    WeekDueAmount = weekDueAmount,
                    MonthDueAmount = monthDueAmount,
                    TodaySoldMenership = todaySoldMembership,
                    WeekSoldMenership = weeksoldMembership,
                    MonthSoldMenership = monthsoldMembership,
                    EnquiryDayList = enquiryDayList,
                    EnquiryWeekList = enquiryWeekList,
                    EnquiryMonthList = enquiryMonthList

                }, 
                    JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }



        private async Task<dynamic> MeasurementList(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            //var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
          
                var lstMeasurement = (from mi in db.MemberInfoes
                                      join ms in db.Measurements on mi.MemberID equals ms.MemberID
                                      where mi.GymId == gymid && mi.BranchId == branchId

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MobileNumber = mi.MobileNumber,
                                          NextMeasurementDate = ms.NextMeasurementDate,
                                          MeasurementDate = ms.MeasurementDate
                                      }).ToList();

                return lstMeasurement;
        }

        private async Task<dynamic> FollowupTillTodayAsync(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
            
                var lstFollowups = (from e in db.Enquiries
                                    join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                    where e.GymId == gymid && f.NextFollowUpDate <= tdayDate
                                    orderby f.NextFollowUpDate descending

                                    select new
                                    {
                                        EnquiryId = e.EnquiryId,
                                        FollowupId = f.FollowupId,
                                        NextFollowUpDate = f.NextFollowUpDate,
                                        FirstName = e.FirstName,
                                        LastName = e.LastName,
                                        MobileNumber = e.MobileNumber,
                                        Gender = e.Gender
                                    }).ToList();

                return lstFollowups;
           
        }

        private async Task<dynamic> LatestMembersList(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
                var lstLatestMembers = (from mi in db.MemberInfoes
                                        where mi.GymId == gymid && mi.BranchId == branchId
                                        orderby mi.MemberID descending

                                        select new
                                        {
                                            MemberID = mi.MemberID,
                                            FirstName = mi.FirstName,
                                            LastName = mi.LastName,
                                            Email = mi.Email,
                                            MobileNumber = mi.MobileNumber,
                                            Gender = mi.Gender
                                        }).ToList();
                return lstLatestMembers;
        }

        private async Task<dynamic> AnniverysaryList(int branchId)
        { 
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
                var lstAnniversary = (from mi in db.MemberInfoes
                                      where mi.GymId == gymid && mi.BranchId == branchId && (mi.AnniversaryDate.Value.Month == tdayDate.Month && mi.AnniversaryDate.Value.Day >= tdayDate.Day)
                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          AnniversaryDate = mi.AnniversaryDate,
                                          MobileNumber = mi.MobileNumber,
                                          Gender = mi.Gender
                                      }).ToList();
                return lstAnniversary;
        }

        private async Task<dynamic> MembershipDueList(int branchId)
        { 
         int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
                var lstMembershipsDue = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                         where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount
                                         orderby ms.StartDate descending

                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate,
                                             EndDate = ms.EndDate,
                                             Amount = ms.Amount,
                                             PaymentAmount = pa.PaymentAmount

                                         }).ToList();
                return lstMembershipsDue;
        }

        private async Task<dynamic> MembershipExpired(int branchId)
        { 
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
                var lstMembershipsExpired = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         where mi.GymId == gymid && mi.BranchId == branchId && ms.EndDate < tdayDate
                                         orderby ms.EndDate descending
                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate,
                                             EndDate = ms.EndDate,
                                             Amount = ms.Amount,
                                         }).ToList();
                return lstMembershipsExpired;
        }

        private async Task<dynamic> MembershipDuePayment(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
                var lstMembershipsDue = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                         where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount
                                         orderby ms.StartDate descending
                                         select new
                                         {
                                             MemberID = mi.MemberID,
                                             FirstName = mi.FirstName,
                                             LastName = mi.LastName,
                                             MembershipId = ms.MembershipId,
                                             MembershipType = ms.MembershipType,
                                             StartDate = ms.StartDate,
                                             EndDate = ms.EndDate,
                                             Amount = ms.Amount,
                                             PaymentAmount = pa.PaymentAmount,
                                             PaymentDueDate = pa.PaymentDueDate

                                         }).ToList();
                return lstMembershipsDue;

        }
        private async Task<dynamic> BirthDayList(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
            // string weekMembershipSold = "0";
                var lstbday = (from mi in db.MemberInfoes
                               where mi.GymId == gymid && mi.BranchId == branchId && (mi.Dob.Value.Month == tdayDate.Month && mi.Dob.Value.Day >= tdayDate.Day)

                               select new
                               {
                                   MemberID = mi.MemberID,
                                   FirstName = mi.FirstName,
                                   LastName = mi.LastName,
                                   Dob = mi.Dob,
                                   MobileNumber = mi.MobileNumber,
                                   Gender = mi.Gender

                               }).ToList();
                return lstbday;

        }
        //private async Task<dynamic> TotalDueAmount(int branchId)
        //{

        //    //use MembershipDuePayment()
        //}
        private async Task<dynamic> TodayDueAmount(int branchId)
        {
             int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
            //var branchId = Convert.ToInt32(BranchId);
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate == tdayDate
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();
                return lstDuePayments;

        }
        private async Task<dynamic> WeekDueAmount(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
            //var branchId = Convert.ToInt32(BranchId);
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate >= weekdays
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();
                return lstDuePayments;
        }
        private async Task<dynamic> MonthDueAmount(int branchId)
        {
             int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
            //var branchId = Convert.ToInt32(BranchId);
                var lstDuePayments = (from mi in db.MemberInfoes
                                      join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                      join pa in db.Payments on ms.MembershipId equals pa.MembershipId
                                      where mi.GymId == gymid && mi.BranchId == branchId && pa.PaymentAmount < ms.Amount && pa.PaymentDueDate >= month
                                      orderby ms.StartDate descending

                                      select new
                                      {
                                          MemberID = mi.MemberID,
                                          FirstName = mi.FirstName,
                                          LastName = mi.LastName,
                                          MembershipId = ms.MembershipId,
                                          MembershipType = ms.MembershipType,
                                          StartDate = ms.StartDate.ToString(),
                                          EndDate = ms.EndDate.ToString(),
                                          Amount = ms.Amount,
                                          PaymentAmount = pa.PaymentAmount

                                      }).ToList();
                return lstDuePayments;

        }
        private async Task<dynamic> TodaySoldMembership(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            //var branchId = Convert.ToInt32(BranchId);
            DateTime tdayDate = DateTime.Now.Date;
                var lstMembershipSold = (from mi in db.MemberInfoes
                                         join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                         where mi.GymId == gymid && ms.StartDate == tdayDate && mi.BranchId == branchId

                                         select new
                                         {
                                             MembershipId = ms.MembershipId,
                                             StartDate = ms.StartDate,
                                             Amount = ms.Amount

                                         }).ToList();
                return lstMembershipSold;
        }
        private async Task<dynamic> WeekSoldMembership(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
            //var branchId = Convert.ToInt32(BranchId);
                var lstWeekMembershipSold = (from mi in db.MemberInfoes
                                             join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                             where mi.GymId == gymid && mi.BranchId == branchId && ms.StartDate >= weekdays

                                             select new
                                             {
                                                 MembershipId = ms.MembershipId,
                                                 StartDate = ms.StartDate,
                                                 Amount = ms.Amount

                                             }).ToList();
                return lstWeekMembershipSold;
        }
        private async Task<dynamic> MonthSoldMembership(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
                var lstWeekMembershipSold = (from mi in db.MemberInfoes
                                             join ms in db.MemberShips on mi.MemberID equals ms.MemberID
                                             where mi.GymId == gymid && mi.BranchId == branchId && ms.StartDate >= month

                                             select new
                                             {
                                                 MembershipId = ms.MembershipId,
                                                 StartDate = ms.StartDate,
                                                 Amount = ms.Amount

                                             }).ToList();
                return lstWeekMembershipSold;

        }
        private async Task<dynamic> TodayEnquiryDetails(int branchId)
        {
             int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime tdayDate = DateTime.Now.Date;
                var lsttdayEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate == tdayDate

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();
                return lsttdayEnquiry;
        }
        private async Task<dynamic> WeekEnquiryDetails(int branchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime weekdays = DateTime.Now.Date.AddDays(-7);
                var lstWeekEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate >= weekdays

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();
                return lstWeekEnquiry;
        }
        private async Task<dynamic> MonthEnquiryDetails(int branchId)
        {
              int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime month = DateTime.Today.AddMonths(-1);
                var lstWeekEnquiry = (from mi in db.Enquiries
                                      where mi.GymId == gymid && mi.EnquiryDate >= month

                                      select new
                                      {
                                          EnquiryId = mi.EnquiryId,
                                          EnquiryDate = mi.EnquiryDate
                                      }).ToList();
                return lstWeekEnquiry;

        }


    }
}
