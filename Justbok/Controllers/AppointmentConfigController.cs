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
    public class AppointmentConfigController : LayoutBaseModel
    {

        [HttpPost]
        public JsonResult SaveAppointmentConfig(Appointment Appointment)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            try
            {
                if (Appointment != null)
                {
                    Appointment.GymId = SessionManger.GymId;
                    if (Appointment.AppointmentId != 0)
                    {
                        db.AppointmentStaffs.RemoveRange(db.AppointmentStaffs.Where(c => c.AppointmentId == Appointment.AppointmentId));
                        db.SaveChanges();

                        db.AppointmentStaffs.AddRange(Appointment.AppointmentStaffs);
                        db.SaveChanges();

                        db.AppointmentSlabs.RemoveRange(db.AppointmentSlabs.Where(c => c.AppointmentId == Appointment.AppointmentId));
                        db.SaveChanges();

                        db.AppointmentSlabs.AddRange(Appointment.AppointmentSlabs);
                        db.SaveChanges();
                    }

                    db.Entry(Appointment).State = Appointment.AppointmentId == 0 ? EntityState.Added : EntityState.Modified;
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

        public JsonResult AppointmentConfigList(int? page, float pagesize, string Title, string sortBy, string sortDirection,int branchId)
        {
            try
            {
                int gymid = SessionManger.GymId;

                var query = (from Appointment in db.Appointments
                             where Appointment.GymId == gymid
                             && Appointment.BranchId == branchId
                             && Appointment.IsActive == true
                             && (string.IsNullOrEmpty(Title) || Appointment.Title.Trim().ToUpper().Contains(Title.Trim().ToUpper()))
                             select Appointment);

                if (sortBy.ToUpper() == "TITLE" && sortDirection.ToUpper() == "ASC")
                {
                    query = query.OrderBy(s => s.Title);
                }
                else if (sortBy.ToUpper() == "TITLE" && sortDirection.ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(s => s.Title);
                }

                var AppointmentConfigList = query.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(AppointmentConfigList.Count / pagesize);

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

                return Json(new { Pages = pages, Result = AppointmentConfig(AppointmentConfigList.ToPagedList(pageIndex, (int)pagesize)) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private IList<AppointmentConfigViewModel> AppointmentConfig(IPagedList<Appointment> Appointments)
        {
            try
            {
                if (Appointments != null && Appointments.Count > 0)
                {
                    IList<AppointmentConfigViewModel> lstAppointmentConfig = new List<AppointmentConfigViewModel>();

                    foreach (Appointment objAppointment in Appointments)
                    {
                        AppointmentConfigViewModel objAppointmentConfigViewModel = new AppointmentConfigViewModel();

                        objAppointmentConfigViewModel.AppointmentId = objAppointment.AppointmentId;
                        objAppointmentConfigViewModel.IsAllStaff = objAppointment.IsAllStaff.HasValue ? Convert.ToBoolean(objAppointment.IsAllStaff) : false;
                        objAppointmentConfigViewModel.Title = objAppointment.Title;
                        objAppointmentConfigViewModel.AppointmentStaffs = objAppointmentConfigViewModel.IsAllStaff ? "All Staff" : GetAppointmentStaff(objAppointment.AppointmentStaffs);
                        objAppointmentConfigViewModel.AppointmentSlabs = GetAppointmentSlab(objAppointment.AppointmentSlabs);

                        lstAppointmentConfig.Add(objAppointmentConfigViewModel);
                    }

                    return lstAppointmentConfig;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        private string GetAppointmentStaff(ICollection<AppointmentStaff> AppointmentStaffs)
        {
            try
            {
                if (AppointmentStaffs.Count > 0)
                {
                    string strAppointmentStaffs = "";
                    foreach (AppointmentStaff objAppointmentStaff in AppointmentStaffs)
                    {
                        strAppointmentStaffs += objAppointmentStaff.Staff.FirstName + " " + objAppointmentStaff.Staff.LastName + ",";
                    }

                    strAppointmentStaffs = strAppointmentStaffs.Substring(0, strAppointmentStaffs.LastIndexOf(","));

                    return strAppointmentStaffs;
                }
            }
            catch (Exception)
            {
            }

            return "";
        }

        private string GetAppointmentSlab(ICollection<AppointmentSlab> AppointmentSlab)
        {
            try
            {
                if (AppointmentSlab.Count > 0)
                {
                    string strAppointmentSlab = "";
                    foreach (AppointmentSlab objAppointmentSlab in AppointmentSlab)
                    {
                        strAppointmentSlab += "Rs" + objAppointmentSlab.Price + " for " + objAppointmentSlab.Minutes + " minutes <br/>";
                    }

                    strAppointmentSlab = strAppointmentSlab.Substring(0, strAppointmentSlab.LastIndexOf("<br/>"));

                    return strAppointmentSlab;
                }
            }
            catch (Exception)
            {
            }

            return "";
        }

        private List<string> GetAppointmentStaffId(ICollection<AppointmentStaff> AppointmentStaffs)
        {
            try
            {
                if (AppointmentStaffs != null && AppointmentStaffs.Count > 0)
                {
                    List<string> arrAppointmentStaffs = new List<string>();
                    foreach (AppointmentStaff objAppointmentStaff in AppointmentStaffs)
                    {
                        arrAppointmentStaffs.Add(objAppointmentStaff.StaffId.ToString());
                    }
                    return arrAppointmentStaffs;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        private List<AppointmentSlabModel> GetAppointmentSlabs(ICollection<AppointmentSlab> AppointmentSlab)
        {
            try
            {
                if (AppointmentSlab.Count > 0)
                {
                    List<AppointmentSlabModel> arrAppointmentSlab = new List<AppointmentSlabModel>();
                    foreach (AppointmentSlab objAppointmentSlab in AppointmentSlab)
                    {
                        AppointmentSlabModel objAppointmentSlabModel = new AppointmentSlabModel();
                        objAppointmentSlabModel.Minutes = objAppointmentSlab.Minutes.HasValue ? Convert.ToInt32(objAppointmentSlab.Minutes) : 0;
                        objAppointmentSlabModel.Price = objAppointmentSlab.Price.HasValue ? Convert.ToDecimal(objAppointmentSlab.Price) : 0;
                        arrAppointmentSlab.Add(objAppointmentSlabModel);
                    }

                    return arrAppointmentSlab;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        [HttpPost]
        public JsonResult Deactivate(int AppointmentId)
        {
            var result = new { Success = "False", Message = "Unable To Remove Appointment Config." };
            try
            {
                if (AppointmentId != 0)
                {
                    Appointment objAppointment = db.Appointments.Find(AppointmentId);
                    objAppointment.IsActive = false;
                    db.Entry(objAppointment).State = objAppointment.AppointmentId == 0 ? EntityState.Added : EntityState.Modified;
                    db.SaveChanges();
                    result = new { Success = "True", Message = "Success" };
                }
            }
            catch (Exception ex)
            {
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public JsonResult GetAppointmentConfigById(int AppointmentId)
        {
            try
            {
                var objAppointment = (from Appointment in db.Appointments
                                      where Appointment.AppointmentId == AppointmentId
                                      select Appointment).SingleOrDefault();

                return Json(AppointmentConfig(objAppointment), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private AppointmentConfigViewModel AppointmentConfig(Appointment objAppointment)
        {
            try
            {
                if (objAppointment != null)
                {

                    AppointmentConfigViewModel objAppointmentConfigViewModel = new AppointmentConfigViewModel();

                    objAppointmentConfigViewModel.AppointmentId = objAppointment.AppointmentId;
                    objAppointmentConfigViewModel.IsAllStaff = objAppointment.IsAllStaff.HasValue ? Convert.ToBoolean(objAppointment.IsAllStaff) : false;
                    objAppointmentConfigViewModel.Title = objAppointment.Title;
                    objAppointmentConfigViewModel.TaxType = objAppointment.TaxType.HasValue ? Convert.ToInt32(objAppointment.TaxType) : 1;
                    objAppointmentConfigViewModel.TaxPercentage = objAppointment.TaxPercentage.HasValue ? Convert.ToDecimal(objAppointment.TaxPercentage) : 0;
                    objAppointmentConfigViewModel.ArrAppointmentStaffs = GetAppointmentStaffId(objAppointment.AppointmentStaffs);
                    objAppointmentConfigViewModel.ArrAppointmentSlabs = GetAppointmentSlabs(objAppointment.AppointmentSlabs);


                    return objAppointmentConfigViewModel;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

    }
}
