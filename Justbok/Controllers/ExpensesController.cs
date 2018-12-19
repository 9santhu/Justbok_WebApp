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
    public class ExpensesController : LayoutBaseModel
    {

        public ActionResult Expenses()
        {
            if(Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult ExpensesList(int? page, float pagesize, int ExpenseTypeId,
            string Search, DateTime? FromDate, DateTime? ToDate, string sortBy, string sortDirection,int branchId)
        {
            try
            {
                int gymid = SessionManger.GymId;

                var query = (from Expenses in db.Expenses
                             where Expenses.GymId == gymid
                             && Expenses.IsActive == true
                             && Expenses.BranchId == branchId
                             && (ExpenseTypeId == 0 || Expenses.ExpenseTypeId == ExpenseTypeId)
                             && (string.IsNullOrEmpty(Search) || Expenses.Description.Trim().ToUpper().Contains(Search.Trim().ToUpper()))
                             && DbFunctions.DiffDays(Expenses.ExpenseDate, FromDate) <= 0
                             && DbFunctions.DiffDays(Expenses.ExpenseDate, ToDate) >= 0
                             select new
                             {
                                 ExpensesType = Expenses.ExpensesType.ExpenseTypeName,
                                 ExpensesAmount = Expenses.ExpenseAmount,
                                 ExpensesDate = Expenses.ExpenseDate,
                                 Description = Expenses.Description,
                                 ExpensesId = Expenses.ExpensesId
                             });

                if (sortBy.ToUpper() == "EXPENSESAMOUNT" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.ExpensesAmount);
                }
                else if (sortBy.ToUpper() == "EXPENSESAMOUNT" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.ExpensesAmount);
                }
                else if (sortBy.ToUpper() == "EXPENSESTYPE" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.ExpensesType);
                }
                else if (sortBy.ToUpper() == "EXPENSESTYPE" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.ExpensesType);
                }
                else if (sortBy.ToUpper() == "EXPENSESDATE" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.ExpensesDate);
                }
                else if (sortBy.ToUpper() == "EXPENSESDATE" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.ExpensesDate);
                }
                else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.Description);
                }
                else if (sortBy.ToUpper() == "DESCRIPTION" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.Description);
                }

                var ExpenseList = query.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(ExpenseList.Count / pagesize);

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

                return Json(new { Pages = pages, Result = ExpenseList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExpenseDetailsById(int ExpensesId)
        {
            try
            {
                var ExpenseDetails = (from Expenses in db.Expenses
                                      where Expenses.ExpensesId == ExpensesId
                                      orderby Expenses.ExpenseAmount ascending
                                      select new
                                      {
                                          ExpensesTypeId = Expenses.ExpensesType.ExpenseTypeId,
                                          ExpensesAmount = Expenses.ExpenseAmount,
                                          ExpensesDate = Expenses.ExpenseDate,
                                          Description = Expenses.Description,
                                          ExpensesId = Expenses.ExpensesId,
                                          ReferenceNumber = Expenses.ReferenceNumber,
                                          ExpensesMode = Expenses.ExpenseMode
                                      });

                return Json(ExpenseDetails, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExpenseModes()
        {
            try
            {
                var ExpenseModesList = (from ExpenseModes in db.ExpenseModes
                                        select new
                                        {
                                            ModeId = ExpenseModes.ModeId,
                                            Mode = ExpenseModes.Mode
                                        });

                return Json(ExpenseModesList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveExpense(Expens obExpens)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (obExpens != null)
                {
                    obExpens.GymId = SessionManger.GymId;
                    db.Entry(obExpens).State = obExpens.ExpensesId == 0 ? EntityState.Added : EntityState.Modified;
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

        [HttpPost]
        public JsonResult RemovingExpense(int ExpenseId)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (ExpenseId != 0)
                {
                    Expens objExpense= db.Expenses.Find(ExpenseId);
                    objExpense.IsActive = false;
                    db.Entry(objExpense).State = objExpense.ExpensesId == 0 ? EntityState.Added : EntityState.Modified;
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
    }
}
