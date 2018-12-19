using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.ADModel;

namespace Justbok.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        Justbok.Models.JustbokEntities db = new Models.JustbokEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }

        public ActionResult Gym()
        {
            return View();
        }
        public ActionResult Spa()
        {
            return View();
        }


        public ActionResult Saloon()
        {
            return View();
        }


        public ActionResult Yoga()
        {
            return View();
        }



        public ActionResult SessionOut()
        {
            Session.Clear();

            return Redirect("/Login/Index");
        }

        [HttpGet]
        public ActionResult RedirectToHome()
        {

            //System.Web.HttpContext.Current.Session["LoggedGym"] = null;


            //System.Web.HttpContext.Current.Session["GymName"] = null;
            //System.Web.HttpContext.Current.Session["LoggedGym"] = null;
            //System.Web.HttpContext.Current.Session["MemberId"] = null;
            //System.Web.HttpContext.Current.Session["Staffid"] = null;
            //System.Web.HttpContext.Current.Session["Dietid"] = null;
            //System.Web.HttpContext.Current.Session["EnquiryId"] = null;
            //System.Web.HttpContext.Current.Session["EditEnquiryId"] = null;
            //System.Web.HttpContext.Current.Session["FollowupID"] = null;
            //System.Web.HttpContext.Current.Session["GYMID"] = null;
            //System.Web.HttpContext.Current.Session["EditGymID"] = null;
            //System.Web.HttpContext.Current.Session["workoutid"] = null;
            Session.Abandon(); // it will clear the session at the end of request
         
           // Session.Abandon();
            //Session.Clear();

            //return Redirect("/Login/Index");

            return RedirectToAction("Home","Index");
        }

        [HttpPost]
        public JsonResult SearchBoxAutoComplete()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var memberList = (from N in db.MemberInfoes
                            where N.GymId == gymid
                            select new { N.FirstName }).ToList();
            return Json(memberList, JsonRequestBehavior.AllowGet);
        }

    }
}
