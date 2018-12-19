using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using PagedList;
using System.Data.Entity;
using Justbok.ADModel;
using System.Web.UI;

namespace Justbok.Controllers
{
    public class EnquiryController : LayoutBaseModel
    {
        //
        // GET: /Enquiry/
        JustbokEntities db = new JustbokEntities();
        EnquiryViewModel reprentative = new EnquiryViewModel();
        public ActionResult NewEnquiryForm()
        {

            ViewBag.Representatvie = PopulateRepresentative();
            ViewBag.Categoery = PopulateCategory();
            ViewBag.Offers = PopulateOffers();
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }
        
        [HttpPost]
        public JsonResult NewEnquiryForm(EnquiryViewModel objEnquriy)
        {
            Enquiry objEnquiryDetails = new Enquiry();
            objEnquiryDetails = UpdateEnquiryDetails(objEnquriy);
            db.Enquiries.Add(objEnquiryDetails);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["EnquiryId"] = objEnquiryDetails.EnquiryId;
            TempData["Result"] = "Data saved successfully";
           ViewBag.Representatvie = PopulateRepresentative();

           return Json("Success", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult UpdateEnquiryForm(EnquiryViewModel objEnquriy)
        {
            Enquiry objEnquiryDetails = new Enquiry();
            objEnquiryDetails = UpdateEnquiryDetails(objEnquriy);
            db.Entry(objEnquiryDetails).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public static Enquiry UpdateEnquiryDetails(EnquiryViewModel objNewEnquriy)
        {
            Enquiry objEnquiry = new Enquiry();
           
            try
            {
                objEnquiry.EnquiryId =Convert.ToInt32(objNewEnquriy.EnquiryId);
                objEnquiry.FirstName = objNewEnquriy.FirstName;
                objEnquiry.LastName = objNewEnquriy.LastName;
                objEnquiry.MobileNumber = objNewEnquriy.MobileNumber;
                objEnquiry.DOB = objNewEnquriy.DOB;
                objEnquiry.EnquiryDate  = objNewEnquriy.EnquiryDate;
                objEnquiry.Email = objNewEnquriy.EmailId ;
                //
                objEnquiry.PhoneNumberResidence  = objNewEnquriy.PhoneNumberResidence;
                objEnquiry.PhoneNumberOffice = objNewEnquriy.PhoneNumberOffice;
                objEnquiry.Gender = objNewEnquriy.Gender;
                objEnquiry.Address = objNewEnquriy.Address;
                objEnquiry.Age = objNewEnquriy.Age;
                objEnquiry.Intention = objNewEnquriy.Intention;
                objEnquiry.ProgramSuggested = objNewEnquriy.ProgramSuggested;
                objEnquiry.Category = objNewEnquriy.Category;
                objEnquiry.AmountOffered = objNewEnquriy.AmountOffered;
                objEnquiry.TrailOffered = objNewEnquriy.TrailOffered;
                objEnquiry.TrailDate = objNewEnquriy.TrailDate;
                objEnquiry.HowDidYouKnow = objNewEnquriy.HowDidYouKnow;
                objEnquiry.RecievedBy = objNewEnquriy.RecievedBy;
                objEnquiry.BranchId = objNewEnquriy.BranchId;
                objEnquiry.Notes = objNewEnquriy.Notes;
                objEnquiry.SMS = objNewEnquriy.SMS ? "Yes" : "No";
                objEnquiry.Email = objNewEnquriy.Email ? "Yes" : "No";
                objEnquiry.call = objNewEnquriy.call ? "Yes" : "No";
                objEnquiry.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            }
            catch (Exception ex)
            { 
            
            }
            return objEnquiry;
        
        }


        public static EnquiryViewModel EditUpdateEnquiryDetails(Enquiry objNewEnquriy)
        {
            EnquiryViewModel objEnquiry = new EnquiryViewModel();

            try
            {
                objEnquiry.EnquiryId = objNewEnquriy.EnquiryId;
                objEnquiry.FirstName = objNewEnquriy.FirstName;
                objEnquiry.LastName = objNewEnquriy.LastName;
                objEnquiry.MobileNumber = objNewEnquriy.MobileNumber;
                objEnquiry.DOB = objNewEnquriy.DOB;
                objEnquiry.EnquiryDate = objNewEnquriy.EnquiryDate;
                objEnquiry.EmailId = objNewEnquriy.EmailId;
                objEnquiry.PhoneNumberResidence = objNewEnquriy.PhoneNumberResidence;
                objEnquiry.PhoneNumberOffice = objNewEnquriy.PhoneNumberOffice;
                objEnquiry.Gender = objNewEnquriy.Gender;
                objEnquiry.Address = objNewEnquriy.Address;
                objEnquiry.Age = objNewEnquriy.Age;
                objEnquiry.Intention = objNewEnquriy.Intention;
                objEnquiry.ProgramSuggested = objNewEnquriy.ProgramSuggested;
                objEnquiry.Category = objNewEnquriy.Category;
                objEnquiry.AmountOffered = objNewEnquriy.AmountOffered;
                objEnquiry.TrailOffered = objNewEnquriy.TrailOffered;
                objEnquiry.TrailDate = objNewEnquriy.TrailDate;
                objEnquiry.HowDidYouKnow = objNewEnquriy.HowDidYouKnow;
                objEnquiry.RecievedBy = objNewEnquriy.RecievedBy;
                objEnquiry.Notes = objNewEnquriy.Notes;
                if(objNewEnquriy.SMS.Trim()=="Yes")
                {
                    objEnquiry.SMS = true;
                }
                else
                {
                    objEnquiry.SMS=false;
                }
                if (objNewEnquriy.Email.Trim() == "Yes")
                {
                    objEnquiry.Email = true;
                }
                else
                {
                    objEnquiry.Email = false;
                }
                if (objNewEnquriy.call.Trim() == "Yes")
                {
                    objEnquiry.call = true;
                }
                else
                {
                    objEnquiry.call = false;
                }
                objEnquiry.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            }
            catch (Exception ex)
            {

            }
            return objEnquiry;

        }


        public ActionResult EnquiryList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult GetEnquiryList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var enquiryList = (from e in db.Enquiries
                               join f in db.Followups on e.EnquiryId equals f.EnquiryId
                               where e.GymId == gymid && e.BranchId==BranchId 

                               select new 
                               {
                                   EnquiryId = e.EnquiryId,
                                   FirstName = e.FirstName,
                                   LastName = e.LastName,
                                   MobileNumber = e.MobileNumber,
                                   EnquiryDate = e.EnquiryDate.ToString(),
                                   LastFollowUpDate = f.LastFollowUpDate.ToString(),
                                   NextFollowUpDate = f.NextFollowUpDate.ToString(),
                                   EnqStatus = f.EnqStatus
                               })
                               .Distinct()
                               .ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(enquiryList.Count / pagesize);
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

            return Json(new { Pages = pages, Result = enquiryList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }


       public  static List<string> PopulateRepresentative()
    {
        //List<string> items = new List<SelectListItem>();
       JustbokEntities  objStaff =new JustbokEntities();
           List<string> lstStaffList = new List<string>();
           lstStaffList = (from staff in objStaff.Staffs
                               select (staff.FirstName + " " + staff.LastName)).ToList();
           return lstStaffList;
    }

       public static List<string> PopulateCategory()
       {
           //List<string> items = new List<SelectListItem>();
           JustbokEntities objCategory = new JustbokEntities();
           List<string> lstCategoryList = new List<string>();
           int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           lstCategoryList = (from categoery in objCategory.Categories
                              where categoery.GymId == gymid
                              select (categoery.CategoryName)).ToList();
           return lstCategoryList;
       }

       public static List<string> PopulateOffers()
       {
           //List<string> items = new List<SelectListItem>();
           JustbokEntities objOffers = new JustbokEntities();
           List<string> lstOfferList = new List<string>();
           int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           lstOfferList = (from offer in objOffers.MembershipOffers
                           where offer.GymId == gymid
                           select (offer.OfferName)).ToList();
           return lstOfferList;
       }

       public ActionResult  EditEnquiry(string id)
       {
           int enqid = Convert.ToInt32(id);
           System.Web.HttpContext.Current.Session["EditEnquiryId"] = enqid;
           //ViewBag.FollowupList = GetFollowupData(enqid);
           if (Request.IsAjaxRequest())
           {
               return PartialView();
           }
           return View();
       }

        [HttpGet]
       public  JsonResult GetFollowupData()
       {
           int enquiryID = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditEnquiryId"]);
          
             var  followups = (from followup in db.Followups
                                 where followup.EnquiryId == enquiryID
                                select new 
                                {
                                    FollowupId = followup.FollowupId,
                                    LastFollowUpDate = followup.LastFollowUpDate.ToString(),
                                    NextFollowUpDate = followup.NextFollowUpDate.ToString(),
                                    StartTime = followup.StartTime,
                                    EndTime = followup.EndTime,
                                    EnqStatus = followup.EnqStatus,
                                    Description = followup.Description
                                }).ToList();
           
           return Json(followups, JsonRequestBehavior.AllowGet);
    }

        [HttpGet]
       public ActionResult EditEnquiryDetails()
       {
           ViewBag.Representatvie = PopulateRepresentative();
           ViewBag.Categoery = PopulateCategory();
           ViewBag.Offers = PopulateOffers();
           int EditEnq = 0;
           if (Convert.ToInt32(System.Web.HttpContext.Current.Session["EditEnquiryId"]) != 0)
           {
                EditEnq = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditEnquiryId"]);
           }
           else
           {
               EditEnq = Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]);
           }
          
          Enquiry enquiry = db.Enquiries.Find(EditEnq);
          EnquiryViewModel upEnquiry = EditUpdateEnquiryDetails(enquiry);
          if (Request.IsAjaxRequest())
          {
              return PartialView(upEnquiry);
          }
          return View(upEnquiry);
       }

        [HttpPost]
        public ActionResult EditEnquiryDetails(EnquiryViewModel enquriy)
        {
            Enquiry objEnquiryDetails = new Enquiry();
            objEnquiryDetails = UpdateEnquiryDetails(enquriy);
            db.Entry(objEnquiryDetails).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EnquiryList");
        }

        [HttpGet]
        public JsonResult GetRepresentativeList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var staff = (from representative in db.Staffs
                            where representative.GymId == gymid
                            select new
                            {
                                FirstName = representative.FirstName,
                                LastName = representative.LastName,
                               
                            }).ToList();

            return Json(staff, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult FollowupDetails(string FollowupId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int followupid = Convert.ToInt32(FollowupId);
            var lstFollowup = (from followup in db.Followups
                               where followup.FollowupId == followupid
                         select new
                         {
                             FollowupId = followup.FollowupId,
                             LastFollowUpDate = followup.LastFollowUpDate.ToString(),
                             NextFollowUpDate = followup.NextFollowUpDate.ToString(),
                             StartTime = followup.StartTime,
                             EndTime = followup.EndTime,
                             EnqStatus = followup.EnqStatus,
                             Description = followup.Description

                         }).ToList();

            return Json(lstFollowup, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMemberDetails()
        {
          long  EditEnq = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditEnquiryId"]);
          var staff = (from enquiry in db.Enquiries
                       where enquiry.EnquiryId == EditEnq
                       select new
                       {
                           Name = enquiry.FirstName+" "+enquiry.LastName,
                           MobileNumber = enquiry.MobileNumber,
                           EnquiryDate = enquiry.EnquiryDate.ToString()

                       }).ToList();

          return Json(staff, JsonRequestBehavior.AllowGet);
        }

        
        [HttpPost]
        public JsonResult AddFollowup(EnquiryViewModel objEnquiry)
        {
            Followup objFollowup = new Followup();
            if (objEnquiry != null)
            {
                objFollowup = GetFollowupDetails(objEnquiry);
                //db.MemberShips.Add(objMembership);
                db.Entry(objFollowup).State = objEnquiry.FollowupId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
               
            }
            // Session["MembershipId"] = objMembership.MembershipId;
            return Json(new { success = true });
        }

        private Followup GetFollowupDetails(EnquiryViewModel objEnquiry)
        {
            Followup objFollowup = new Followup();
            try
            {
                objFollowup.FollowupId = objEnquiry.FollowupId;
                objFollowup.LastFollowUpDate = objEnquiry.LastFollowUpDate;
                objFollowup.NextFollowUpDate = objEnquiry.NextFollowUpDate;
                objFollowup.EnqStatus = objEnquiry.EnqStatus;
                objFollowup.Description = objEnquiry.Description;
                objFollowup.StartTime = objEnquiry.StartTime;
                objFollowup.EndTime = objEnquiry.EndTime;
                 objFollowup.EnquiryId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditEnquiryId"]);
            }
            catch (Exception ex)
            {
            }
            return objFollowup;
        }



        [HttpGet]
        public JsonResult GetMembershipOffers()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            var offers = (from offer in db.MembershipOffers
                            where offer.GymId == gymid
                          select new
                          {
                              OfferName = offer.OfferName,
                          }).ToList();

            return Json(offers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchMemberList(int? page, float pagesize, string membername, string gender, string filter, string startdate, string todate, string status, string membership, string recievedby, int branchid)
        {
            long memberId = 0;
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
                memberId = long.Parse(membername);
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
                else if (filter.ToLower().Trim() == "next followUp date")
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
                                       where e.GymId == gymid && e.BranchId == branchid && ((e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(memberName))

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
                if (filter.ToLower().Trim() == "next followUp date")
                {
                    var enquirylist = (from e in db.Enquiries
                                       join f in db.Followups on e.EnquiryId equals f.EnquiryId
                                       where e.GymId == gymid && e.BranchId == branchid && e.MobileNumber == memberId && f.NextFollowUpDate >= startDate && f.NextFollowUpDate <= endDate

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
                                       where e.GymId == gymid && e.BranchId == branchid && e.MobileNumber == memberId && e.EnquiryDate >= startDate && e.EnquiryDate <= endDate

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
                                       where e.GymId == gymid && e.BranchId == branchid && e.MobileNumber == memberId

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
            else if (startdate != "")
            {
                if (todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                if (gender != "-----Select-----" || status != "-----Select-----" || membership != "-----Select-----" || recievedby != "-----Select-----")
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

        [HttpGet]
        public JsonResult NewMembershipDetails(int EnquiryId)
        {
            if (EnquiryId > 0)
            {
                TempData["EnquiryMemberInfo"] = EnquiryId;

                //var details = (from enquiry in db.Enquiries
                //               where enquiry.EnquiryId == EnquiryId
                //               select new
                //               {
                //                   FirstName = enquiry.FirstName,
                //                   LastName=enquiry.LastName,
                //                   MobileNumber = enquiry.MobileNumber,
                //                   EnquiryDate = enquiry.EnquiryDate.ToString()
                //               }).ToList();
                //ViewBag.EnquiryMemberInfo = Newtonsoft.Json.JsonConvert.SerializeObject(details);

                //return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


    }
}
