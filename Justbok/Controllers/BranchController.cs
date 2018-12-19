using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using PagedList;
using System.Data.Entity;
using System.Data.Linq;
using System.IO;
using Justbok.ADModel;


namespace Justbok.Controllers
{
    public class BranchController : LayoutBaseModel
    {
        //
        // GET: /Branch/
        JustbokEntities db = new JustbokEntities();

        public ActionResult BranchList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpGet]
        public ActionResult AddBranch()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpGet]
        public JsonResult BindGymList(string prefix)
        {
            //int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            var gymDetails = (from mi in db.GymLists
                              where mi.GymName.Contains(prefix)
                              select new
                              {
                                  GymId = mi.Gymid,
                                  GymName = mi.GymName,
                              }).ToList();

            return Json(gymDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddGymBranch(Branch branch)
        {
            Branch objBranch = new Branch();
            objBranch.BranchId = branch.BranchId;
            objBranch.Branchcode = branch.Branchcode;
            objBranch.BranchName = branch.BranchName;
            objBranch.BranchAdress = branch.BranchAdress;
            objBranch.PhoneNo = branch.PhoneNo;
            objBranch.City = branch.City;
            objBranch.BranchState = branch.BranchState;
            objBranch.GymId = branch.GymId;
            db.Entry(objBranch).State = objBranch.BranchId == 0 ? EntityState.Added : EntityState.Modified;
           // db.Branches.Add(objBranch);
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetGymList()
        {
            
            var gymList = (from gym in db.GymLists
                             select new
                             {
                                 Gymid = gym.Gymid,
                                 GymName = gym.GymName
                             }).ToList();

            return Json(gymList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BranchesList()
        {
            var branches = (from branch in db.Branches
                           select new
                           {
                               BranchId = branch.BranchId,
                               Branchcode = branch.Branchcode,
                               BranchName = branch.BranchName,
                               BranchAdress = branch.BranchAdress,
                               PhoneNo = branch.PhoneNo,
                               City = branch.City,
                               BranchState = branch.BranchState,
                               GymId = branch.GymId
                               

                           }).ToList();

            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditBranch(string BranchId)
        {

            int branchid = Convert.ToInt32(BranchId);
            //var   lstMemberData = (from membership in objMembersdata.MemberShips
            //                    where membership.MemberID == memberId
            //                    select membership).ToList();

            var branchlist = db.Branches
                .Where(x => x.BranchId == branchid)
                .Select(a => new
                {

                    BranchId = a.BranchId,
                    GymId = a.GymId,
                    Branchcode = a.Branchcode,
                    BranchName = a.BranchName,
                    BranchAdress = a.BranchAdress,
                    PhoneNo = a.PhoneNo,
                    City = a.City,
                    BranchState = a.BranchState


                }).ToList();

            return Json(branchlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteBranch(string BranchId)
        {
            var result = new { Success = "False", Message = "Unable To delete Information." };
            if (BranchId != null)
            {
                int branchid = int.Parse(BranchId);
                var branch = db.Branches.Where(x => x.BranchId == branchid).SingleOrDefault();

                if (branch != null)
                {
                    db.Branches.Remove(branch);
                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
