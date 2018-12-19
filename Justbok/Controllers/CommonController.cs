using Justbok.ADModel;
using Justbok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Controllers
{
    public class CommonController : LayoutBaseModel
    {
        [HttpGet]
        public JsonResult GetTaxTypes()
        {
            try
            {
                var Taxes = (from Tax in db.TaxTypes
                             select new
                             {
                                 TaxTypeId = Tax.TaxTypeId,
                                 Description = Tax.Description
                             }).ToList();

                return Json(Taxes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetStaff(int branchId)
        {
            try
            {
                int GymId = SessionManger.GymId;
                var Staffs = (from Staff in db.Staffs
                              join SBranch in db.StaffBranches on Staff.StaffId equals SBranch.StaffId
                              where Staff.Isactive.Trim().ToUpper().Equals("YES")
                              && SBranch.Enable == true
                              && Staff.GymId == GymId
                              && SBranch.BranchId == branchId
                              //&& (Staff.StaffRole.Trim().ToUpper().Equals("STAFF MEMBER")
                              //|| Staff.StaffRole.Trim().ToUpper().Equals("TRAINER"))
                              select new
                              {
                                  StaffId = Staff.StaffId,
                                  FirstName = Staff.FirstName,
                                  LastName = Staff.LastName
                              }).ToList();

                return Json(Staffs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStaffForAutoComplete(string prefix, int branchId)
        {
            try
            {
                int GymId = SessionManger.GymId;
                var Staffs = (from Staff in db.Staffs
                              join SBranch in db.StaffBranches on Staff.StaffId equals SBranch.StaffId
                              where Staff.Isactive.Trim().ToUpper().Equals("YES")
                              && SBranch.Enable == true
                              && Staff.GymId == GymId
                              && SBranch.BranchId == branchId
                              && (Staff.FirstName.Trim().ToLower().Contains(prefix.Trim().ToLower())
                              || Staff.LastName.Trim().ToLower().Contains(prefix.Trim().ToLower()))
                              && Staff.GymId == GymId
                              select new
                              {
                                  StaffId = Staff.StaffId,
                                  FirstName = Staff.FirstName,
                                  LastName = Staff.LastName
                              }).ToList();

                return Json(Staffs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMembersForAutoComplete(string prefix, int branchId)
        {
            try
            {
                int GymId = SessionManger.GymId;
                var Staffs = (from MemberInfo in db.MemberInfoes
                              where MemberInfo.GymId == GymId
                              && MemberInfo.BranchId == branchId
                               && (MemberInfo.FirstName.Trim().ToLower().Contains(prefix.Trim().ToLower())
                              || MemberInfo.LastName.Trim().ToLower().Contains(prefix.Trim().ToLower()))
                              select new
                              {
                                  FirstName = MemberInfo.FirstName,
                                  LastName = MemberInfo.LastName,
                                  MemberId = MemberInfo.MemberID
                              }).ToList();

                return Json(Staffs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMembers()
        {
            try
            {
                int gymid = SessionManger.GymId;

                var Members = (from MemberInfo in db.MemberInfoes
                               where MemberInfo.GymId == gymid
                               select new
                               {
                                   FirstName = MemberInfo.FirstName,
                                   LastName = MemberInfo.LastName,
                                   MemberId = MemberInfo.MemberID
                               }).ToList();
                return Json(Members, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}
