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
    public class WorkoutPlanController : LayoutBaseModel
    {
        //
        // GET: /WorkoutPlan/

        JustbokEntities db = new JustbokEntities();

        public ActionResult GetWorkoutPlans()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
        //WorkoutPlanNames

        [HttpGet]
        public JsonResult GetWorkoutPlanNameList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var PlanName = (from mi in db.WorkoutPlanNames
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               PlaneNameId = mi.PlaneNameId,
                               PlanName = mi.PlanName,
                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(PlanName.Count / pagesize);
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

            return Json(new { Pages = pages, Result = PlanName.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddWorkoutPlan()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddWorkoutPlan(List<WorkoutPlanViewModel> workoutplan)
        {
            int PlaneNameId=0;

            if (workoutplan != null)
            {
                WorkoutPlanName addWorkoutplanname = new WorkoutPlanName();
              

               // addWorkoutplanname.PlanName=workoutpla

                for (int plan = 0; plan < workoutplan.Count; plan++)
                { 
                    if (plan == 0 && workoutplan[plan].PlanName != null)
                    {
                        addWorkoutplanname.PlanName = workoutplan[plan].PlanName;
                        addWorkoutplanname.BranchId = workoutplan[plan].BranchId;
                        addWorkoutplanname.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                        db.WorkoutPlanNames.Add(addWorkoutplanname);
                        db.SaveChanges();
                        PlaneNameId = addWorkoutplanname.PlaneNameId;
                    }
                    else
                    {
                        
                    }

                    if (plan > 0)
                    {
                        WorkoutPlan addworkoutplan = new WorkoutPlan();
                        addworkoutplan.Workout = workoutplan[plan].Workout;
                        if (workoutplan[plan].SetMin!=null && workoutplan[plan].SetMin.ToLower().Trim() == "sets")
                        {
                            addworkoutplan.NumberOfSets = workoutplan[plan].NumberOfSets;
                            addworkoutplan.Repeats = workoutplan[plan].Repeats;
                        }
                        else if (workoutplan[plan].SetMin != null && workoutplan[plan].SetMin.ToLower().Trim() == "minutes")
                        {
                            addworkoutplan.NumberOfMinutes = workoutplan[plan].NumberOfSets;
                        }
                        //addworkoutplan.NumberOfMinutes = workoutplan[plan].NumberOfMinutes;
                        addworkoutplan.NumberofDays = workoutplan[plan].NumberofDays;
                        addworkoutplan.ExcerciseOrder = workoutplan[plan].ExcerciseOrder;
                        addworkoutplan.PlaneNameId = PlaneNameId;
                        db.WorkoutPlans.Add(addworkoutplan);
                        db.SaveChanges();
                    }
                
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditWorkoutPlan(string id)
        {
            int workoutid = Convert.ToInt32(id);
            System.Web.HttpContext.Current.Session["workoutid"] = workoutid;
           WorkoutPlanName workout = db.WorkoutPlanNames.Find(workoutid);
           if (Request.IsAjaxRequest())
           {
               return PartialView(workout);
           }
           return View(workout);
        }

         [HttpGet]
        public JsonResult BindWorkoutPlan()
        {
            //int workoutid = Convert.ToInt32(id);
            int workoutid = Convert.ToInt32(System.Web.HttpContext.Current.Session["workoutid"]);
            //   WorkoutPlanName workout = db.WorkoutPlanNames.Find(workoutid);

            var workouts = (from mi in db.WorkoutPlanNames
                            join ms in db.WorkoutPlans on mi.PlaneNameId equals ms.PlaneNameId
                            where mi.PlaneNameId == workoutid

                            select new
                            {
                                PlaneNameId = mi.PlaneNameId,
                                PlanName = mi.PlanName,
                                Planid = ms.Planid,
                                Workout = ms.Workout,
                                NumberOfSets = ms.NumberOfSets,
                                NumberOfMinutes = ms.NumberOfMinutes,
                                NumberofDays = ms.NumberofDays,
                                ExcerciseOrder = ms.ExcerciseOrder,
                                Repeats = ms.Repeats
                            }).ToList();
            return Json(workouts, JsonRequestBehavior.AllowGet);
        }

         [HttpPost]
         public JsonResult EditWorkoutPlan(List<WorkoutPlanViewModel> workoutplan)
         {
             if (workoutplan != null)
             {
                 int PlaneNameId = Convert.ToInt32(System.Web.HttpContext.Current.Session["workoutid"]);

                 var workoutlist = (from N in db.WorkoutPlans
                             where N.PlaneNameId==PlaneNameId
                             select new { N.Planid }).ToList();


                 foreach (var workout in workoutlist)
                 {

                     var workoutid = workoutplan.Where(x => x.Planid == workout.Planid).ToList();
                     if (workoutid.Count == 0)
                     {
                         int memberWorkOutId = Convert.ToInt32(workout.Planid);
                         var memberWorkout = db.WorkoutPlans.Where(x => x.Planid == memberWorkOutId).SingleOrDefault();
                         db.WorkoutPlans.Remove(memberWorkout);
                         db.SaveChanges();
                     }
                 }

                 workoutlist = (from N in db.WorkoutPlans
                                where N.PlaneNameId == PlaneNameId
                                select new { N.Planid }).ToList();

                 if (workoutlist.Count > 0)
                 {
                     foreach (var wrkt in workoutplan)
                     {
                         var isExistingWorkout = workoutlist.Where(m => m.Planid == wrkt.Planid).ToList();
                         if (isExistingWorkout.Count > 0)
                         {
                             WorkoutPlan addworkoutplan = new WorkoutPlan();
                             addworkoutplan.Planid = wrkt.Planid;
                             addworkoutplan.Workout = wrkt.Workout;
                             if (wrkt.SetMin != null && wrkt.SetMin.ToLower().Trim() == "sets")
                             {
                                 addworkoutplan.NumberOfSets = wrkt.NumberOfSets;
                                 addworkoutplan.Repeats = wrkt.Repeats;
                             }
                             else
                             {
                                 addworkoutplan.NumberOfMinutes = wrkt.NumberOfSets;
                             }
                             addworkoutplan.NumberofDays = wrkt.NumberofDays;
                             addworkoutplan.ExcerciseOrder = wrkt.ExcerciseOrder;
                             addworkoutplan.Repeats = wrkt.Repeats;
                             addworkoutplan.PlaneNameId = PlaneNameId;
                             db.Entry(addworkoutplan).State = EntityState.Modified;
                             db.SaveChanges();
                         }
                         else
                         {
                             WorkoutPlan addworkoutplan = new WorkoutPlan();
                             addworkoutplan.Workout = wrkt.Workout;
                             if (wrkt.SetMin != null && wrkt.SetMin.ToLower().Trim() == "sets")
                             {
                                 addworkoutplan.NumberOfSets = wrkt.NumberOfSets;
                                 addworkoutplan.Repeats = wrkt.Repeats;
                             }
                             else
                             {
                                 addworkoutplan.NumberOfMinutes = wrkt.NumberOfSets;
                             }
                             addworkoutplan.NumberofDays = wrkt.NumberofDays;
                             addworkoutplan.ExcerciseOrder = wrkt.ExcerciseOrder;
                             addworkoutplan.Repeats = wrkt.Repeats;
                             addworkoutplan.PlaneNameId = PlaneNameId;
                             db.WorkoutPlans.Add(addworkoutplan);
                             db.SaveChanges();
                         }
                     }
                 }
                 else
                 {
                     foreach (var wrkt in workoutplan)
                     {
                         WorkoutPlan addworkoutplan = new WorkoutPlan();
                         addworkoutplan.Workout = wrkt.Workout;
                         if (wrkt.SetMin != null && wrkt.SetMin.ToLower().Trim() == "sets")
                         {
                             addworkoutplan.NumberOfSets = wrkt.NumberOfSets;
                             addworkoutplan.Repeats = wrkt.Repeats;
                         }
                         else
                         {
                             addworkoutplan.NumberOfMinutes = wrkt.NumberOfSets;
                         }
                         addworkoutplan.NumberofDays = wrkt.NumberofDays;
                         addworkoutplan.ExcerciseOrder = wrkt.ExcerciseOrder;
                         addworkoutplan.Repeats = wrkt.Repeats;
                         addworkoutplan.PlaneNameId = PlaneNameId;
                         db.WorkoutPlans.Add(addworkoutplan);
                         db.SaveChanges();
                     
                     }

                   
                 }

                
             }

         

             return Json("Success", JsonRequestBehavior.AllowGet);
         }

         [HttpPost]
         public JsonResult GetWorkoutAutoComplete()
         {
             int gymid=Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
             var CityList = (from N in db.Workouts
                             where N.GymId == gymid 
                             select new { N.WorkoutName }).ToList();
             return Json(CityList, JsonRequestBehavior.AllowGet);  
         }
    }
}
