using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Models
{
    public class CommonData
    {

        public List<string> GetStaffList()
        {
            List<string> lstStaffList = new List<string>();
            JustbokEntities objStaff = new JustbokEntities();
            try
            {
                lstStaffList = (from staff in objStaff.Staffs
                                select (staff.FirstName + " " + staff.LastName)).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstStaffList;

        }

        public List<SelectListItem> GetProductList()
        {
            List<SelectListItem> lstProdcuList = new List<SelectListItem>();
            JustbokEntities db = new JustbokEntities();
            try
            {
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                lstProdcuList = (from product in db.Products
                                 where product.GymId == gymid
                                 select new SelectListItem { Text = product.ProductName, Value = product.ProductId.ToString() }).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstProdcuList;
        }

        public List<SelectListItem> GetDiscountTypes()
        {
            List<SelectListItem> lstDiscountTypes = new List<SelectListItem>()
            {
                new SelectListItem { Value = "0", Text = "--Select--" },
                new SelectListItem { Value = "1", Text = "Fix Discount"  },
                new SelectListItem {Value = "2", Text = "Discount Percent"}
            };
            return lstDiscountTypes;
        }

        public List<SelectListItem> GetGSTList()
        {
            List<SelectListItem> lstGST = new List<SelectListItem>();

            lstGST.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            lstGST.Add(new SelectListItem { Text = "5%", Value = "5" });
            lstGST.Add(new SelectListItem { Text = "12%", Value = "12" });
            lstGST.Add(new SelectListItem { Text = "18%", Value = "18" });
            lstGST.Add(new SelectListItem { Text = "28%", Value = "28" });
            return lstGST;
        }

        public List<SelectListItem> GetPaymentViaList()
        {
            List<SelectListItem> lstPaymentvia = new List<SelectListItem>();

            lstPaymentvia.Add(new SelectListItem { Text = "Cash", Value = "Cash" });
            lstPaymentvia.Add(new SelectListItem { Text = "Card", Value = "Card" });
            lstPaymentvia.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            lstPaymentvia.Add(new SelectListItem { Text = "Cancel", Value = "Cancel" });
            return lstPaymentvia;
        }
    }
}