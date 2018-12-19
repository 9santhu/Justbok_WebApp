
using Justbok.ADModel;
using Justbok.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Controllers
{
    public class CalendarController : LayoutBaseModel
    {
        public ActionResult AppointmentConfig()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult BookAppointment()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult Classes()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult Events()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public ActionResult Calendar()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
    }
}
