using Justbok.Models;
using NCrontab;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Controllers
{
    public class SchedulerController : Controller
    {
        JustbokEntities db = new JustbokEntities();

        public ActionResult Scheduler()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSchedules(string start, string end, string resourceView)
        {
            List<ScheduleEvent> Events = null;
            try
            {
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                DateTime FromDate = Convert.ToDateTime(start);
                DateTime ToDate = Convert.ToDateTime(end);

                var Schedules = (from Scheduler in db.Schedulers
                                 where Scheduler.GymId == gymid
                                 && Scheduler.IsActive == true
                                 && ((DbFunctions.DiffDays(Scheduler.StartDate, FromDate) <= 0
                                 && DbFunctions.DiffDays(Scheduler.EndDate, ToDate) >= 0) ||
                                 ((DbFunctions.DiffDays(Scheduler.StartDate, FromDate) >= 0 || DbFunctions.DiffDays(Scheduler.StartDate, FromDate) <= 0)
                                 && !Scheduler.EndDate.HasValue))
                                 select Scheduler).ToList();
 
                if (Schedules != null)
                {
                    Events = new List<ScheduleEvent>();

                    foreach (var Scheduler in Schedules)
                    {
                        var schedule = CrontabSchedule.Parse(Scheduler.Repeatable);
                        DateTime StartDate = Scheduler.StartDate.HasValue ? Convert.ToDateTime(Scheduler.StartDate) : FromDate;
                        DateTime EndDate = Scheduler.EndDate.HasValue ? Convert.ToDateTime(Scheduler.EndDate).AddDays(1) : ToDate.AddDays(1);

                        var nextSchdule = schedule.GetNextOccurrences(StartDate, EndDate);

                        foreach (var startDate in nextSchdule)
                        {
                            ScheduleEvent objScheduleEvent = new ScheduleEvent();
                            objScheduleEvent.SchedulerId = Scheduler.SchedulerId;
                            objScheduleEvent.StartDate = startDate;
                            objScheduleEvent.EndDate = startDate.AddMinutes((double)Scheduler.Duration);
                            objScheduleEvent.Title = Scheduler.MemberInfo.FirstName + " " + Scheduler.MemberInfo.LastName + " Has Training With " + Scheduler.Staff.FirstName + " " + Scheduler.Staff.LastName;
                            //itm.color = rptEvnt.statusString;
                            Events.Add(objScheduleEvent);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return Json(Events, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMembers()
        {
            try
            {
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                var Members = (from MemberInfo in db.MemberInfoes
                               where MemberInfo.GymId == gymid
                               select new
                               {
                                   FirstName = MemberInfo.FirstName,
                                   LastName = MemberInfo.LastName,
                                   MemberId = MemberInfo.MemberID
                               }).ToList();
                return Json(Members, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetStaff()
        {
            try
            {
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                var Staffs = (from Staff in db.Staffs
                              where Staff.GymId == gymid
                              select new
                              {
                                  FirstName = Staff.FirstName,
                                  LastName = Staff.LastName,
                                  StaffId = Staff.StaffId
                              }).ToList();
                return Json(Staffs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult PostSchedule(Scheduler Scheduler)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (Scheduler != null)
                {
                    Scheduler.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                    db.Entry(Scheduler).State = Scheduler.SchedulerId == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                    result = new { Success = "True", Message = "Success" };
                }
            }
            catch (Exception ex)
            {
                result = new { Success = "False", Message = "Unable To Save Information." };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetScheduleDetailsById(int ScheduleId)
        {
            try
            {
                var Schedule = (from Sched in db.Schedulers
                                 where Sched.SchedulerId == ScheduleId
                                 select Sched
                                 ).SingleOrDefault();

                var Scheduler = new SchedulerViewModel()
                {
                    MemberId = Schedule.MemberId.HasValue ? Convert.ToInt32(Schedule.MemberId) : 0,
                    StaffId = Schedule.StaffId.HasValue ? Convert.ToInt32(Schedule.StaffId) : 0,
                    StartDate = Schedule.StartDate.HasValue ? Convert.ToDateTime(Schedule.StartDate) : DateTime.Now,
                    EndDate = Schedule.EndDate,
                    Duration = Schedule.Duration.HasValue ? Convert.ToInt32(Schedule.Duration) : 0,
                    SelectType = Schedule.SelectType.HasValue ? Convert.ToInt32(Schedule.SelectType) : 0,
                    SelectedDay = Schedule.SelectedDay,
                    Repeatable = Schedule.Repeatable,
                };

                Scheduler.StartDate = GetStartDate(Scheduler.Repeatable, Scheduler.StartDate);

                Scheduler.strStartDate = JsonConvert.SerializeObject(Scheduler.StartDate, new IsoDateTimeConverter());

                return Json(Scheduler, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        private DateTime GetStartDate(string Repeatable, DateTime StartDate)
        {
            var schedule = CrontabSchedule.Parse(Repeatable);

            var nextSchdule = schedule.GetNextOccurrences(StartDate, StartDate.AddDays(1)).ToList();

            foreach (var startDate in nextSchdule)
            {
                StartDate = startDate;
            }

            return StartDate;
        }

        [HttpPost]
        public JsonResult UpdateScheduleDetails(int ScheduleId,int Duration)
        {
            var result = new { Success = "False", Message = "Unable To Update Information." };
            try
            {
                var Scheduler = db.Schedulers.Find(ScheduleId);

                Scheduler.Duration = Duration;
                db.Entry(Scheduler).State = Scheduler.SchedulerId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();

                result = new { Success = "True", Message = "Success" };
            }
            catch (Exception)
            {
                result = new { Success = "False", Message = "Unable To Update Information." };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
