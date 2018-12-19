using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using Justbok.ADModel;

namespace Justbok.Controllers
{
    public class FollowupController : LayoutBaseModel
    {

        JustbokEntities db = new JustbokEntities();
        //
        // GET: /Followup/

        public ActionResult AddFollowup()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddFollowup(Followup objFollowup)
        {
            objFollowup.EnquiryId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]);
           // db.Followups.Add(objFollowup);
            db.Entry(objFollowup).State = objFollowup.FollowupId == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["FollowupID"] = objFollowup.FollowupId;
            //ViewBag.FollowupList = GetFollowupData( Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]));
            return Json(new { success = true });
        }

        public ActionResult EditFollowup()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        private List<EnquiryViewModel> GetFollowupData(int enquiryId)
        {
            List<EnquiryViewModel> lstMemberData = new List<EnquiryViewModel>();
            JustbokEntities objFollowupdata = new JustbokEntities();
            int enquiryID = enquiryId;
            try
            {
                lstMemberData = (from enquiry in objFollowupdata.Enquiries
                                 join followup in objFollowupdata.Followups on enquiry.EnquiryId equals followup.EnquiryId
                                 where followup.EnquiryId == enquiryID
                                 select new EnquiryViewModel
                                 {
                                     FirstName = enquiry.FirstName,
                                     LastName = enquiry.LastName,
                                     MobileNumber = enquiry.MobileNumber,
                                     EnquiryDate = enquiry.EnquiryDate,
                                     FollowupId = followup.FollowupId,
                                     LastFollowUpDate = followup.LastFollowUpDate,
                                     NextFollowUpDate = followup.NextFollowUpDate,
                                     StartTime = followup.StartTime,
                                     EndTime = followup.EndTime,
                                     EnqStatus = followup.EnqStatus,
                                     Description = followup.Description
                                 }).ToList();
            }
            catch (Exception ex)
            {

            }
            return lstMemberData;
        }

        [HttpGet]
        public JsonResult EnquiryDetails()
        {

            int enquiryId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]);
            var lstFollowup = (from enquiry in db.Enquiries
                               where enquiry.EnquiryId == enquiryId
                               select new
                               {
                                   FirstName = enquiry.FirstName,
                                   LastName = enquiry.LastName,
                                   MobileNumber = enquiry.MobileNumber,
                                   EnquiryDate = enquiry.EnquiryDate.ToString()

                               }).ToList();

            return Json(lstFollowup, JsonRequestBehavior.AllowGet);

        }
   
        [HttpGet]
        public JsonResult FollowupDetails()
        {

            int enquiryid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]);
            var lstFollowup = (from followup in db.Followups
                               where followup.EnquiryId == enquiryid
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
        public ActionResult EditEnquiryDetails()
        {
            ViewBag.Representatvie =EnquiryController.PopulateRepresentative();
            ViewBag.Categoery = EnquiryController.PopulateCategory();
            ViewBag.Offers = EnquiryController.PopulateOffers();
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
            EnquiryViewModel upEnquiry = EnquiryController.EditUpdateEnquiryDetails(enquiry);
            return View(upEnquiry);
        }

        [HttpPost]
        public ActionResult EditEnquiryDetails(EnquiryViewModel enquriy)
        {
            Enquiry objEnquiryDetails = new Enquiry();
            objEnquiryDetails =EnquiryController.UpdateEnquiryDetails(enquriy);
            db.Entry(objEnquiryDetails).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EnquiryList","Enquiry");
        }


        //private List<Followup> GetFollowupData()
        //{
        //    List<Followup > lstMemberData = new List<Followup >();
        //    JustbokEntities objFollowupdata = new JustbokEntities();
        //    int followupId = Convert.ToInt32(System.Web.HttpContext.Current.Session["FollowupID"]);
        //    int enquiryID = Convert.ToInt32(System.Web.HttpContext.Current.Session["EnquiryId"]);
        //    try
        //    {
        //        lstMemberData = (from followup in objFollowupdata.Followups
        //                         where followup.EnquiryId == enquiryID
        //                         select followup).ToList<Followup>();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return lstMemberData;
        //}

    }
}
