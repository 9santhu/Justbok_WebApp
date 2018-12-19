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
    public class DietController : LayoutBaseModel
    {
        //
        // GET: /Diet/
        JustbokEntities db = new JustbokEntities();

        public ActionResult DietPlanList(int? page)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
          
        }

        [HttpGet]
        public JsonResult GetDietPlanList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var DeitPlan = (from mi in db.DietPlanNames
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               DietPlanId = mi.DietPlanId,
                               DietPlanName1 = mi.DietPlanName1,
                             
                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(DeitPlan.Count / pagesize);
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

            return Json(new { Pages = pages, Result = DeitPlan.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }
        //DietPlanNames
        public ActionResult  AddDietPlan()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
          
        }

        [HttpGet]
        public JsonResult BindMealTime(int branchid)
        {

            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var mealtime = (from mi in db.MealTimes
                            where mi.GymId == gymid && mi.BranchId==branchid
                            select new
                            {
                                MealTime1 = mi.MealTime1

                            }).ToList();
            return Json(mealtime, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddDietPlan(List<DietViewModel> dietplan)
        {
            int PlaneNameId = 0;

            if (dietplan != null)
            {
                DietPlanName addDietplanname = new DietPlanName();


                // addWorkoutplanname.PlanName=workoutpla

                for (int plan = 0; plan < dietplan.Count; plan++)
                {
                    if (plan == 0 && dietplan[plan].DietPlanName1 != null)
                    {
                        addDietplanname.DietPlanName1 = dietplan[plan].DietPlanName1;
                        addDietplanname.BranchId = dietplan[plan].BranchId;
                        addDietplanname.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                        db.DietPlanNames.Add(addDietplanname);
                        db.SaveChanges();
                        PlaneNameId = addDietplanname.DietPlanId;
                    }
                    else
                    {

                    }

                    if (plan > 0)
                    {
                        Diet addDiet = new Diet();
                        addDiet.DietTime = dietplan[plan].MealTime1;
                        addDiet.MondayDiet = dietplan[plan].MondayDiet;
                        addDiet.TuesdayDiet = dietplan[plan].TuesdayDiet;
                        addDiet.WednesdayDiet = dietplan[plan].WednesdayDiet;
                        addDiet.ThursdayDiet = dietplan[plan].ThursdayDiet;
                        addDiet.FridayDiet = dietplan[plan].FridayDiet;
                        addDiet.SaturdayDiet = dietplan[plan].SaturdayDiet;
                        addDiet.SundayDiet = dietplan[plan].SundayDiet;
                        addDiet.DietPlanId = PlaneNameId;
                        db.Diets.Add(addDiet);
                        db.SaveChanges();
                    }

                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditDietPlan(string id)
        {
            int dietid = Convert.ToInt32(id);
            System.Web.HttpContext.Current.Session["Dietid"] = dietid;
            DietPlanName workout = db.DietPlanNames.Find(dietid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(workout);
            }
            return View(workout);
        }

        [HttpGet]
        public JsonResult BindEditDiet()
        {
            //int workoutid = Convert.ToInt32(id);
            int dietid = Convert.ToInt32(System.Web.HttpContext.Current.Session["Dietid"]);
            //   WorkoutPlanName workout = db.WorkoutPlanNames.Find(workoutid);

            var diets = (from mi in db.DietPlanNames
                            join ms in db.Diets on mi.DietPlanId equals ms.DietPlanId
                            where mi.DietPlanId == dietid

                            select new
                            {
                                DietPlanId = mi.DietPlanId,
                                DietPlanName1 = mi.DietPlanName1,
                                DietId=ms.DietId,
                                DietTime = ms.DietTime,
                                MondayDiet = ms.MondayDiet,
                                TuesdayDiet = ms.TuesdayDiet,
                                WednesdayDiet = ms.WednesdayDiet,
                                ThursdayDiet = ms.ThursdayDiet,
                                FridayDiet = ms.FridayDiet,
                                SaturdayDiet = ms.SaturdayDiet,
                                SundayDiet = ms.SundayDiet
                                
                            }).ToList();
            return Json(diets, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditDietPlan(List<DietViewModel> diet)
        {
            int PlaneNameId = 0;

            if (diet != null)
            {


                // addWorkoutplanname.PlanName=workoutpla

                for (int plan = 0; plan < diet.Count; plan++)
                {
                    if (diet[plan].DietPlanId != null && diet[plan].DietPlanId != 0 && plan == 1)
                    {

                        DietPlanName addWorkoutplanname = new DietPlanName();
                        addWorkoutplanname.DietPlanId = Convert.ToInt32(diet[plan].DietPlanId);
                        addWorkoutplanname.DietPlanName1 = diet[0].DietPlanName1;
                        addWorkoutplanname.BranchId = diet[0].BranchId;
                        addWorkoutplanname.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                        db.Entry(addWorkoutplanname).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (plan == 1)
                    {
                        PlaneNameId = Convert.ToInt32(diet[plan].DietPlanId);
                    }
                    if (diet[plan].DietPlanId != 0 && diet[plan].DietPlanId!=null)
                    {
                        Diet addworkoutplan = new Diet();
                        addworkoutplan.DietId = diet[plan].DietId;
                        addworkoutplan.DietTime = diet[plan].MealTime1;
                        addworkoutplan.MondayDiet = diet[plan].MondayDiet;
                        addworkoutplan.TuesdayDiet = diet[plan].TuesdayDiet;
                        addworkoutplan.WednesdayDiet = diet[plan].WednesdayDiet;
                        addworkoutplan.ThursdayDiet = diet[plan].ThursdayDiet;
                        addworkoutplan.FridayDiet = diet[plan].FridayDiet;
                        addworkoutplan.SaturdayDiet = diet[plan].SaturdayDiet;
                        addworkoutplan.SundayDiet = diet[plan].SundayDiet;


                        addworkoutplan.DietPlanId = Convert.ToInt32(diet[plan].DietPlanId);
                        db.Entry(addworkoutplan).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (diet[plan].DietPlanId == 0 && plan > 0)
                    {
                        Diet addworkoutplan = new Diet();
                        addworkoutplan.DietTime = diet[plan].DietTime;
                        addworkoutplan.MondayDiet = diet[plan].MondayDiet;
                        addworkoutplan.TuesdayDiet = diet[plan].TuesdayDiet;
                        addworkoutplan.WednesdayDiet = diet[plan].WednesdayDiet;
                        addworkoutplan.ThursdayDiet = diet[plan].ThursdayDiet;
                        addworkoutplan.SaturdayDiet = diet[plan].SaturdayDiet;
                        addworkoutplan.SundayDiet = diet[plan].SundayDiet;
                        addworkoutplan.DietPlanId = PlaneNameId;
                        db.Diets.Add(addworkoutplan);
                        db.SaveChanges();
                    }

                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDiet(string mealid)
        {
            int mealID = Convert.ToInt32(mealid);
            var meal = db.MealTimes.Where(x => x.MealTimeId == mealID).SingleOrDefault();

            if (meal != null)
            {
                db.MealTimes.Remove(meal);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}
