using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using PagedList;
using System.Data.Entity;
using Justbok.ADModel;

namespace Justbok.Controllers
{
    public class POSController : LayoutBaseModel
    {

        public ActionResult PurchaseOrders()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public PartialViewResult NewPurchaseOrder()
        {
            return PartialView();
        }

        public ActionResult GetPurchaseOrders(int? page, float pagesize, string strSearch, DateTime? FromDate, DateTime? ToDate, string sortBy, string sortDirection, int branchId)
        {
            int gymid = SessionManger.GymId;

            var query = (from purchase in db.PurchaseOrderHeaders
                         where purchase.GymId == gymid.ToString()
                         && purchase.BranchId == branchId
                         && (string.IsNullOrEmpty(strSearch) ||
                             purchase.PurchaseId.ToString().Trim().ToUpper().Contains(strSearch.ToUpper()) ||
                             purchase.FirstName.ToString().Trim().ToUpper().Contains(strSearch.ToUpper()) ||
                             purchase.LastName.ToString().Trim().ToUpper().Contains(strSearch.ToUpper()))
                         && DbFunctions.DiffDays(purchase.OrderDate, FromDate) <= 0
                         && DbFunctions.DiffDays(purchase.OrderDate, ToDate) >= 0
                         select new {
                             OrderNo = purchase.PurchaseId,
                             OrderDate = purchase.OrderDate,
                             FirstName = purchase.FirstName,
                             LastName=purchase.LastName,
                             PhoneNo=purchase.MobileNo,
                             GSTAmount=purchase.GSTAmount,
                             TotalAmount = purchase.TotalAmount,
                             PaymentVia = purchase.PaymentVia,
                             Representative = purchase.Staff.FirstName+" "+purchase.Staff.LastName
                         });

            if (sortBy.ToUpper() == "ORDERNO" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.OrderNo);
            }
            else if (sortBy.ToUpper() == "ORDERNO" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.OrderNo);
            }
            else if (sortBy.ToUpper() == "ORDERDATE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.OrderDate);
            }
            else if (sortBy.ToUpper() == "ORDERDATE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.OrderDate);
            }
            else if (sortBy.ToUpper() == "FIRSTNAME" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.FirstName);
            }
            else if (sortBy.ToUpper() == "FIRSTNAME" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.FirstName);
            }
            else if (sortBy.ToUpper() == "LASTNAME" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.LastName);
            }
            else if (sortBy.ToUpper() == "LASTNAME" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.LastName);
            }
            else if (sortBy.ToUpper() == "PHONENO" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.PhoneNo);
            }
            else if (sortBy.ToUpper() == "PHONENO" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.PhoneNo);
            }
            else if (sortBy.ToUpper() == "GSTAMOUNT" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.GSTAmount);
            }
            else if (sortBy.ToUpper() == "GSTAMOUNT" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.GSTAmount);
            }
            else if (sortBy.ToUpper() == "TOTALAMOUNT" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.TotalAmount);
            }
            else if (sortBy.ToUpper() == "TOTALAMOUNT" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.TotalAmount);
            }
            else if (sortBy.ToUpper() == "PAYMENTVIA" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.PaymentVia);
            }
            else if (sortBy.ToUpper() == "PAYMENTVIA" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.PaymentVia);
            }
            else if (sortBy.ToUpper() == "REPRESENTATIVE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Representative);
            }
            else if (sortBy.ToUpper() == "REPRESENTATIVE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Representative);
            }

            var PurchaseOrders = query.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(PurchaseOrders.Count / pagesize);

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

            return Json(new { Pages = pages, Result = PurchaseOrders.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PurchaseOrders(PurchaseOrderHeader objPurchaseOrderHeader)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (objPurchaseOrderHeader != null)
                {
                    objPurchaseOrderHeader.GymId = System.Web.HttpContext.Current.Session["LoggedGym"].ToString();
                    if (objPurchaseOrderHeader.PurchaseId != 0)
                    {
                        db.PurchaseOrderDetails.RemoveRange(db.PurchaseOrderDetails.Where(c => c.PurchaseId == objPurchaseOrderHeader.PurchaseId));
                        db.SaveChanges();

                        db.PurchaseOrderDetails.AddRange(objPurchaseOrderHeader.PurchaseOrderDetails);
                        db.SaveChanges();
                    }

                    db.Entry(objPurchaseOrderHeader).State = objPurchaseOrderHeader.PurchaseId == 0 ? EntityState.Added : EntityState.Modified;
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
        public JsonResult GetProducts(int branchId)
        {
            int gymid = SessionManger.GymId;
            var products = (from product in db.Products
                            where product.GymId == gymid
                            && product.BranchId == branchId
                            select new
                            {
                                ProductId = product.ProductId,
                                ProductName = product.ProductName,
                                Price = product.Price
                            }).ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductDetails(string PrdouctId)
        {
            int productid = 0;
            try
            {
                productid = Convert.ToInt32(PrdouctId);
            }
            catch (Exception ex)
            {

            }
            var products = (from product in db.Products
                            where product.ProductId == productid
                            select new
                            {
                                Price = product.Price,
                            }).ToList();


            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMemberDetails(string searchCharacter, int? page,int branchId)
        {
            int gymid = SessionManger.GymId;
            var Memebers = (from MemberInfo in db.MemberInfoes
                            where MemberInfo.GymId == gymid
                            where MemberInfo.BranchId == branchId
                            && ((MemberInfo.FirstName.ToUpper() + " " + MemberInfo.LastName.ToUpper()).Contains(searchCharacter.Trim().ToUpper())
                            || MemberInfo.MemberID.ToString().Contains(searchCharacter.Trim())
                            || MemberInfo.MobileNumber.ToString().Contains(searchCharacter.Trim()))

                            select new
                            {
                                FirstName = MemberInfo.FirstName,
                                LastName = MemberInfo.LastName,
                                MemberID = MemberInfo.MemberID,
                                MobileNumber = MemberInfo.MobileNumber,
                            }).ToList();

            int pagesize = pagesize = 6;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return Json(new { Pages = Math.Ceiling(Memebers.Count / 6.0), Result = Memebers.ToPagedList(pageIndex, pagesize) }, JsonRequestBehavior.AllowGet);
        }

        private PurchaseOrderViewModel PurchaseOrder(PurchaseOrderHeader objPurchaseOrderHeader)
        {
            PurchaseOrderViewModel objPurchaseOrderViewModel = new PurchaseOrderViewModel();
            try
            {
                objPurchaseOrderViewModel.PurchaseId = objPurchaseOrderHeader.PurchaseId;
                objPurchaseOrderViewModel.MemberId = objPurchaseOrderHeader.MemberId.HasValue ? Convert.ToInt32(objPurchaseOrderHeader.MemberId) : 0;
                objPurchaseOrderViewModel.FirstName = objPurchaseOrderHeader.FirstName;
                objPurchaseOrderViewModel.LastName = objPurchaseOrderHeader.LastName;
                objPurchaseOrderViewModel.MobileNo = objPurchaseOrderHeader.MobileNo;
                objPurchaseOrderViewModel.OrderDate = objPurchaseOrderHeader.OrderDate;
                objPurchaseOrderViewModel.PaymentVia = objPurchaseOrderHeader.PaymentVia;
                objPurchaseOrderViewModel.Representative = objPurchaseOrderHeader.Staff!=null ?objPurchaseOrderHeader.Staff.FirstName + " " + objPurchaseOrderHeader.Staff.LastName : "";
                objPurchaseOrderViewModel.GymId = objPurchaseOrderHeader.GymId;
                objPurchaseOrderViewModel.BranchId = objPurchaseOrderHeader.BranchId.HasValue ? Convert.ToInt32(objPurchaseOrderHeader.BranchId) : 0;
                objPurchaseOrderViewModel.StaffId = objPurchaseOrderHeader.StaffId.HasValue ? Convert.ToInt32(objPurchaseOrderHeader.StaffId) : 0;
                objPurchaseOrderViewModel.SubTotal = objPurchaseOrderHeader.SubTotal.HasValue ? Convert.ToDecimal(objPurchaseOrderHeader.SubTotal) : 0;
                objPurchaseOrderViewModel.GSTAmount = objPurchaseOrderHeader.GSTAmount.HasValue ? Convert.ToDecimal(objPurchaseOrderHeader.GSTAmount) : 0;
                objPurchaseOrderViewModel.TotalAmount = objPurchaseOrderHeader.TotalAmount.HasValue ? Convert.ToDecimal(objPurchaseOrderHeader.TotalAmount) : 0;


                List<PurchaseOrderDetailViewModel> lstPurchaseOrder = new List<PurchaseOrderDetailViewModel>();

                foreach (PurchaseOrderDetail objOrdedetail in objPurchaseOrderHeader.PurchaseOrderDetails)
                {
                    PurchaseOrderDetailViewModel objPurchaseOrderDetailViewModel = new PurchaseOrderDetailViewModel();

                    objPurchaseOrderDetailViewModel.DetailId = objOrdedetail.DetailId;
                    objPurchaseOrderDetailViewModel.PurchaseId = objOrdedetail.PurchaseId.HasValue ? Convert.ToInt32(objOrdedetail.PurchaseId) : 0;
                    objPurchaseOrderDetailViewModel.ProductId = objOrdedetail.ProductId.HasValue ? Convert.ToInt32(objOrdedetail.ProductId) : 0;
                    objPurchaseOrderDetailViewModel.Qty = objOrdedetail.Qty.HasValue ? Convert.ToDecimal(objOrdedetail.Qty) : 0;
                    objPurchaseOrderDetailViewModel.Rate = objOrdedetail.Rate.HasValue ? Convert.ToDecimal(objOrdedetail.Rate) : 0;
                    objPurchaseOrderDetailViewModel.DiscountType = objOrdedetail.DiscountType.HasValue ? Convert.ToInt32(objOrdedetail.DiscountType) : 0;
                    objPurchaseOrderDetailViewModel.Discount = objOrdedetail.Discount.HasValue ? Convert.ToDecimal(objOrdedetail.Discount) : 0;
                    objPurchaseOrderDetailViewModel.GST = objOrdedetail.GST.HasValue ? Convert.ToInt32(objOrdedetail.GST) : 0;
                    decimal DiscountType = objPurchaseOrderDetailViewModel.DiscountType;
                    decimal Qty = objPurchaseOrderDetailViewModel.Qty;
                    decimal Rate = objPurchaseOrderDetailViewModel.Rate;
                    decimal Discount = objPurchaseOrderDetailViewModel.Discount;
                    decimal Gst = objPurchaseOrderDetailViewModel.GST;
                    decimal Amount = Qty * Rate;
                    decimal DiscountAmt = DiscountType != 0 ? DiscountType == 1 ? Discount : Amount * (Discount / 100) : 0;

                    objPurchaseOrderDetailViewModel.NetAmount = Amount - DiscountAmt;
                    objPurchaseOrderDetailViewModel.GstAmount = Gst != 0 ? objPurchaseOrderDetailViewModel.NetAmount * (Gst / 100) : 0;

                    objPurchaseOrderDetailViewModel.ProductName = objOrdedetail.Product.ProductName;

                    lstPurchaseOrder.Add(objPurchaseOrderDetailViewModel);
                }

                objPurchaseOrderViewModel.PurchaseOrderDetails = lstPurchaseOrder;

            }
            catch (Exception ex)
            {

            }
            return objPurchaseOrderViewModel;
        }

        public JsonResult GetOrderById(string id)
        {
            int PurchaseId = Convert.ToInt32(id);
            PurchaseOrderHeader purchaseOrder = db.PurchaseOrderHeaders.Find(PurchaseId);
            PurchaseOrderViewModel order = PurchaseOrder(purchaseOrder);
            return Json(order, JsonRequestBehavior.AllowGet);
        }
    }
}
