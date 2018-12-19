using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Justbok.Models;
using PagedList;
using Justbok.ADModel;


namespace Justbok.Controllers
{
    public class ProductController : LayoutBaseModel
    {
        //
        // GET: /Product/
        JustbokEntities db = new JustbokEntities();

        public ActionResult GetProducts()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetProductList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var product = (from mi in db.Products
                           where mi.GymId == gymid && mi.BranchId==BranchId
                           select new
                           {
                               ProductId = mi.ProductId,
                               BrandName = mi.BrandName,
                               ProductName = mi.ProductName,
                               Price = mi.Price,
                               LowStockQuantity = mi.LowStockQuantity,
                               Description = mi.Description,
                               ForSale = mi.ForSale

                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(product.Count / pagesize);
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

            return Json(new { Pages = pages, Result = product.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult AddProduct()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddProduct(Product objProduct)
        {
            Product addProduct = new Product();
            addProduct.BrandName = objProduct.BrandName;
            addProduct.ProductName = objProduct.ProductName;
            addProduct.Price = objProduct.Price;
            addProduct.Quantity = objProduct.Quantity;
            addProduct.Description = objProduct.Description;
            addProduct.LowStockQuantity = objProduct.LowStockQuantity;
            addProduct.BranchId = objProduct.BranchId;
            addProduct.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            addProduct.ForSale = objProduct.ForSale;
            db.Products.Add(addProduct);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditProduct(string id)
        
{
            int productid = Convert.ToInt32(id);
            Product prdcut = db.Products.Find(productid);
            if (Request.IsAjaxRequest())
            {
                return PartialView(prdcut);
            }
            return View(prdcut);
        }


        //[HttpPost]
        //public ActionResult EditProduct(Product  product)
        //{
        //    db.Entry(product).State = EntityState.Modified;
        //    db.SaveChanges();

        //    return RedirectToAction("GetProducts");
        //}

        [HttpPost]
        public JsonResult EditProduct(Product objProduct)
        {
            //GetMemberInfoDetails
            Product addProduct = new Product();
            addProduct.ProductId = objProduct.ProductId;
            addProduct.BrandName = objProduct.BrandName;
            addProduct.ProductName = objProduct.ProductName;
            addProduct.Price = objProduct.Price;
            addProduct.Quantity = objProduct.Quantity;
            addProduct.Description = objProduct.Description;
            addProduct.LowStockQuantity = objProduct.LowStockQuantity;
            addProduct.BranchId = objProduct.BranchId;
            addProduct.ForSale = objProduct.ForSale;
            addProduct.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(addProduct).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult DeleteProduct(string Productid)
        {
            int productid = Convert.ToInt32(Productid);
            var product = db.Products.Where(x => x.ProductId == productid).SingleOrDefault();

            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }



    }
}
