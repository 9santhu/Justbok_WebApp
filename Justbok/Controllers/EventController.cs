using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.ADModel;
using Justbok.Models;
using PagedList;

namespace Justbok.Controllers
{
    public class EventController : LayoutBaseModel
    {
        public PartialViewResult EventView()
        {
            return PartialView();
        }

        public PartialViewResult EventLogo()
        {
            return PartialView();
        }

        public JsonResult Events(int? page, float pagesize, string eventname, string description, DateTime? fromDate, DateTime? toDate, int branchId, string sortBy, string sortDirection)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            var query = (from evt in db.Events
                         where evt.GymId == gymid
                         && evt.BranchId == branchId
                         && evt.IsActive == true
                         && (string.IsNullOrEmpty(eventname) || evt.Name.Trim().ToUpper().Contains(eventname.Trim().ToUpper()))
                         && (string.IsNullOrEmpty(description) || evt.Description.Trim().ToUpper().Contains(description.Trim().ToUpper()))
                         && (DbFunctions.DiffDays(evt.StartDate, fromDate) <= 0 && DbFunctions.DiffDays(evt.StartDate, toDate) >= 0)
                         select evt);

            if (sortBy.ToUpper() == "NAME" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Name);
            }
            else if (sortBy.ToUpper() == "NAME" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Name);
            }
            else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Description);
            }
            else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Description);
            }
            else if (sortBy.ToUpper() == "STARTDATE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.StartDate);
            }
            else if (sortBy.ToUpper() == "STARTDATE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.StartDate);
            }
            else if (sortBy.ToUpper() == "ENDDATE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.EndDate);
            }
            else if (sortBy.ToUpper() == "ENDDATE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.EndDate);
            }
            else if (sortBy.ToUpper() == "PRICE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Price);
            }
            else if (sortBy.ToUpper() == "PRICE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Price);
            }
            else if (sortBy.ToUpper() == "REGISTRATIONLIMIT" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.RegistrationLimit);
            }
            else if (sortBy.ToUpper() == "REGISTRATIONLIMIT" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.RegistrationLimit);
            }

            var eventsList = query.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(eventsList.Count / pagesize);

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
            return Json(new { Pages = pages, Result = Events(eventsList.ToPagedList(pageIndex, (int)pagesize)) }, JsonRequestBehavior.AllowGet);
        }

        private IList<EventModel> Events(IPagedList<Event> Events)
        {
            if (Events != null && Events.Count > 0)
            {
                IList<EventModel> lstEvents = new List<EventModel>();

                foreach (Event objEvent in Events)
                {
                    EventModel objEventModel = new EventModel();

                    objEventModel.EventId = objEvent.EventId;
                    objEventModel.Name = objEvent.Name;
                    objEventModel.Description = objEvent.Description;
                    objEventModel.StartDate = objEvent.StartDate;
                    objEventModel.EndDate = objEvent.EndDate;
                    objEventModel.IsAllDay = objEvent.IsAllDay.HasValue ? Convert.ToBoolean(objEvent.IsAllDay) : false;
                    objEventModel.IncludedTax = objEvent.IncludedTax.HasValue ? Convert.ToBoolean(objEvent.IncludedTax) : false;
                    objEventModel.Price = objEvent.Price.HasValue ? Convert.ToDecimal(objEvent.Price) : 0;
                    objEventModel.RegistrationLimit = objEvent.RegistrationLimit.HasValue ? Convert.ToInt32(objEvent.RegistrationLimit) : 0;
                    objEventModel.PhotoUrl = objEvent.PhotoUrl;

                    lstEvents.Add(objEventModel);
                }

                return lstEvents;
            }

            return null;
        }

        public JsonResult SaveEvent(Event objEvent)
        {
            if (objEvent != null)
            {
                objEvent.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                if (objEvent.EventId == 0)
                {
                    objEvent.CreatedOn = DateTime.Now;
                    db.Entry(objEvent).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    objEvent.ModifiedOn = DateTime.Now;
                    db.Entry(objEvent).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EventDetailsById(int EventId)
        {
            if (EventId != 0)
            {
                var Class = (from evt in db.Events
                             where evt.EventId == EventId
                             select new
                             {
                                 Name = evt.Name,
                                 Description = evt.Description,
                                 StartDate = evt.StartDate,
                                 EndDate = evt.EndDate,
                                 IsAllDay = evt.IsAllDay,
                                 IncludedTax = evt.IncludedTax,
                                 Price = evt.Price,
                                 RegistrationLimit = evt.RegistrationLimit,
                                 PhotoUrl = evt.PhotoUrl,
                                 CreatedOn = evt.CreatedOn
                             }).SingleOrDefault();
                return Json(new { Status = "success", Data = Class }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure", Data = new { } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteEvent(int EventId)
        {
            if (EventId != 0)
            {
                var evnt = db.Events.Find(EventId);
                evnt.IsActive = false;
                db.Entry(evnt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadEventImage()
        {
            string _imgname = string.Empty;
            var virtualPath = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["files[]"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Guid.NewGuid();
                    var _ext = Path.GetExtension(pic.FileName);
                    _imgname = fileName + _ext;
                    var _comPath = Path.Combine(Server.MapPath("~/Uploads/Events/"),_imgname);
                    virtualPath=Url.Content("~/Uploads/Events/" + _imgname);
                    pic.SaveAs(_comPath);
                }
            }

            return Json(virtualPath, JsonRequestBehavior.AllowGet);

        }

    }
}
