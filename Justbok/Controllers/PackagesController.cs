using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using Justbok.Models;
using Justbok.ADModel;


namespace Justbok.Controllers
{
    public class PackagesController : LayoutBaseModel
    {
        //
        // GET: /Packages/
        JustbokEntities db = new JustbokEntities();

        public ActionResult GetPackages(int? page)
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetPackagesList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var packages = (from mi in db.MembershipOffers
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               MembershipOfferId = mi.MembershipOfferId,
                               OfferName = mi.OfferName,
                               Months = mi.Months,
                               Amount = mi.Amount,
                               MinimumAmount = mi.MinimumAmount,
                               Category = mi.Category,
                               Active = mi.Active,  
                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(packages.Count / pagesize);
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

            return Json(new { Pages = pages, Result = packages.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddPackage()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddPackage(MembershipOffer objPackage)
        {
            MembershipOffer addPackage = new MembershipOffer();
            addPackage.OfferName = objPackage.OfferName;
            addPackage.Months = objPackage.Months;
            addPackage.Amount = objPackage.Amount;
            addPackage.MinimumAmount = objPackage.MinimumAmount;
            addPackage.Category = objPackage.Category;
            addPackage.Active = objPackage.Active;
            addPackage.BranchId = objPackage.BranchId;
            addPackage.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.MembershipOffers.Add(addPackage);
            db.SaveChanges();
            return Json(new { Success = true });
        }

        public ActionResult EditPackage(string id)
        {
           // int packageid = Convert.ToInt32(id);
            System.Web.HttpContext.Current.Session["PackageId"] = id;
         // Convert.ToInt32();
            //MembershipOffer  package = db.MembershipOffers.Find(packageid);
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult BindPackage()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int packageid = Convert.ToInt32(System.Web.HttpContext.Current.Session["PackageId"]);
            try
            {
                var package = (from mi in db.MembershipOffers
                               where mi.MembershipOfferId == packageid

                                      select new
                                      {
                                          MembershipOfferId = mi.MembershipOfferId,
                                          Amount = mi.Amount,
                                          Months = mi.Months,
                                          OfferName = mi.OfferName,
                                          MinimumAmount = mi.MinimumAmount,
                                          Category = mi.Category,
                                          Active = mi.Active

                                      }).ToList();

                return Json(package, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EditPackage(MembershipOffer objPackage)
        {
            MembershipOffer addPackage = new MembershipOffer();
            addPackage.MembershipOfferId = objPackage.MembershipOfferId;
            addPackage.OfferName = objPackage.OfferName;
            addPackage.Months = objPackage.Months;
            addPackage.Amount = objPackage.Amount;
            addPackage.MinimumAmount = objPackage.MinimumAmount;
            addPackage.Category = objPackage.Category;
            addPackage.Active = objPackage.Active;
            addPackage.BranchId = objPackage.BranchId;
            addPackage.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            db.Entry(addPackage).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Success = true });
        }

        public JsonResult DeletePackage(string PackageId)
        {
            int packageid = Convert.ToInt32(PackageId);
            var package = db.MembershipOffers.Where(x => x.MembershipOfferId == packageid).SingleOrDefault();

            if (package != null)
            {
                db.MembershipOffers.Remove(package);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            try
            {
                var category = (from pk in db.Categories
                                where pk.GymId == gymid 

                               select new
                               {
                                   CategoryName = pk.CategoryName,
                                   Active = pk.Active,

                               }).ToList();

                return Json(category, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

      
    }
}
