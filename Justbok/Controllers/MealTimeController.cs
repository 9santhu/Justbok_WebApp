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
    public class MealTimeController : LayoutBaseModel
    {
        //
        // GET: /MealTime/
        JustbokEntities db = new JustbokEntities();

        public ActionResult MealList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
        //MealTimes

        [HttpGet]
        public JsonResult GetMealList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var MealTime = (from mi in db.MealTimes
                            where mi.GymId == gymid && mi.BranchId == BranchId
                            select new
                            {
                                MealTimeId = mi.MealTimeId,
                                MealTime1 = mi.MealTime1,
                                MealDescription = mi.MealDescription

                            }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(MealTime.Count / pagesize);
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

            return Json(new { Pages = pages, Result = MealTime.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddMealTime()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddMealTime(MealTime mealTime)
        {

            MealTime objMealTime = new MealTime();

            objMealTime.MealTime1 = mealTime.MealTime1;
            objMealTime.MealDescription = mealTime.MealDescription;
            objMealTime.BranchId = mealTime.BranchId;
            objMealTime.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.MealTimes.Add(objMealTime);
            db.SaveChanges();


            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditMealtTime(string id)
        {
            System.Web.HttpContext.Current.Session["MealId"] = id;
            //int mealtimeId = Convert.ToInt32(id);
            //System.Web.HttpContext.Current.Session["MealTimeId"] = mealtimeId;
            //MealTime mealtime = db.MealTimes.Find(mealtimeId);
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();

        }

        public JsonResult BindMeal()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int mealId = Convert.ToInt32(System.Web.HttpContext.Current.Session["MealId"]);
            try
            {
                var meal = (from mi in db.MealTimes
                               where mi.MealTimeId == mealId

                               select new
                               {
                                   MealTimeId = mi.MealTimeId,
                                   MealTime1 = mi.MealTime1,
                                   MealDescription = mi.MealDescription,

                               }).ToList();

                return Json(meal, JsonRequestBehavior.AllowGet);
                //weekMembershipSold = lstWeekMembershipSold.Count.ToString();


            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateMealTime(MealTime mealTime)
        {

            MealTime objMealTime = new MealTime();

            objMealTime.MealTimeId = mealTime.MealTimeId;
            objMealTime.MealTime1 = mealTime.MealTime1;
            objMealTime.MealDescription = mealTime.MealDescription;
            objMealTime.BranchId = mealTime.BranchId;
            objMealTime.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(objMealTime).State = EntityState.Modified;
            db.SaveChanges();


            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteMealTime(string MealTimeId)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {


                int mealTimeId = Convert.ToInt32(MealTimeId);
                //Login
                var mealTime = db.MealTimes.Where(x => x.MealTimeId == mealTimeId).SingleOrDefault();
                if (mealTime != null)
                {
                    db.MealTimes.Remove(mealTime);
                    db.SaveChanges();
                    result = new { Success = "True", Message = "Removed Successfully." };
                }

            }
            catch (Exception ex)
            {
                result = new { Success = "False", Message = "Unable To Save Information." };
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
      

    }
}
