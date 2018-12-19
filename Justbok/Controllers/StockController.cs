using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;
using Justbok.ADModel;

namespace Justbok.Controllers
{
    public class StockController : LayoutBaseModel
    {
        //
        // GET: /Stock/

        JustbokEntities db = new JustbokEntities();

        public ActionResult StockDetails()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
           
        }

        [HttpGet]
        public JsonResult StockDetailsList(int? page, float pagesize, int BranchId)
        { 
              int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
           var  lstProductList = (from product in db.Products
                                           where product.GymId == gymid && product.BranchId==BranchId
                                           select new 
                                           {
                                               ProductId=product.ProductId,
                                                BrandName=product.BrandName,
                                                ProductName=product.ProductName,
                                                Quantity=product.Quantity,
                                           }).ToList();

           int pageIndex = 1;
           pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           double pages = Math.Ceiling(lstProductList.Count / pagesize);
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

           return Json(new { Pages = pages, Result = lstProductList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

    [HttpPost]
        public JsonResult UpdateStock(List<StockViewModel> objStock)
        {

            if (objStock != null)
            {
                for (int stock = 0; stock < objStock.Count; stock++)
                {
                    if (objStock[stock].ProductId != 0)
                    {
                        Product updateProduct = new Product();
                        if (objStock[stock].StockIn != null)
                        {
                            updateProduct.ProductId = objStock[stock].ProductId;
                            updateProduct.Quantity = objStock[stock].Quantity + objStock[stock].StockIn;
                            db.Products.Attach(updateProduct);
                            // db.Entry(updateProduct).State = EntityState.Modified;
                            db.Entry(updateProduct).Property(x => x.Quantity).IsModified = true;
                            db.SaveChanges();

                            Stock AddStock = new Stock();

                            AddStock.StockIn = objStock[stock].StockIn;
                            AddStock.StockOut = objStock[stock].StockOut;
                            AddStock.ProductId = objStock[stock].ProductId;
                            AddStock.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                            if (objStock[stock].StockIn != null)
                            {
                                AddStock.StockinDate = DateTime.Today;
                            }

                            if (objStock[stock].StockOut != null)
                            {
                                AddStock.StockoutDate = DateTime.Today;
                            }

                            db.Stocks.Add(AddStock);
                            db.SaveChanges();
                        }

                       

                    }
                }
            }
            string ReturnURL = "/Stock/StockDetails";
            return Json(ReturnURL); 
        }

    [HttpPost]
    public JsonResult RemoveStock(List<StockViewModel> objStock)
    {

        if (objStock != null)
        {
            for (int stock = 0; stock < objStock.Count; stock++)
            {
                if (objStock[stock].ProductId != 0)
                {
                    Product updateProduct = new Product();
                    if (objStock[stock].StockOut != null)
                    {
                        updateProduct.ProductId = objStock[stock].ProductId;
                        updateProduct.Quantity = objStock[stock].Quantity - objStock[stock].StockOut;
                        db.Products.Attach(updateProduct);
                        // db.Entry(updateProduct).State = EntityState.Modified;
                        db.Entry(updateProduct).Property(x => x.Quantity).IsModified = true;
                        db.SaveChanges();

                        Stock AddStock = new Stock();

                        AddStock.StockIn = objStock[stock].StockIn;
                        AddStock.StockOut = objStock[stock].StockOut;
                        AddStock.ProductId = objStock[stock].ProductId;
                        AddStock.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                        if (objStock[stock].StockIn != null)
                        {
                            AddStock.StockinDate = DateTime.Today;
                        }

                        if (objStock[stock].StockOut != null)
                        {
                            AddStock.StockoutDate = DateTime.Today;
                        }

                        db.Stocks.Add(AddStock);
                        db.SaveChanges();
                    }



                }
            }
        }
        string ReturnURL = "/Stock/StockDetails";
        return Json(ReturnURL);
    }

        [HttpGet]
    public ActionResult StockInList()
    {
        if (Request.IsAjaxRequest())
        {
            return PartialView();
        }
        return View();

    }

        public JsonResult StockInListDetails(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
        var lststockList = (from p in db.Products
                                                 join s in db.Stocks on p.GymId equals s.GymId
                                                 where p.ProductId == s.ProductId && s.StockIn != null && s.GymId == gymid
                                                 select new StockViewModel
                                                 {
                                                     Manufacture = p.BrandName,
                                                     ProductName = p.ProductName,
                                                     Quantity = p.Quantity,
                                                     StockIn = s.StockIn,
                                                     StockinDate = s.StockinDate
                                                 }).ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lststockList.Count / pagesize);
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

            return Json(new { Pages = pages, Result = lststockList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult StockInList()
        //{
        //    return Redirect("StockOutList");

        //}


          [HttpGet]
    public ActionResult StockOutList(int? page)
    {

        if (Request.IsAjaxRequest())
        {
            return PartialView();
        }
        return View();
    }

          public JsonResult StockOutDetails(int? page, float pagesize, int BranchId)
          {
              int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lststockList = (from p in db.Products
                                                   join s in db.Stocks on p.GymId equals s.GymId
                                                   where p.ProductId == s.ProductId && s.StockOut != null && s.GymId == gymid
                                                   select new StockViewModel
                                                   {
                                                       Manufacture = p.BrandName,
                                                       ProductName = p.ProductName,
                                                       Quantity = p.Quantity,
                                                       StockOut = s.StockOut,
                                                       StockoutDate = s.StockoutDate
                                                   }).ToList();

              int pageIndex = 1;
              pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
              double pages = Math.Ceiling(lststockList.Count / pagesize);
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

              return Json(new { Pages = pages, Result = lststockList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
          
          }

          //[HttpPost]
          //public ActionResult StockOutList()
          //{
          //    return Redirect("StockInList");

          //}

    }
}
