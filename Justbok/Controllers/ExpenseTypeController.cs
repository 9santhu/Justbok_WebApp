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
    public class ExpenseTypeController : LayoutBaseModel
    {
        public ActionResult ExpenseType()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult ExpenseTypes(int? page, float pagesize, string Name,
            string Description, string sortBy, string sortDirection,int branchId)
        {
            try
            {
                int gymid = SessionManger.GymId;

                var query = (from ExpensesTypes in db.ExpensesTypes
                             where ExpensesTypes.GymId == gymid
                             && ExpensesTypes.BranchId == branchId
                             && (string.IsNullOrEmpty(Name) || ExpensesTypes.ExpenseTypeName.Trim().ToUpper().Contains(Name.Trim().ToUpper()))
                             && (string.IsNullOrEmpty(Description) || ExpensesTypes.ExpenseTypeDescription.Trim().ToUpper().Contains(Description.Trim().ToUpper()))
                             select new
                             {
                                 Name = ExpensesTypes.ExpenseTypeName,
                                 Description = ExpensesTypes.ExpenseTypeDescription,
                                 ExpenseTypeId = ExpensesTypes.ExpenseTypeId
                             });

                if (sortBy.ToUpper() == "NAME" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.Name);
                }
                else if (sortBy.ToUpper() == "NAME" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.Name);
                }
                else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.Description);
                }
                else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.Description);
                }

                var ExpenseTypeList = query.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(ExpenseTypeList.Count / pagesize);

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

                return Json(new { Pages = pages, Result = ExpenseTypeList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExpenseTypeDetailsById(int ExpenseTypeId)
        {
            try
            {
                var ExpenseTypeDetails = (from ExpensesTypes in db.ExpensesTypes
                                      where ExpensesTypes.ExpenseTypeId == ExpenseTypeId
                                      select new
                                      {
                                          ExpenseTypeName = ExpensesTypes.ExpenseTypeName,
                                          ExpenseTypeDescription = ExpensesTypes.ExpenseTypeDescription,
                                          ExpenseTypeId = ExpensesTypes.ExpenseTypeId
                                      }).SingleOrDefault();

                return Json(ExpenseTypeDetails, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveExpenseType(ExpensesType objExpensesType)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (objExpensesType != null)
                {
                    objExpensesType.GymId = SessionManger.GymId;
                    db.Entry(objExpensesType).State = objExpensesType.ExpenseTypeId == 0 ? EntityState.Added : EntityState.Modified;
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
        public JsonResult ExpenseTypeList(int branchId)
        {
            try
            {
                int gymid = SessionManger.GymId;
                var ExpensesTypesList = (from ExpensesTypes in db.ExpensesTypes
                                         where ExpensesTypes.GymId == gymid
                                         where ExpensesTypes.BranchId == branchId
                                         select new
                                         {
                                             ExpensesTypeId = ExpensesTypes.ExpenseTypeId,
                                             ExpenseTypeName = ExpensesTypes.ExpenseTypeName
                                         }).ToList();
                return Json(ExpensesTypesList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }

            return Json(new {}, JsonRequestBehavior.AllowGet);

        }

    }
}
