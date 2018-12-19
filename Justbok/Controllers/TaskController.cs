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
    public class TaskController : LayoutBaseModel
    {
        //
        // GET: /Task/
        JustbokEntities db = new JustbokEntities();
        public ActionResult TaskList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        [HttpGet]
        public JsonResult GetTaskList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var Tasks = (from mi in db.Tasks
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               TaskId = mi.TaskId,
                               Title = mi.Title,
                               TaskDescription = mi.TaskDescription,
                               Interval = mi.Interval,
                               IntervalType = mi.IntervalType,
                               StartDate = mi.StartDate.ToString(),
                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(Tasks.Count / pagesize);
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

            return Json(new { Pages = pages, Result = Tasks.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddTask()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddTask(Task  objTask)
        {
            Task addTask = new Task();
            addTask.Title = objTask.Title;
            addTask.TaskDescription = objTask.TaskDescription;
            addTask.Interval = objTask.Interval;
            addTask.IntervalType = objTask.IntervalType;
            addTask.StartDate = objTask.StartDate;
            addTask.BranchId = objTask.BranchId;
            addTask.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Tasks.Add(addTask);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTask(string id)
        {
            int taskid = Convert.ToInt32(id);
            Task task = db.Tasks.Find(taskid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(task);
            }
            return View(task);
        }


        [HttpPost]
        public JsonResult EditTask(Task objTask)
        {
            Task addTask = new Task();
            addTask.TaskId = objTask.TaskId;
            addTask.Title = objTask.Title;
            addTask.TaskDescription = objTask.TaskDescription;
            addTask.Interval = objTask.Interval;
            addTask.IntervalType = objTask.IntervalType;
            addTask.StartDate = objTask.StartDate;
            addTask.BranchId = objTask.BranchId;
            addTask.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(addTask).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTask(string TaskId)
        {
            int taskid = Convert.ToInt32(TaskId);
            var tasks = db.Tasks.Where(x => x.TaskId == taskid).SingleOrDefault();

            if (tasks != null)
            {
                db.Tasks.Remove(tasks);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
