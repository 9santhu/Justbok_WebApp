using Justbok.ADModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;

namespace Justbok.Controllers
{
    public class ClassesController : LayoutBaseModel
    {
        public PartialViewResult ClassView()
        {
            return PartialView();
        }

        public JsonResult Classes(int? page, float pagesize, string classname, string description, string sortBy, string sortDirection,int branchId)
        {
            int gymid = SessionManger.GymId;

            var query = (from clas in db.Classes
                         where clas.GymId == gymid
                         && clas.IsActive == true
                         && clas.BranchId== branchId
                         && (string.IsNullOrEmpty(classname) || clas.ClassName.Trim().ToUpper().Contains(classname.Trim().ToUpper()))
                         && (string.IsNullOrEmpty(description) || clas.Description.Trim().ToUpper().Contains(description.Trim().ToUpper()))
                         select clas);

            if (sortBy.ToUpper() == "CLASSNAME" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.ClassName);
            }
            else if (sortBy.ToUpper() == "CLASSNAME" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.ClassName);
            }
            else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Description);
            }
            else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Description);
            }
            else if (sortBy.ToUpper() == "ATTENDENCELIMIT" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.AttendenceLimit);
            }
            else if (sortBy.ToUpper() == "ATTENDENCELIMIT" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.AttendenceLimit);
            }
            else if (sortBy.ToUpper() == "RESERVATIONLIMIT" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.ReservationLimit);
            }
            else if (sortBy.ToUpper() == "RESERVATIONLIMIT" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.ReservationLimit);
            }

            var ClassesList = query.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(ClassesList.Count / pagesize);

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
            return Json(new { Pages = pages, Result = Classes(ClassesList.ToPagedList(pageIndex, (int)pagesize)) }, JsonRequestBehavior.AllowGet);
        }

        private IList<ClassModel> Classes(IPagedList<Class> Classes)
        {
            if (Classes != null && Classes.Count > 0)
            {
                IList<ClassModel> lstClassModel = new List<ClassModel>();

                foreach (Class objClass in Classes)
                {
                    ClassModel objClassModel = new ClassModel();

                    objClassModel.ClassId = objClass.ClassId;
                    objClassModel.ClassName = objClass.ClassName;
                    objClassModel.Description = objClass.Description;
                    objClassModel.AttendenceLimit = objClass.AttendenceLimit.HasValue ? Convert.ToInt32(objClass.AttendenceLimit) : 0;
                    objClassModel.ReservationLimit = objClass.ReservationLimit.HasValue ? Convert.ToInt32(objClass.ReservationLimit) : 0;

                    lstClassModel.Add(objClassModel);
                }

                return lstClassModel;
            }

            return null;
        }

        public JsonResult ClassDetailsById(int ClassId)
        {
            if (ClassId != 0)
            {
                var Class = (from clas in db.Classes
                             where clas.ClassId == ClassId
                             select new
                             {
                                 ClassName = clas.ClassName,
                                 Description = clas.Description,
                                 AttendenceLimit = clas.AttendenceLimit,
                                 ReservationLimit = clas.ReservationLimit,
                                 DropInType = clas.DropInType,
                                 DropInRate = clas.DropInRate,
                                 CalenderColor = clas.CalendarColor
                             }).SingleOrDefault();
                return Json(new { Status = "success", Data = Class }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure", Data = new { } }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveClass(Class objClass)
        {
            if (objClass != null)
            {
                objClass.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                db.Entry(objClass).State = objClass.ClassId == 0 ? System.Data.Entity.EntityState.Added : System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteClass(int ClassId)
        {
            if (ClassId != 0)
            {
                var Class = db.Classes.Find(ClassId);
                Class.IsActive = false;
                db.Entry(Class).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

    }
}
