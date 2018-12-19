using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.ADModel;
using Justbok.Models;
using System.Data.Entity;


namespace Justbok.Controllers
{
    public class GymBranchController : LayoutBaseModel
    {
        //
        // GET: /GymBranch/

        public ActionResult BranchList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpGet]
    

        public JsonResult BranchesList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var branches = (from branch in db.Branches
                            where branch.GymId==gymid
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

    }
}
