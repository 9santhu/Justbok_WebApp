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
    public class CategoryController : LayoutBaseModel
    {
        //
        // GET: /Category/
        JustbokEntities db = new JustbokEntities();
        public ActionResult GetCategory()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetCategoryList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var category = (from mi in db.Categories
                           where mi.GymId == gymid && mi.BranchId == BranchId
                           select new
                           {
                               CategoryId = mi.CategoryId,
                               CategoryName = mi.CategoryName,
                               CategoryDescription = mi.CategoryDescription,
                               Active = mi.Active,
                            
                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(category.Count / pagesize);
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

            return Json(new { Pages = pages, Result = category.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCategory()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddCategory(Category objCategory)
        {
            Category addCategory = new Category();
            addCategory.CategoryName = objCategory.CategoryName;
            addCategory.CategoryDescription = objCategory.CategoryDescription;
            addCategory.Active = objCategory.Active;
            addCategory.BranchId = objCategory.BranchId;
            addCategory.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Categories.Add(addCategory);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editcategory(string id)
        {
            int categoryid = Convert.ToInt32(id);
            Category category = db.Categories.Find(categoryid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(category);
            }
            return View(category);
        }


        [HttpPost]
        public JsonResult Editcategory(Category objCategory)
        {
            Category addCategory = new Category();
            addCategory.CategoryId = objCategory.CategoryId;
            addCategory.CategoryName = objCategory.CategoryName;
            addCategory.CategoryDescription = objCategory.CategoryDescription;
            addCategory.Active = objCategory.Active;
            addCategory.BranchId = objCategory.BranchId;
            addCategory.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(addCategory).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCategory(string CategoryId)
        {
            int categoryid = Convert.ToInt32(CategoryId);
            var category = db.Categories.Where(x => x.CategoryId == categoryid).SingleOrDefault();

            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


    }
}
