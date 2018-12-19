using Justbok.ADModel;
using Justbok.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Controllers
{
    public class ClassTimeController : LayoutBaseModel
    {
        public PartialViewResult ClassTimesView()
        {
            return PartialView();
        }

        public PartialViewResult AddEditClassTime()
        {
            return PartialView();
        }

        public JsonResult ClassTimings(int? page, float pagesize, int classId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var query = (from ct in db.ClassTimings
                         where ct.ClassId == classId
                         && ct.IsActive == true
                         select new
                         {
                             TimingId = ct.TimingId,
                             DayNames = ct.DayNames,
                             StaffName = ct.Staff.FirstName + " " + ct.Staff.LastName,
                             StartDate = ct.StartDate,
                             EndDate = ct.EndDate,
                             StartTime = ct.StartTime,
                             EndTime = ct.EndTime,
                             Every=ct.Every
                         });
            var TimingList = query.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(TimingList.Count / pagesize);

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
            return Json(new { Pages = pages, Result = TimingList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveClassTiming(ClassTiming objClassTiming)
        {
            if (objClassTiming != null)
            {
                objClassTiming.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                db.Entry(objClassTiming).State = objClassTiming.TimingId == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClassTimingDetailsById(int TimingId)
        {
            if (TimingId != 0)
            {
                var classTiming = (from ct in db.ClassTimings
                                   where ct.TimingId == TimingId
                                   select new
                                   {
                                       TimingId = ct.TimingId,
                                       IsRepeats = ct.IsRepeats,
                                       Every = ct.Every,
                                       DayVals = ct.DayVals,
                                       StaffId = ct.StaffId,
                                       StaffName = ct.Staff.FirstName + " " + ct.Staff.LastName,
                                       StartDate = ct.StartDate,
                                       EndDate = ct.EndDate,
                                       StartTime = ct.StartTime,
                                       EndTime = ct.EndTime
                                   }).SingleOrDefault();

                return Json(classTiming, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteClassTiming(int TimingId)
        {
            if (TimingId != 0)
            {
                var ct = db.ClassTimings.Find(TimingId);
                ct.IsActive = false;
                db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }
    }
}
