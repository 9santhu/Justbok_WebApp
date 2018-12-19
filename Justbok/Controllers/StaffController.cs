using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using PagedList;
using Justbok.ADModel;
using System.Data.Entity;
using System.IO;

namespace Justbok.Controllers
{

    public class AddBranchStaff
    {
        public string BranchName { get; set; }
        public string BranchId { get; set; }

    }

    public class ShiftStaff
    {
        public string ShiftId { get; set; }
        public string ShiftName { get; set; }

    }

    public class StaffController : LayoutBaseModel
    {
        //
        // GET: /Staff/

     
        JustbokEntities db = new JustbokEntities();
        public ActionResult StaffList()
        {
           // int pagesize = pagesize = 10;
           // int pageIndex = 1;
           // pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           //// IPagedList Stafflist = null;
           // List<Staff> lstStaffList = db.Staffs.ToList();
           // return View(lstStaffList.ToPagedList(pageIndex, pagesize));

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
            
        }

    [HttpGet]
        public JsonResult GetStaffList(int? page, float pagesize, int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var staff = (from mi in db.Staffs
                           join sb in db.StaffBranches on mi.StaffId equals sb.StaffId
                           where mi.GymId == gymid && sb.BranchId == BranchId
                           select new
                           {
                               StaffId = mi.StaffId,
                               FirstName = mi.FirstName,
                               LastName = mi.LastName,
                               Email = mi.Email,
                               PhoneNumber = mi.PhoneNumber,
                               StaffRole = mi.StaffRole,

                           }).ToList();
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(staff.Count / pagesize);
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

            return Json(new { Pages = pages, Result = staff.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddStaff()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [HttpPost]
        public JsonResult AddStaff(StaffViewModel objStaffViewModel)
        {
            Staff objStaff = new Staff();
            GymLogin objStaffLogin = new GymLogin();
            StaffBranch objStaffBranch = new StaffBranch();
            StaffShift objstaffShift = new StaffShift();

            objStaff = UpdateStaff(objStaffViewModel);
            db.Staffs.Add(objStaff);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["Staffid"] = objStaff.StaffId;
            if (objStaffViewModel.UserName != null)
            {
                objStaffLogin = UpdateStaffLogin(objStaffViewModel);
                db.GymLogins.Add(objStaffLogin);
                db.SaveChanges();
            }
           
           UpdateStaffBranch(objStaffViewModel);
            //db.StaffBranches.Add(objStaffBranch);
            //db.SaveChanges();
         UpdateStaffShift(objStaffViewModel);
         SaveTrainerDetails(objStaffViewModel);
         return Json("Success", JsonRequestBehavior.AllowGet);
        }


        private void SaveTrainerDetails(StaffViewModel objStaff)
        {
            TrainerDetail objTrainerDetails = new TrainerDetail();
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
            objTrainerDetails.StaffId = staffid;
            objTrainerDetails.TrainerDescription = objStaff.TrainerDescription;
            objTrainerDetails.Experience = objStaff.Experience;
            objTrainerDetails.Qulifiaction = objStaff.Qulifiaction;
            db.TrainerDetails.Add(objTrainerDetails);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["Trainerid"] = objTrainerDetails.Trainerid;

        }

        private void EditSaveTrainerDetails(StaffViewModel objStaff)
        {
            TrainerDetail objTrainerDetails = new TrainerDetail();
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            System.Web.HttpContext.Current.Session["EditTrainerid"]=objStaff.Trainerid;
            if (objStaff.Trainerid != 0)
            {
                objTrainerDetails.Trainerid = objStaff.Trainerid;
                objTrainerDetails.StaffId = staffid;
                objTrainerDetails.TrainerDescription = objStaff.TrainerDescription;
                objTrainerDetails.Experience = objStaff.Experience;
                objTrainerDetails.Qulifiaction = objStaff.Qulifiaction;
                db.Entry(objTrainerDetails).State = EntityState.Modified;
                db.SaveChanges();
            }
           
        }


        [HttpPost]
        public JsonResult UploadImage()
        {
            var result = "error";
            int trainerId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Trainerid"]);
            if (trainerId != 0)
            {
                string _imgname = string.Empty;
                TrainerDetail objTarinerImage = new TrainerDetail();
                var user = trainerId;
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var pic = System.Web.HttpContext.Current.Request.Files["imageUploadForm"];
                    if (pic.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(pic.FileName);
                        var _ext = Path.GetExtension(pic.FileName);
                        _imgname = user + _ext;
                        var _comPath = Server.MapPath("~/Users/") + _imgname;
                        pic.SaveAs(_comPath);
                        var dbpath = "/Users/" + _imgname;

                        List<TrainerDetail> trainerDetails = (from am in db.TrainerDetails
                                                                   where am.Trainerid == trainerId
                                                                   select am).ToList();

                        if (trainerDetails.Count > 0)
                        {
                            trainerDetails[0].ImageData = dbpath;
                            db.TrainerDetails.Attach(trainerDetails[0]);
                            db.Entry(trainerDetails[0]).Property(x => x.ImageData).IsModified = true;
                            db.SaveChanges();
                            result = "success";
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateUploadImage()
        {
            var result = "error";
            int trainerId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditTrainerid"]);
            if (trainerId != 0)
            {
                string _imgname = string.Empty;
                TrainerDetail objTarinerImage = new TrainerDetail();
                var user = trainerId;
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    var pic = System.Web.HttpContext.Current.Request.Files["imageUploadForm"];
                    if (pic.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(pic.FileName);
                        var _ext = Path.GetExtension(pic.FileName);
                        _imgname = user + _ext;
                        var _comPath = Server.MapPath("~/Users/") + _imgname;
                        pic.SaveAs(_comPath);
                        var dbpath = "/Users/" + _imgname;

                        List<TrainerDetail> trainerDetails = (from am in db.TrainerDetails
                                                              where am.Trainerid == trainerId
                                                              select am).ToList();

                        if (trainerDetails.Count > 0)
                        {
                            trainerDetails[0].ImageData = dbpath;
                            db.TrainerDetails.Attach(trainerDetails[0]);
                            db.Entry(trainerDetails[0]).Property(x => x.ImageData).IsModified = true;
                            db.SaveChanges();
                            result = "success";
                        }
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }




        public ActionResult EditStaff(string id)
        {
        int staffid= Convert.ToInt32(id);
        System.Web.HttpContext.Current.Session["EditStaffid"] = staffid;
            //Staff staff = db.Staffs.Find(staffid);
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpPost]
        public JsonResult SaveEditStaff(StaffViewModel objStaffViewModel)
        {
            Staff objStaff = new Staff();
            GymLogin objStaffLogin = new GymLogin();
            StaffBranch objStaffBranch = new StaffBranch();
            StaffShift objstaffShift = new StaffShift();
            objStaff = UpdateStaff(objStaffViewModel);
            objStaff.StaffId = objStaffViewModel.StaffId;
            db.Entry(objStaff).State = EntityState.Modified;
            db.SaveChanges();
            if (objStaffViewModel.UserName != null)
            {
                objStaffLogin = EditUpdateStaffLogin(objStaffViewModel);
                db.Entry(objStaffLogin).State = objStaffLogin.Loginid == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            
            EditUpdateStaffShift(objStaffViewModel);
            EditStaffBranch(objStaffViewModel);
            EditSaveTrainerDetails(objStaffViewModel);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStaffDetails()
        {
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            var stafflist = db.Staffs
                .Where(x => x.StaffId == staffid)
                .Select(a => new
                {

                    StaffId = a.StaffId,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    DOB = a.DOB.ToString(),
                    PhoneNumber = a.PhoneNumber,
                    Email = a.Email,
                    StaffAddress = a.StaffAddress,
                    StaffRole = a.StaffRole,
                    DailySalary = a.DailySalary,
                    Isactive = a.Isactive
                }).ToList();

            return Json(stafflist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTrainerDetails()
        {
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            var trainerlist = db.TrainerDetails
                .Where(x => x.StaffId == staffid)
                .Select(a => new
                {

                    Trainerid = a.Trainerid,
                    ImageData = a.ImageData,
                    Experience = a.Experience,
                    Qulifiaction = a.Qulifiaction,
                    TrainerDescription = a.TrainerDescription,
                }).ToList();

            return Json(trainerlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLoginDetails()
        {
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            var LoginList = db.GymLogins
                .Where(x => x.StaffId == staffid)
                .Select(a => new
                {

                    Loginid = a.Loginid,
                    UserName = a.UserName,
                    Password = a.Password,
                    IsLoginActive = a.IsLoginActive,
                }).ToList();

            return Json(LoginList, JsonRequestBehavior.AllowGet);
        }

        private Staff  UpdateStaff(StaffViewModel objStaff)
        {
            Staff  objStaffUpdate = new Staff();
            try
            {
                objStaffUpdate.FirstName = objStaff.FirstName;
                objStaffUpdate.LastName = objStaff.LastName;
                objStaffUpdate.Email = objStaff.Email;
                objStaffUpdate.DOB = objStaff.DOB;
                objStaffUpdate.PhoneNumber = objStaff.PhoneNumber;
                objStaffUpdate.StaffAddress = objStaff.StaffAddress;
                objStaffUpdate.StaffRole = objStaff.StaffRole;
                objStaffUpdate.DailySalary = objStaff.DailySalary;
                objStaffUpdate.Isactive = objStaff.Isactive ? "Yes": "No";
                objStaffUpdate.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]); 
            }
            catch (Exception ex)
            {
            }
            return objStaffUpdate;
        }


        private GymLogin UpdateStaffLogin(StaffViewModel objStaffLog)
        {
            //StaffLogin objStaffLogin = new StaffLogin();
            GymLogin objLogin = new GymLogin();
            try
            {
                objLogin.UserName = objStaffLog.UserName;
                objLogin.Password = objStaffLog.Password;
                objLogin.IsLoginActive = objStaffLog.IsLoginactive ? "Yes" : "No";
                objLogin.Role = objStaffLog.StaffRole;
                objLogin.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]); 
                objLogin.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
            }
            catch (Exception ex)
            {
            }
            return objLogin;
        }

        private GymLogin EditUpdateStaffLogin(StaffViewModel objStaffLog)
        {
            //StaffLogin objStaffLogin = new StaffLogin();
            GymLogin objLogin = new GymLogin();
            try
            {
                objLogin.Loginid = objStaffLog.Loginid;
                objLogin.UserName = objStaffLog.UserName;
                objLogin.Password = objStaffLog.Password;
                objLogin.IsLoginActive = objStaffLog.IsLoginactive ? "Yes" : "No";
                objLogin.Role = objStaffLog.StaffRole;
                objLogin.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                objLogin.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            }
            catch (Exception ex)
            {
            }
            return objLogin;
        }

        private void UpdateStaffBranch(StaffViewModel objBranch)
        {
           
            List<string> lstBranches = new List<string>();
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
            try
            {
                string[] arrBranches = objBranch.MultiselectBranch.Split(',');
                foreach (string branch in arrBranches)
                {
                    if (!branch.Contains("multiselect-all"))
                    {
                        lstBranches.Add(branch);
                    }
                }


                if (lstBranches.Count > 0)
                {

                    StaffBranch objStaffbranch = new StaffBranch();
                    int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                    var branchtype = (from branch in db.Branches
                                      where branch.GymId == gymid
                                      select new
                                      {
                                          BranchId = branch.BranchId
                                      }).ToList();

                    if (branchtype.Count > 0)
                    {
                        foreach (var staffbranch in branchtype)
                        {
                            objStaffbranch.StaffId = staffid;
                            objStaffbranch.BranchId = Convert.ToInt32(staffbranch.BranchId);
                            objStaffbranch.Enable = false;
                            db.StaffBranches.Add(objStaffbranch);
                            db.SaveChanges();

                        }

                        //enable selected shifts
                        foreach (string branch in lstBranches)
                        {
                            var branchid = Convert.ToInt32(branch);
                            List<StaffBranch> getallbranchesDetails = (from am in db.StaffBranches
                                                                       where am.StaffId == staffid && am.BranchId == branchid
                                                                       select am).ToList();

                            if (getallbranchesDetails.Count > 0)
                            {
                                getallbranchesDetails[0].Enable = true;
                                db.StaffBranches.Attach(getallbranchesDetails[0]);
                                db.Entry(getallbranchesDetails[0]).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                            }



                        }
                    }
                }


                //if (lstBranches.Count > 0)
                //{
                //    foreach (string branch in lstBranches)
                //    {
                //        StaffBranch objStaffBranch = new StaffBranch();
                //        objStaffBranch.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
                //        objStaffBranch.BranchId = Convert.ToInt32(branch);
                //        db.StaffBranches.Add(objStaffBranch);
                //        db.SaveChanges();
                //    }

                //}

                //objStaffBranch.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
                //objStaffBranch.BranchId = objBranch.BranchId;
                
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateStaffShift(StaffViewModel objShift)
        {
            StaffShift objStaffShift1 = new StaffShift();
            List<string> lstShift = new List<string>();
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
            try
            {
                string[] arrShift = objShift.MultiselectShiftType.Split(',');
                foreach (string shift in arrShift)
                {
                    if (!shift.Contains("multiselect-all"))
                    {
                        lstShift.Add(shift);
                    }
                }

                if (lstShift.Count > 0)
                {
                    StaffShift objStaffShift = new StaffShift();
                    var Shifttype = (from shifttype in db.ShiftTypes
                                     select new
                                     {
                                         ShiftId = shifttype.ShiftId
                                     }).ToList();

                    if (Shifttype.Count > 0)
                    {
                        foreach (var staffshift in Shifttype)
                        {
                            objStaffShift.StaffId = staffid;
                            objStaffShift.ShiftId = Convert.ToInt32(staffshift.ShiftId);
                            objStaffShift.Enable = false;
                            db.StaffShifts.Add(objStaffShift);
                            db.SaveChanges();

                        }

                        //enable selected shifts
                        foreach (string shift in lstShift)
                        {
                            var shiftid = Convert.ToInt32(shift);
                            List<StaffShift> staffShiftdetails = (from am in db.StaffShifts
                                                                  where am.StaffId == staffid && am.ShiftId == shiftid
                                                                  select am).ToList();
                            if (staffShiftdetails.Count > 0)
                            {
                                staffShiftdetails[0].Enable = true;
                                db.StaffShifts.Attach(staffShiftdetails[0]);
                                db.Entry(staffShiftdetails[0]).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                            }
                        }
                    }
                }


                //if (lstShift.Count > 0)
                //{
                //    foreach (string shift in lstShift)
                //    {
                //        StaffShift objStaffShift = new StaffShift();
                //        objStaffShift.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
                //        objStaffShift.ShiftId = Convert.ToInt32(shift);
                //        db.StaffShifts.Add(objStaffShift);
                //        db.SaveChanges();
                //    }

                //}


                //objStaffShift.StaffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["Staffid"]);
                //objStaffShift.ShiftId = objShift.ShiftId;

            }
            catch (Exception ex)
            {
            }
        }

        private void EditUpdateStaffShift(StaffViewModel objShift)
        {
            StaffShift objStaffShift1 = new StaffShift();
            List<string> lstShift = new List<string>();
            try
            {
                string[] arrShift = objShift.MultiselectShiftType.Split(',');
                foreach (string shift in arrShift)
                {
                    if (!shift.Contains("multiselect-all"))
                    {
                        lstShift.Add(shift);
                    }
                }
                 int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
                if (lstShift.Count > 0)
                {
                   
                   
                 List<StaffShift> getallshifts = (from am in db.StaffShifts
                                                   where am.StaffId == staffid
                                                   select am).ToList();
                    //get all shifts available for staff
                    if (getallshifts.Count > 0)
                    {
                        foreach (var checkshift in getallshifts)
                        {
                            var ShiftFound = lstShift.Find(x => x.Equals(checkshift.ShiftId.ToString()));
                            if (ShiftFound != null)
                            {
                                if (checkshift.Enable == false)
                                { 
                                checkshift.Enable = true;
                                db.StaffShifts.Attach(checkshift);
                                db.Entry(checkshift).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                                }
                            }
                            else
                            {
                                checkshift.Enable = false;
                                db.StaffShifts.Attach(checkshift);
                                db.Entry(checkshift).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                            }
                        }

                        
                    }
                    else
                    {
                        //if there are no shifts added then add new shifts 
                        //add all shift with disable
                        StaffShift objStaffShift = new StaffShift();
                        var Shifttype = (from shifttype in db.ShiftTypes
                                          select new
                                          {
                                              ShiftId = shifttype.ShiftId
                                          }).ToList();

                        if (Shifttype.Count > 0)
                        {
                            foreach (var staffshift in Shifttype)
                            {
                                objStaffShift.StaffId = staffid;
                                objStaffShift.ShiftId = Convert.ToInt32(staffshift.ShiftId);
                                objStaffShift.Enable = false;
                                db.StaffShifts.Add(objStaffShift);
                                db.SaveChanges();

                            }

                             //enable selected shifts
                        foreach (string shift in lstShift)
                        {
                            var shiftid = Convert.ToInt32(shift);
                            List<StaffShift> staffShiftdetails = (from am in db.StaffShifts
                                                                  where am.StaffId == staffid && am.ShiftId == shiftid
                                                             select am).ToList();
                            if (staffShiftdetails.Count > 0)
                            {
                                staffShiftdetails[0].Enable = true;
                                db.StaffShifts.Attach(staffShiftdetails[0]);
                                db.Entry(staffShiftdetails[0]).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                            }
                        }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void EditStaffBranch(StaffViewModel objBranch)
        {
            List<string> lstBranches = new List<string>();
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            try
            {
                string[] arrBranches = objBranch.MultiselectBranch.Split(',');
                foreach (string branch in arrBranches)
                {
                    if (!branch.Contains("multiselect-all"))
                    {
                        lstBranches.Add(branch);
                    }
                }


                if (lstBranches.Count > 0)
                {


                    List<StaffBranch> getallbranches = (from am in db.StaffBranches
                                                     where am.StaffId == staffid
                                                     select am).ToList();
                    //get all shifts available for staff
                    if (getallbranches.Count > 0)
                    {
                        foreach (var checkBranch in getallbranches)
                        {
                            var BranchFound = lstBranches.Find(x => x.Equals(checkBranch.BranchId.ToString()));
                            if (BranchFound != null)
                            {
                                if (checkBranch.Enable == false)
                                {
                                    checkBranch.Enable = true;
                                    db.StaffBranches.Attach(checkBranch);
                                    db.Entry(checkBranch).Property(x => x.Enable).IsModified = true;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                checkBranch.Enable = false;
                                db.StaffBranches.Attach(checkBranch);
                                db.Entry(checkBranch).Property(x => x.Enable).IsModified = true;
                                db.SaveChanges();
                            }
                        }


                    }
                    else
                    {
                        //if there are no shifts added then add new shifts 
                        //add all shift with disable
                        StaffBranch objStaffbranch = new StaffBranch();
                        int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                        var branchtype = (from branch in db.Branches
                                          where branch.GymId==gymid
                                         select new
                                         {
                                             BranchId = branch.BranchId
                                         }).ToList();

                        if (branchtype.Count > 0)
                        {
                            foreach (var staffbranch in branchtype)
                            {
                                objStaffbranch.StaffId = staffid;
                                objStaffbranch.BranchId = Convert.ToInt32(staffbranch.BranchId);
                                objStaffbranch.Enable = false;
                                db.StaffBranches.Add(objStaffbranch);
                                db.SaveChanges();

                            }

                            //enable selected shifts
                            foreach (string branch in lstBranches)
                            {
                                var branchid = Convert.ToInt32(branch);
                                List<StaffBranch> getallbranchesDetails = (from am in db.StaffBranches
                                                                           where am.StaffId == staffid && am.BranchId == branchid
                                                                    select am).ToList();

                                if (getallbranchesDetails.Count > 0)
                                {
                                    getallbranchesDetails[0].Enable = true;
                                    db.StaffBranches.Attach(getallbranchesDetails[0]);
                                    db.Entry(getallbranchesDetails[0]).Property(x => x.Enable).IsModified = true;
                                    db.SaveChanges();
                                }



                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        [HttpGet]
        public JsonResult GetBranchList()
        {
           var gymid=Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            var branhList = (from branch in db.Branches
                             where branch.GymId == gymid
                             select new
                             {
                                 BranchId = branch.BranchId,
                                 BranchName = branch.BranchName,

                             }).ToList();
            return Json(branhList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftList()
        {
            var ShiftList = db.ShiftTypes
                .Select(a => new
                {
                    ShiftId = a.ShiftId,
                    ShiftName = a.ShiftName,
                }).ToList();
            return Json(ShiftList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSelectedBranchList()
        {
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            var branhList = (from branch in db.StaffBranches
                             where branch.StaffId == staffid && branch.Enable == true
                             select new
                             {
                                 BranchId = branch.BranchId,

                             }).ToList();
            return Json(branhList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSelectedShiftList()
        {
            int staffid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditStaffid"]);
            var ShiftList = (from staff in db.StaffShifts
                             where staff.StaffId == staffid && staff.Enable==true
                             select new
                             {
                                 ShiftId = staff.ShiftId,

                             }).ToList();
            return Json(ShiftList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteStaff(string staffid)
        {
            int Staffid = Convert.ToInt32(staffid);
            //Login
            var Login = db.GymLogins.Where(x => x.StaffId == Staffid).SingleOrDefault();
            if (Login != null)
            {
                db.GymLogins.Remove(Login);
                db.SaveChanges();
            }

            //Trainer
            var Tranier = db.TrainerDetails.Where(x => x.StaffId == Staffid).SingleOrDefault();
            if (Tranier != null)
            {
                db.TrainerDetails.Remove(Tranier);
                db.SaveChanges();
            }

            //Staff Branch
            List<StaffBranch> staffbranches = (from am in db.StaffBranches
                                               where am.StaffId == Staffid
                                                  select am).ToList();
            if (staffbranches.Count>0)
            {
                foreach (var staff in staffbranches)
                {
                    db.StaffBranches.Remove(staff);
                    db.SaveChanges();
                }

            }

            //Staff Shift
            List<StaffShift> staffshifts = (from am in db.StaffShifts
                                               where am.StaffId == Staffid
                                               select am).ToList();
            if (staffshifts.Count>0)
            {
                foreach (var shift in staffshifts)
                {
                    db.StaffShifts.Remove(shift);
                    db.SaveChanges();
                }

            }

            //Remove staff
            var getstaff = db.Staffs.Where(x => x.StaffId == Staffid).SingleOrDefault();

            if (getstaff != null)
            {
                db.Staffs.Remove(getstaff);
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}
