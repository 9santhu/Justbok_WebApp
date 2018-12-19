using Justbok.ADModel;
using Justbok.Models;
using NCrontab.Advanced;
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
    public class CalenderController : LayoutBaseModel
    {
        public PartialViewResult AddClassTime()
        {
            return PartialView();
        }

        public PartialViewResult AddPeople()
        {
            return PartialView();
        }

        public PartialViewResult AddEventPeople()
        {
            return PartialView();
        }

        public JsonResult GettingCalendarData(string start, string end, int branchId)
        {
            DateTime FromDate = Convert.ToDateTime(start);
            DateTime ToDate = Convert.ToDateTime(end);

            int GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            var classtimings = (from cs in db.Classes
                                join ct in db.ClassTimings on cs.ClassId equals ct.ClassId
                                where cs.IsActive == true && cs.GymId == GymId
                                && cs.BranchId == branchId
                                && ct.IsActive == true
                                && ((!ct.EndDate.HasValue)
                                    || (DbFunctions.DiffDays(ct.EndDate, ToDate) >= 0 && DbFunctions.DiffDays(ct.EndDate, FromDate) <= 0)
                                    || (DbFunctions.DiffDays(ct.EndDate, ToDate) < 0 && DbFunctions.DiffDays(ct.StartDate, ToDate) > 0))
                                select ct).ToList();

            var Events = (from et in db.Events
                          where et.GymId == GymId
                          && et.BranchId == branchId
                          && et.IsActive == true
                          && DbFunctions.DiffDays(et.StartDate, FromDate) <= 0
                          && DbFunctions.DiffDays(et.StartDate, ToDate) >= 0
                          select et).ToList();


            if ((classtimings != null && classtimings.Count > 0) || (Events != null && Events.Count > 0))
            {
                List<CalendarModel> lstData = new List<CalendarModel>();

                if (classtimings != null && classtimings.Count > 0)
                {
                    foreach (var timing in classtimings)
                    {
                        if (timing != null && timing.Pattern != null)
                        {
                            string strTitle = timing.Class.ClassName;
                            var schedule = CrontabSchedule.Parse(timing.Pattern);
                            DateTime StartDate = timing.StartDate.HasValue ? Convert.ToDateTime(timing.StartDate) > FromDate ? Convert.ToDateTime(timing.StartDate) : FromDate : FromDate;
                            DateTime EndDate = timing.EndDate.HasValue ? Convert.ToDateTime(timing.EndDate).AddDays(1) < ToDate ? Convert.ToDateTime(timing.EndDate).AddDays(1) : ToDate.AddDays(1) : ToDate.AddDays(1);

                            var nextSchdule = schedule.GetNextOccurrences(StartDate, EndDate);

                            foreach (var nextDate in nextSchdule)
                            {
                                CalendarModel objCalendarModel = new CalendarModel();
                                objCalendarModel.ActualId = timing.TimingId;
                                objCalendarModel.StartDate = nextDate;
                                objCalendarModel.StaffId = timing.StaffId.HasValue ? Convert.ToInt32(timing.StaffId) : 0;
                                objCalendarModel.EndDate = nextDate.AddMinutes((double)timing.Duration);
                                objCalendarModel.Title = strTitle;
                                objCalendarModel.Color = "#" + timing.Class.CalendarColor;
                                objCalendarModel.isAllDay = false;
                                objCalendarModel.isClass = true;
                                objCalendarModel.ReservationLimint = Convert.ToInt32(timing.Class.ReservationLimit);
                                objCalendarModel.AttendenceLimit = Convert.ToInt32(timing.Class.AttendenceLimit);
                                lstData.Add(objCalendarModel);
                            }
                        }
                    }
                }

                if (Events != null && Events.Count > 0)
                {
                    foreach (var evt in Events)
                    {
                        if (evt != null)
                        {
                            CalendarModel objCalendarModel = new CalendarModel();
                            objCalendarModel.ActualId = evt.EventId;
                            objCalendarModel.StartDate = Convert.ToDateTime(evt.StartDate);
                            objCalendarModel.EndDate = Convert.ToDateTime(evt.EndDate);
                            objCalendarModel.Title = evt.Name;
                            objCalendarModel.Color = "#3f4c6b";
                            objCalendarModel.isAllDay = Convert.ToBoolean(evt.IsAllDay);
                            objCalendarModel.isClass = false;
                            objCalendarModel.AttendenceLimit = Convert.ToInt32(evt.RegistrationLimit);
                            objCalendarModel.ReservationLimint = Convert.ToInt32(evt.RegistrationLimit);
                            lstData.Add(objCalendarModel);
                        }
                    }
                }
                return Json(lstData, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GettingClasses(int branchId)
        {
            if (branchId != 0)
            {
                int GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                var classes = (from clas in db.Classes
                               where clas.IsActive == true
                               && clas.GymId == GymId
                               && clas.BranchId == branchId
                               select new
                               {
                                   ClassName = clas.ClassName,
                                   ClassId = clas.ClassId
                               }).ToList();

                return Json(classes, JsonRequestBehavior.AllowGet);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GettingCalendarInstructor(int timingId, DateTime CalendarDate, int StaffId,bool isClass)
        {
            if (timingId != 0)
            {
                var instructor = (from CI in db.CalendarInstructors
                                  where CI.TimeId == timingId && DbFunctions.DiffDays(CI.CalendarDate, CalendarDate) == 0
                                  && CI.IsActive == true && CI.IsClass==isClass
                                  select new
                                  {
                                      CalendarId = CI.CalendarId,
                                      FirstName = CI.Staff.FirstName,
                                      LastName = CI.Staff.LastName,
                                      StaffId = CI.StaffId,
                                      TimeId = CI.TimeId,
                                      CalendarDate = CI.CalendarDate
                                  }).SingleOrDefault();

                if (instructor == null && (StaffId != 0 || !isClass))
                {
                    CalendarInstructor objinstructor = new CalendarInstructor();
                    objinstructor.CalendarId = 0;
                    objinstructor.TimeId = timingId;
                    if (isClass)
                    {
                        objinstructor.StaffId = StaffId;
                    }
                    objinstructor.CalendarDate = CalendarDate;
                    objinstructor.IsActive = true;
                    objinstructor.IsClass = isClass;

                    db.Entry(objinstructor).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();

                    instructor = (from CI in db.CalendarInstructors
                                  where CI.TimeId == timingId && DbFunctions.DiffDays(CI.CalendarDate, CalendarDate) == 0
                                  && CI.IsActive == true && CI.IsClass == isClass
                                  select new
                                  {
                                      CalendarId = CI.CalendarId,
                                      FirstName = CI.Staff.FirstName,
                                      LastName = CI.Staff.LastName,
                                      StaffId = CI.StaffId,
                                      TimeId = CI.TimeId,
                                      CalendarDate = CI.CalendarDate
                                  }).SingleOrDefault();
                }

                return Json(instructor, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdatingCalendarInstructor(CalendarInstructor instructor)
        {
            if (instructor != null)
            {
                db.Entry(instructor).State = instructor.CalendarId == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success", CalendarId = instructor.CalendarId }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure", CalendarId = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GettingCalendarMembers(int CalendarId)
        {
            if (CalendarId != 0)
            {
                var members = (from CM in db.CalendarMembers
                               where CM.CalendarId == CalendarId
                               select new
                               {
                                   CMemberId = CM.CMemberId,
                                   FirstName = CM.MemberInfo.FirstName,
                                   LastName = CM.MemberInfo.LastName,
                                   MemberId = CM.MemberId,
                                   Type = CM.Type,
                                   isAttended = CM.isAttended,
                                   isReserved = CM.isReserved
                               }).ToList();

                return Json(members, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SvaingCalendarMember(CalendarMember CMember)
        {
            if (CMember != null)
            {
                db.Entry(CMember).State = CMember.CMemberId == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success", CMemberId = CMember.CMemberId }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure", CMemberId = 0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMember(int CMemberId)
        {
            if (CMemberId != 0)
            {
                var member = db.CalendarMembers.Find(CMemberId);
                if (member != null)
                {
                    db.Entry(member).State = EntityState.Deleted;
                    db.SaveChanges();
                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MarkAsAttended(int CMemberId)
        {
            if (CMemberId != 0)
            {
                var member = db.CalendarMembers.Find(CMemberId);
                if (member != null)
                {
                    member.isAttended = true;
                    db.Entry(member).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEventMember(int CMemberId,int Type)
        {
            if (CMemberId != 0)
            {
                var member = db.CalendarMembers.Find(CMemberId);
                if (member != null)
                {
                    if ((Type == 1 && !Convert.ToBoolean(member.isAttended)) || (Type == 2 && !Convert.ToBoolean(member.isReserved)))
                    {
                        db.Entry(member).State = EntityState.Deleted;
                    }
                    else 
                    {
                        if (Type == 1)
                        {
                            member.isReserved = false;
                        }
                        else
                        {
                            member.isAttended = false;
                        }
                        db.Entry(member).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }
    }
}
