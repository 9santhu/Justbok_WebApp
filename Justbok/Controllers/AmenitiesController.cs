using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;

namespace Justbok.Controllers
{
    public class AmenitiesController : Controller
    {
        //
        // GET: /Amenities/
        JustbokEntities db = new JustbokEntities();

        public ActionResult AmenitiesList(int? page)
        {
            int pagesize = pagesize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
          //  IPagedList memberlist = null;
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<Amenity> lstAmenityList = (from amenities in db.Amenities
                                           
                                           select amenities).ToList();
            return View(lstAmenityList.ToPagedList(pageIndex, pagesize));
        }

        [HttpPost]
        public JsonResult AddAmenities(Amenity aminities)
        {
            Amenity addAmenity = new Amenity();
            addAmenity.AmenitiesId = aminities.AmenitiesId;
            addAmenity.AmenitiesName = aminities.AmenitiesName;
            db.Entry(addAmenity).State = addAmenity.AmenitiesId == 0 ? EntityState.Added : EntityState.Modified;
           // db.Amenities.Add(addAmenity);
            db.SaveChanges();


            return Json(new {Success=true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditAmenities(string AmenitiesId)
        {
            int amenitiesId = Convert.ToInt32(AmenitiesId);
            var AminityList = (from mi in db.Amenities
                         where mi.AmenitiesId==amenitiesId

                         select new
                         {
                             AmenitiesId = mi.AmenitiesId,
                             AmenitiesName = mi.AmenitiesName,
                         }).ToList();


            return Json(AminityList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteAmenities(string AmenitiesId)
        {
            if (AmenitiesId != null)
            {
            int amenitiesId = Convert.ToInt32(AmenitiesId);
            var amenities = db.Amenities.Where(x => x.AmenitiesId == amenitiesId).SingleOrDefault();
            if (amenities != null)
            {
                db.Amenities.Remove(amenities);
                db.SaveChanges();
            }
            
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


    }
}
