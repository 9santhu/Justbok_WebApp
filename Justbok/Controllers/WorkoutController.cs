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
    public class WorkoutController : LayoutBaseModel
    {
        //
        // GET: /Workout/

        JustbokEntities db = new JustbokEntities();

        public ActionResult WorkoutList(int? page)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetWorkoutList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var workouts = (from mi in db.Workouts
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               WorkoutId = mi.WorkoutId,
                               WorkoutName = mi.WorkoutName,
                               Unit = mi.Unit,

                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(workouts.Count / pagesize);
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

            return Json(new { Pages = pages, Result = workouts.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddWorkout()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddWorkout(Workout workout)
        {
            Workout addworkout = new Workout();
            addworkout.WorkoutName = workout.WorkoutName;
            addworkout.Unit = workout.Unit;
            addworkout.BranchId = workout.BranchId;
            addworkout.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Workouts.Add(addworkout);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditWorkout(string id)
        {
            int workoutid = Convert.ToInt32(id);
            Workout workout = db.Workouts.Find(workoutid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(workout);
            }
            return View(workout);
        }


        [HttpPost]
        public JsonResult EditWorkout(Workout workout)
        {
            Workout addworkout = new Workout();
            addworkout.WorkoutId = workout.WorkoutId;
            addworkout.WorkoutName = workout.WorkoutName;
            addworkout.Unit = workout.Unit;
            addworkout.BranchId = workout.BranchId;
            addworkout.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(addworkout).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWorkout(string WorkoutId)
        {
            int workoutid = Convert.ToInt32(WorkoutId);
            var workout = db.Workouts.Where(x => x.WorkoutId == workoutid).SingleOrDefault();

            if (workout != null)
            {
                db.Workouts.Remove(workout);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}
