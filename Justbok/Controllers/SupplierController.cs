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
    public class SupplierController : LayoutBaseModel
    {
        //
        // GET: /Supplier/
        JustbokEntities db = new JustbokEntities();

        public ActionResult GetSuppliers()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetSupplierList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var supplier = (from mi in db.Suppliers
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               SupplierId = mi.SupplierId,
                               FirstName = mi.FirstName,
                               LastName = mi.LastName,
                               CompanyName = mi.CompanyName,
                               RegistrationNumber = mi.RegistrationNumber,
                               

                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(supplier.Count / pagesize);
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

            return Json(new { Pages = pages, Result = supplier.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSupplier()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddSupplier(Supplier objSupplier)
        {
            Supplier addSupplier = new Supplier();
            addSupplier.CompanyName = objSupplier.CompanyName;
            addSupplier.RegistrationNumber = objSupplier.RegistrationNumber;
            addSupplier.FirstName = objSupplier.FirstName;
            addSupplier.LastName = objSupplier.LastName;
            addSupplier.Email = objSupplier.Email;
            addSupplier.PhoneNumber = objSupplier.PhoneNumber;
            addSupplier.SupplierAddress = objSupplier.SupplierAddress;
            addSupplier.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            addSupplier.FaxNumber = objSupplier.FaxNumber;
            addSupplier.BranchId = objSupplier.BranchId;
            db.Suppliers.Add(addSupplier);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult EditSupplier(string id)
        {
            int supplierid = Convert.ToInt32(id);
            Supplier supplier = db.Suppliers.Find(supplierid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(supplier);
            }
            return View(supplier);
        }


        [HttpPost]
        public JsonResult EditSupplier(Supplier objSupplier)
        {
            Supplier addSupplier = new Supplier();
            addSupplier.SupplierId = objSupplier.SupplierId;
            addSupplier.CompanyName = objSupplier.CompanyName;
            addSupplier.RegistrationNumber = objSupplier.RegistrationNumber;
            addSupplier.FirstName = objSupplier.FirstName;
            addSupplier.LastName = objSupplier.LastName;
            addSupplier.Email = objSupplier.Email;
            addSupplier.PhoneNumber = objSupplier.PhoneNumber;
            addSupplier.SupplierAddress = objSupplier.SupplierAddress;
            addSupplier.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            addSupplier.FaxNumber = objSupplier.FaxNumber;
            addSupplier.BranchId = objSupplier.BranchId;
            db.Entry(addSupplier).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult DeleteSupplier(string SupplierId)
        {
            int supplierid = Convert.ToInt32(SupplierId);
            var supplier = db.Suppliers.Where(x => x.SupplierId == supplierid).SingleOrDefault();

            if (supplier != null)
            {
                db.Suppliers.Remove(supplier);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}
