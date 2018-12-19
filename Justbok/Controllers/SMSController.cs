using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.ADModel;
using PagedList;
using Justbok.Models;

namespace Justbok.Controllers
{
    public class SMSController : LayoutBaseModel
    {
        //
        // GET: /SMS/

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult SMSLog()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }



        [HttpGet]
        public JsonResult MembersList(int? page, float pagesize, string TableType, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            if (TableType.ToLower().Equals("allmembers"))
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId

                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       MemberName = m.FirstName + " " + m.LastName,
                                       Email = m.Email,
                                       Address = m.MemberAddress.ToString(),
                                       PhoneNo = m.MobileNumber,
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
            else if (TableType.ToLower().Equals("duemembership"))
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where s.EndDate<DateTime.Today.Date && m.GymId == gymid && m.BranchId == BranchId

                                   select new
                                   {

                                       MemberName = m.FirstName + " " + m.LastName,
                                       Email = m.Email,
                                       Address = m.MemberAddress.ToString(),
                                       PhoneNo = m.MobileNumber,
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
            else if (TableType.ToLower().Equals("expiredmembership"))
            {
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where s.EndDate < DateTime.Today.Date && m.GymId == gymid && m.BranchId == BranchId

                                   select new
                                   {

                                       MemberName = m.FirstName + " " + m.LastName,
                                       Email = m.Email,
                                       Address = m.MemberAddress.ToString(),
                                       PhoneNo = m.MobileNumber,
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
            else if (TableType.ToLower().Equals("allenquiry"))
            {

            }
            else if (TableType.ToLower().Equals("paymentdue"))
            {

            }


            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OnloadBindMemberList(int ? page,float pagesize,int branchid)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberslist = (from m in db.MemberInfoes
                               where m.GymId == gymid && m.BranchId == branchid
                               select new
                               {
                                   MemberId = m.MemberID,
                                   MemberName = m.FirstName + " " + m.LastName,
                                   Email = m.Email,
                                   Address = m.MemberAddress.ToString(),
                                   PhoneNo = m.MobileNumber,
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
        public JsonResult OnloadBindDueMembership(int? page, float pagesize, string fromdate, string todate, int branchid)
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

            if (fromdate != "")
            {
                if (todate.Equals(""))
                {
                    endDate = DateTime.Today;
                }
                if (todate != null && todate.Equals(""))
                {
                    endDate = DateTime.Today;
                }
                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == branchid && s.StartDate>=startDate && s.EndDate<=endDate

                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       MemberName = m.FirstName + " " + m.LastName,
                                       MembershipType = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       MobileNumber = m.MobileNumber
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
                                   where m.GymId == gymid && m.BranchId == branchid && s.EndDate > DateTime.Today

                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       MemberName = m.FirstName + " " + m.LastName,
                                       MembershipType = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       MobileNumber = m.MobileNumber
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

            return Json("",JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OnloadBindExpiredMembership(int? page, float pagesize,string fromdate,string todate, int branchid)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (fromdate != "" && fromdate != null)
            {
                startDate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "" && fromdate != null)
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (fromdate != "")
            {
                if (todate == null)
                {
                    endDate = DateTime.Today;
                }

                if (todate != null && todate.Equals(""))
                {
                    endDate = DateTime.Today;
                }

                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == branchid && s.StartDate>=startDate && s.EndDate < endDate

                                   select new
                                   {
                                       MemberId=m.MemberID,
                                       MemberName = m.FirstName + " " + m.LastName,
                                       MembershipType = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       MobileNumber = m.MobileNumber
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
                                   where m.GymId == gymid && m.BranchId == branchid && s.EndDate < DateTime.Today

                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       MemberName = m.FirstName + " " + m.LastName,
                                       MembershipType = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       MobileNumber = m.MobileNumber
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

            return Json("",JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OnloadBindAllEnquiryList(int? page, float pagesize,string fromdate,string todate, int branchid)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (fromdate != "" && fromdate != null)
            {
                startDate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "" && fromdate != null)
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (fromdate != "")
            {
                if (todate == null)
                {
                    endDate = DateTime.Today;
                }

                if (todate != null && todate.Equals(""))
                {
                    endDate = DateTime.Today;
                }
                var memberslist = (from m in db.Enquiries
                                   join s in db.Followups on m.EnquiryId equals s.EnquiryId
                                   where m.GymId == gymid && m.BranchId == branchid && m.EnquiryDate>=startDate && m.EnquiryDate<=endDate

                                   select new
                                   {
                                       MemberName = m.FirstName + " " + m.LastName,
                                       Address = m.Address,
                                       MobileNumber = m.MobileNumber,
                                       EnquiryDate = m.EnquiryDate,
                                       LastFollowUpDate = s.LastFollowUpDate,
                                       NextFollowUpDate = s.NextFollowUpDate,
                                       EnqStatus = s.EnqStatus
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
                var memberslist = (from m in db.Enquiries
                                   join s in db.Followups on m.EnquiryId equals s.EnquiryId
                                   where m.GymId == gymid && m.BranchId == branchid

                                   select new
                                   {
                                       MemberName = m.FirstName + " " + m.LastName,
                                       Address = m.Address,
                                       MobileNumber = m.MobileNumber,
                                       EnquiryDate = m.EnquiryDate,
                                       LastFollowUpDate = s.LastFollowUpDate,
                                       NextFollowUpDate = s.NextFollowUpDate,
                                       EnqStatus = s.EnqStatus
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

        [HttpGet]
        public JsonResult OnloadBindDuePaymentList(int? page, float pagesize, string fromdate, string todate, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (fromdate != "" && fromdate!=null)
            {
                startDate = DateTime.ParseExact(fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (todate != "" && fromdate != null)
            {
                endDate = DateTime.ParseExact(todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (fromdate != "")
            {
                if (todate==null)
                {
                    endDate = DateTime.Today;
                }

                if (todate != null && todate.Equals(""))
                {
                    endDate = DateTime.Today;
                }

                var memberslist = (from m in db.MemberInfoes
                                   join s in db.MemberShips on m.MemberID equals s.MemberID
                                   join p in db.Payments on s.MembershipId equals p.MembershipId
                                   where m.GymId == gymid && m.BranchId == BranchId && p.PaymentDate>=startDate && p.PaymentDate<=endDate
                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       EnrollDate = m.EnrollDate.ToString(),
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount,
                                       PaymentDueDate = p.PaymentDueDate
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
                                   where m.GymId == gymid && m.BranchId == BranchId

                                   select new
                                   {
                                       MemberId = m.MemberID,
                                       FirstName = m.FirstName + " " + m.LastName,
                                       MobileNumber = m.MobileNumber,
                                       EnrollDate = m.EnrollDate.ToString(),
                                       Package = s.MembershipType,
                                       StartDate = s.StartDate,
                                       EndDate = s.EndDate,
                                       Amount = s.Amount,
                                       PaymentAmount = p.PaymentAmount,
                                       PaymentDueDate = p.PaymentDueDate
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
            
        }

        [HttpGet]
        public JsonResult SaveMessages(string phoneNumber, string Message, string memberid, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            SMSHistory sms = new SMSHistory();
            string[] mobileNumber;
            string[] memberId= new string[0];
            int counter = 0;
            if (phoneNumber != null && phoneNumber != "")
            {
                mobileNumber = phoneNumber.Split(',');
                if (memberid != null && memberid != "")
                {
                    memberId = memberid.Split(',');
                }
               
                foreach(string mob in mobileNumber)
                {
                    
                    sms.PhoneNumber =long.Parse(mob);
                    sms.SMSDate = DateTime.Today;
                    sms.SMSMessage = Message;
                    sms.BranchId = BranchId;
                    sms.GymId = gymid;
                    if (memberId.Count()>0)
                    {
                        sms.MemberId = int.Parse(memberId[counter]);
                    }
                   
                    db.SMSHistories.Add(sms);
                    db.SaveChanges();
                    counter = counter + 1;
                }
            }
           
          return  Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SMSList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var messagelist = (from msg in db.SMSHistories
                               where msg.GymId==gymid && msg.BranchId==BranchId
                               select new
                               {
                                   SmsDate=msg.SMSDate,
                                   Message=msg.SMSMessage,
                                   PhoneNumber=msg.PhoneNumber
                               }).ToList();
              int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(messagelist.Count / pagesize);
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

                return Json(new { Pages = pages, Result = messagelist.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }

    }
}
