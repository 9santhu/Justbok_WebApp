using Justbok.ADModel;
using Justbok.Models;
using NCrontab;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Justbok.Controllers
{
    public class BookAppointmentController : LayoutBaseModel
    {
        public PartialViewResult AddAppointmentSlot()
        {
            return PartialView();
        }

        public PartialViewResult ChooseSlotOption()
        {
            return PartialView();
        }

        public PartialViewResult BookSlotAppointment()
        {
            return PartialView();
        }

        public PartialViewResult DeleteAppointmentSlot()
        {
            return PartialView();
        }

        public PartialViewResult ViewAppointment()
        {
            return PartialView();
        }

        public JsonResult GetAppConfigByStaffId(int staffId, int branchId)
        {
            if (staffId != 0)
            {
                var appConfigs = (from app in db.Appointments
                                  join appstaff in db.AppointmentStaffs on app.AppointmentId equals appstaff.AppointmentId into staff
                                  from apstaff in staff.Where(s => s.StaffId == staffId).DefaultIfEmpty()
                                  where app.IsActive == true && (app.IsAllStaff == true || apstaff.AppointmentId != null)
                                  && app.BranchId == branchId
                                  select new { ConfigId = app.AppointmentId, Title = app.Title }).ToList();

                return Json(appConfigs, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavingAppointmentSlot(AppointmentSlot AppointmentSlots)
        {
            if (AppointmentSlots != null)
            {
                AppointmentSlots.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                if (AppointmentSlots.SlotId != 0)
                {
                    db.AppointmentSlotConfigs.RemoveRange(db.AppointmentSlotConfigs.Where(c => c.SlotId == AppointmentSlots.SlotId));
                    db.SaveChanges();

                    db.AppointmentSlotConfigs.AddRange(AppointmentSlots.AppointmentSlotConfigs);
                    db.SaveChanges();

                    db.AppointmentSlotPatterns.RemoveRange(db.AppointmentSlotPatterns.Where(c => c.SlotId == AppointmentSlots.SlotId));
                    db.SaveChanges();

                    db.AppointmentSlotPatterns.AddRange(AppointmentSlots.AppointmentSlotPatterns);
                    db.SaveChanges();
                }

                db.Entry(AppointmentSlots).State = AppointmentSlots.SlotId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
                return Json(new { Success = "True", Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = "False", Message = "Failed" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AppointmentSlots(string start, string end, int branchId)
        {
            DateTime FromDate = Convert.ToDateTime(start);
            DateTime ToDate = Convert.ToDateTime(end);
            int GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            var BookesSlots = (from slots in db.BookAppointments
                               where slots.IsActive == true && slots.GymId == GymId
                               && slots.BranchId == branchId
                               && ((!slots.EndDate.HasValue)
                                   || (DbFunctions.DiffDays(slots.EndDate, ToDate) >= 0 && DbFunctions.DiffDays(slots.EndDate, FromDate) <= 0)
                                   || (DbFunctions.DiffDays(slots.EndDate, ToDate) < 0 && DbFunctions.DiffDays(slots.StartDate, ToDate) > 0))
                               select slots).ToList();


            var Slots = (from slots in db.AppointmentSlots
                         where slots.IsActive == true && slots.GymId == GymId
                         && slots.BranchId == branchId
                         && ((!slots.EndDate.HasValue)
                                   || (DbFunctions.DiffDays(slots.EndDate, ToDate) >= 0 && DbFunctions.DiffDays(slots.EndDate, FromDate) <= 0)
                                   || (DbFunctions.DiffDays(slots.EndDate, ToDate) < 0 && DbFunctions.DiffDays(slots.StartDate, ToDate) > 0))
                         select slots).ToList();

            var DeletedSlots = (from slots in db.DeletAppointmentSlots
                                where slots.GymId == GymId
                                && slots.BranchId == branchId
                                && DbFunctions.DiffDays(slots.SlotDate, FromDate) <= 0
                                && DbFunctions.DiffDays(slots.SlotDate, ToDate) >= 0
                                select slots).ToList();


            if ((BookesSlots != null && BookesSlots.Count > 0) || (Slots != null && Slots.Count > 0))
            {
                List<AppointmentSlotModel> lstSlots = new List<AppointmentSlotModel>();

                if (BookesSlots != null && BookesSlots.Count > 0)
                {
                    foreach (var Slot in BookesSlots)
                    {
                        if (Slot != null && Slot.Pattern != null)
                        {
                            string strAppTitle = Slot.Appointment.Title;
                            string strMemberName = Slot.MemberInfo.FirstName + " " + Slot.MemberInfo.LastName;
                            string strTitle = strAppTitle + " with " + strMemberName;
                            int staffId = Slot.Staff.StaffId;
                            var schedule = CrontabSchedule.Parse(Slot.Pattern);
                            DateTime StartDate = Slot.StartDate.HasValue ? Convert.ToDateTime(Slot.StartDate) > FromDate ? Convert.ToDateTime(Slot.StartDate) : FromDate : FromDate;
                            DateTime EndDate = Slot.EndDate.HasValue ? Convert.ToDateTime(Slot.EndDate).AddDays(1) < ToDate ? Convert.ToDateTime(Slot.EndDate).AddDays(1) : ToDate.AddDays(1) : ToDate.AddDays(1);
                            var nextSchdule = schedule.GetNextOccurrences(StartDate, EndDate);

                            foreach (var nextDate in nextSchdule)
                            {
                                AppointmentSlotModel objAppointmentSlotModel = new AppointmentSlotModel();
                                objAppointmentSlotModel.AppointmentSlotId = Slot.BookingId;
                                objAppointmentSlotModel.AppSlotID = Slot.AppSlotId.HasValue ? Convert.ToInt32(Slot.AppSlotId) : 0;
                                objAppointmentSlotModel.Pattern = Slot.Pattern;
                                objAppointmentSlotModel.StartDate = nextDate;
                                objAppointmentSlotModel.EndDate = nextDate.AddMinutes((double)Slot.MeetingDuration);
                                objAppointmentSlotModel.Title = strTitle;
                                objAppointmentSlotModel.StaffName = "";
                                objAppointmentSlotModel.StaffId = staffId;
                                objAppointmentSlotModel.TotalDuration = (int)Slot.MeetingDuration;
                                objAppointmentSlotModel.UsedDuration = (int)Slot.MeetingDuration;
                                objAppointmentSlotModel.MinDuration = (int)Slot.MeetingDuration;
                                objAppointmentSlotModel.isBooked = true;
                                objAppointmentSlotModel.Color = "#00b100";
                                objAppointmentSlotModel.BranchId = Slot.BranchId.HasValue ? Convert.ToInt32(Slot.BranchId) : 0;
                                lstSlots.Add(objAppointmentSlotModel);
                            }
                        }
                    }
                }

                if (Slots != null && Slots.Count > 0)
                {
                    foreach (var Slot in Slots)
                    {
                        if (Slot != null && Slot.AppointmentSlotPatterns != null && Slot.AppointmentSlotPatterns.Count > 0)
                        {
                            string strStaffName = Slot.Staff.FirstName + " " + Slot.Staff.LastName;
                            string strTitle = Title(Slot.AppointmentSlotConfigs) + "(" + strStaffName + ")";
                            int staffId = Slot.Staff.StaffId;

                            foreach (var SlotPattern in Slot.AppointmentSlotPatterns)
                            {
                                var schedule = CrontabSchedule.Parse(SlotPattern.Pattern);
                                DateTime StartDate = Slot.StartDate.HasValue ? Convert.ToDateTime(Slot.StartDate) > FromDate ? Convert.ToDateTime(Slot.StartDate) : FromDate : FromDate;
                                DateTime EndDate = Slot.EndDate.HasValue ? Convert.ToDateTime(Slot.EndDate).AddDays(1) < ToDate ? Convert.ToDateTime(Slot.EndDate).AddDays(1) : ToDate.AddDays(1) : ToDate.AddDays(1);
                                var nextSchdule = schedule.GetNextOccurrences(StartDate, EndDate);

                                foreach (var nextDate in nextSchdule)
                                {
                                    var Data = lstSlots.Where(x => x.StartDate.Subtract(nextDate).Duration().Days == 0 && x.StartDate.Subtract(nextDate).Duration().TotalMinutes <= 0 && x.EndDate.Subtract(nextDate).Duration().TotalMinutes > 0 && x.AppSlotID == Slot.SlotId && x.isBooked).ToList();

                                    if (Data != null && Data.Count == 0)
                                    {
                                        var isDeleted = false;
                                        if (DeletedSlots != null && DeletedSlots.Count > 0)
                                        {
                                            var Deleted = DeletedSlots.Where(x => Convert.ToDateTime(x.SlotDate).Subtract(nextDate).Days == 0 && x.AppSlotId == Slot.SlotId).ToList();

                                            if (Deleted != null && Deleted.Count > 0)
                                            {
                                                isDeleted = true;
                                            }
                                        }

                                        if (!isDeleted)
                                        {
                                            AppointmentSlotModel objAppointmentSlotModel = new AppointmentSlotModel();
                                            objAppointmentSlotModel.AppointmentSlotId = Slot.SlotId;
                                            objAppointmentSlotModel.AppSlotID = Slot.SlotId;
                                            objAppointmentSlotModel.Pattern = SlotPattern.Pattern;
                                            objAppointmentSlotModel.StartDate = nextDate;
                                            objAppointmentSlotModel.EndDate = nextDate.AddMinutes((double)SlotPattern.SlotDuration);
                                            objAppointmentSlotModel.Title = strTitle;
                                            objAppointmentSlotModel.StaffName = strStaffName;
                                            objAppointmentSlotModel.StaffId = staffId;
                                            objAppointmentSlotModel.TotalDuration = (int)Slot.TotalDuaration;
                                            objAppointmentSlotModel.UsedDuration = GettingUsedDuration(Slot.SlotId,nextDate);
                                            objAppointmentSlotModel.MinDuration = (int)Slot.MinDuration;
                                            objAppointmentSlotModel.isBooked = false;
                                            objAppointmentSlotModel.Color = "#c3d9ff";
                                            objAppointmentSlotModel.BranchId = Slot.BranchId.HasValue ? Convert.ToInt32(Slot.BranchId) : 0;
                                            lstSlots.Add(objAppointmentSlotModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return Json(lstSlots, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private string Title(ICollection<AppointmentSlotConfig> appointmentSlotConfig)
        {

            if (appointmentSlotConfig != null && appointmentSlotConfig.Count > 0)
            {

                string Title = "";
                foreach (var objAppointmentSlotConfig in appointmentSlotConfig)
                {
                    Title += objAppointmentSlotConfig.Appointment.Title + ",";
                }
                return Title != "" ? Title.Substring(0, Title.LastIndexOf(",")) : Title;
            }
            return "";
        }

        public JsonResult AppointConfigBySlotId(int SlotId)
        {
            var slotConfigs = (from slotConfig in db.AppointmentSlotConfigs
                               where slotConfig.SlotId == SlotId
                               select slotConfig).ToList();
            if (slotConfigs != null && slotConfigs.Count > 0)
            {
                List<AppointmentConfigViewModel> lstAppointmentConfigViewModel = new List<AppointmentConfigViewModel>();

                foreach (var objAppointmentSlotConfig in slotConfigs)
                {
                    AppointmentConfigViewModel objAppointmentConfigViewModel = new AppointmentConfigViewModel();

                    objAppointmentConfigViewModel.Title = objAppointmentSlotConfig.Appointment.Title;
                    objAppointmentConfigViewModel.TaxType = objAppointmentSlotConfig.Appointment.TaxType.HasValue ? (int)objAppointmentSlotConfig.Appointment.TaxType : 0;
                    objAppointmentConfigViewModel.TaxPercentage = objAppointmentSlotConfig.Appointment.TaxPercentage.HasValue ? (decimal)objAppointmentSlotConfig.Appointment.TaxPercentage : 0;
                    objAppointmentConfigViewModel.AppointmentId = objAppointmentSlotConfig.Appointment.AppointmentId;
                    lstAppointmentConfigViewModel.Add(objAppointmentConfigViewModel);
                }
                return Json(lstAppointmentConfigViewModel, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BookAppointment(BookAppointment objBookAppointment)
        {
            if (objBookAppointment != null)
            {
                objBookAppointment.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                objBookAppointment.IsBillsDeleted = false;

                if (objBookAppointment.BookingId != 0)
                {
                    db.AppointmentBills.RemoveRange(db.AppointmentBills.Where(c => c.BookingId == objBookAppointment.BookingId));
                    db.SaveChanges();

                    db.AppointmentBills.AddRange(objBookAppointment.AppointmentBills);
                    db.SaveChanges();
                }

                db.Entry(objBookAppointment).State = objBookAppointment.BookingId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { Success = "True", Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GenerateBill(string startDate, string endDate, string memberId, string configId, string duration, string title)
        {
            if (!string.IsNullOrEmpty(memberId) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(configId) && !string.IsNullOrEmpty(duration))
            {
                int _memberId = Convert.ToInt32(memberId);
                int _duration = Convert.ToInt32(duration);
                int _configId = Convert.ToInt32(configId);
                var membershipEndDate = (from mship in db.MemberShips
                                         where mship.MemberID == _memberId
                                         select mship).Max(d => d.EndDate);

                var _slab = (from slab in db.AppointmentSlabs
                             where slab.AppointmentId == _configId
                             && (slab.Minutes - _duration) <= 0
                             orderby slab.Minutes
                             select slab).FirstOrDefault();

                if (_slab != null)
                {
                    DateTime dtStartDate = Convert.ToDateTime(startDate);
                    var weekday = (int)dtStartDate.DayOfWeek;

                    DateTime dtEndDate = dtStartDate;

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        dtEndDate = Convert.ToDateTime(endDate);
                    }

                    var schedule = CrontabSchedule.Parse("30 00 * * " + weekday);
                    var allBillDates = schedule.GetNextOccurrences(dtStartDate, dtEndDate.AddDays(1));

                    List<AppointmentBillModel> Bills = new List<AppointmentBillModel>();

                    foreach (var billDate in allBillDates)
                    {
                        var price = (Convert.ToDecimal(duration) / (decimal)_slab.Minutes) * _slab.Price;

                        AppointmentBillModel objAppointmentBillModel = new AppointmentBillModel();
                        objAppointmentBillModel.Date = billDate.ToString("MMM d, yyyy");
                        objAppointmentBillModel.Title = title;
                        objAppointmentBillModel.Amount = price.ToString();

                        Bills.Add(objAppointmentBillModel);
                    }

                    return Json(new { Status = "Success", Message = "", Data = Bills }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "Failed", Message = "No Pricing Slots Available In Appoinment Config", Data = new { } }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Status = "Failed", Message = "Any Of these information is missed (When, Client or Appointment Config)", Data = new { } }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SlotDetailsById(int SlotId)
        {
            var SlotDetails = (from slot in db.AppointmentSlots
                               where slot.SlotId == SlotId
                               select slot).SingleOrDefault();
            if (SlotDetails != null)
            {
                var SlotInfo = new SlotModel()
                {
                    StaffId = SlotDetails.Staff.StaffId.ToString(),
                    StaffName = SlotDetails.Staff.FirstName + " " + SlotDetails.Staff.LastName,
                    ConfigIds = ConfigIds(SlotDetails.AppointmentSlotConfigs),
                    StartDate = SlotDetails.StartDate.HasValue ? Convert.ToDateTime(SlotDetails.StartDate) : DateTime.Now,
                    EndDate = SlotDetails.EndDate,
                    IsRepeat = SlotDetails.IsRepeat.HasValue ? Convert.ToBoolean(SlotDetails.IsRepeat) : false,
                    MinDuration = SlotDetails.MinDuration.HasValue ? Convert.ToInt32(SlotDetails.MinDuration) : 0,
                    TotalDuration = SlotDetails.TotalDuaration.HasValue ? Convert.ToInt32(SlotDetails.TotalDuaration) : 0,
                    SlotType = SlotDetails.SlotType.HasValue ? Convert.ToInt32(SlotDetails.SlotType) : 0,
                    RepeatsOn = SlotDetails.AppointmentSlotPatterns.ElementAt(0).Pattern.Replace(SlotDetails.AppointmentSlotPatterns.ElementAt(0).Pattern.Substring(0, SlotDetails.AppointmentSlotPatterns.ElementAt(0).Pattern.LastIndexOf(" ")), "").Trim()
                };

                SlotInfo.StartDate = GetStartDate(SlotDetails.AppointmentSlotPatterns.ElementAt(0).Pattern, SlotInfo.StartDate);
                SlotInfo.strStartDate = JsonConvert.SerializeObject(SlotInfo.StartDate, new IsoDateTimeConverter());

                return Json(SlotInfo, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private string ConfigIds(ICollection<AppointmentSlotConfig> appointmentSlotConfig)
        {

            if (appointmentSlotConfig != null && appointmentSlotConfig.Count > 0)
            {

                string ConfigIds = "";
                foreach (var objAppointmentSlotConfig in appointmentSlotConfig)
                {
                    ConfigIds += objAppointmentSlotConfig.Appointment.AppointmentId + ",";
                }
                return ConfigIds != "" ? ConfigIds.Substring(0, ConfigIds.LastIndexOf(",")) : ConfigIds;
            }
            return "";
        }

        private DateTime GetStartDate(string Repeatable, DateTime StartDate)
        {
            var schedule = CrontabSchedule.Parse(Repeatable);

            var nextSchdule = schedule.GetNextOccurrences(StartDate, StartDate.AddDays(1)).ToList();

            foreach (var startDate in nextSchdule)
            {
                StartDate = startDate;
            }

            return StartDate;
        }

        public JsonResult DeleteSlot(int SlotId, int Type, DateTime SlotDate, int branchId)
        {
            if (SlotId != 0)
            {
                if (Type == 1)
                {
                    int GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                    var delAppSlot = new DeletAppointmentSlot()
                    {
                        AppSlotId = SlotId,
                        SlotDate = SlotDate,
                        GymId = GymId,
                        BranchId = branchId
                    };
                    db.DeletAppointmentSlots.Add(delAppSlot);
                    db.SaveChanges();

                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else if (Type == 2 || Type == 3)
                {
                    var appSlot = db.AppointmentSlots.Find(SlotId);
                    if (Type == 2)
                    {
                        appSlot.EndDate = SlotDate.AddDays(-1);
                    }
                    else if (Type == 3)
                    {
                        appSlot.IsActive = false;
                    }
                    db.Entry(appSlot).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GettingBookingDetailsById(int BookingId)
        {
            if (BookingId != 0)
            {
                var booking = (from book in db.BookAppointments
                               where book.BookingId == BookingId
                               select book).SingleOrDefault();

                if (booking != null)
                {
                    var bookingDetails = new
                    {
                        Client = booking.MemberInfo.FirstName + " " + booking.MemberInfo.LastName,
                        With = booking.Staff.FirstName + " " + booking.Staff.LastName,
                        Appointment = booking.Appointment.Title,
                        When = GetStartDate(booking.Pattern, Convert.ToDateTime(booking.StartDate)),
                        IsRepatable = booking.IsRepatable,
                        RepeatsUntil = booking.EndDate,
                        Length = booking.MeetingDuration,
                        Bills = Bills(booking.AppointmentBills)
                    };

                    return Json(new { Status = "Success", Message = "", Data = bookingDetails }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Status = "Failure", Message = "Unable to retrieve information.", Data = new { } }, JsonRequestBehavior.AllowGet);
        }

        public List<AppointmentBillModel> Bills(ICollection<AppointmentBill> AppointmentBills)
        {
            List<AppointmentBillModel> Bills = new List<AppointmentBillModel>();

            if (AppointmentBills != null && AppointmentBills.Count > 0)
            {
                foreach (AppointmentBill objBill in AppointmentBills)
                {
                    AppointmentBillModel objAppointmentBillModel = new AppointmentBillModel();

                    objAppointmentBillModel.Date = Convert.ToDateTime(objBill.BillDate).ToString("MMM d, yyyy");
                    objAppointmentBillModel.Title = objBill.Description;
                    objAppointmentBillModel.Amount = objBill.Amount.ToString();
                    Bills.Add(objAppointmentBillModel);
                }
            }

            return Bills;
        }

        public JsonResult DeleteBooking(int BookId, bool BillToDelete)
        {
            if (BookId != 0)
            {
                var Booking = db.BookAppointments.Find(BookId);
                Booking.IsActive = false;
                Booking.IsBillsDeleted = BillToDelete;
                db.Entry(Booking).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }

        public int GettingUsedDuration(int slotId, DateTime SlotDate)
        {
            var usedDuration = (from slots in db.BookAppointments
                                where slots.IsActive == true
                                 && slots.AppSlotId == slotId
                                && DbFunctions.DiffDays(SlotDate, slots.StartDate) >= 0
                                && DbFunctions.DiffDays(SlotDate, slots.EndDate) <= 0
                                select new
                                {
                                    MeetingDuration = slots.MeetingDuration
                                }).ToList();

            var sum = 0;

            if (usedDuration != null && usedDuration.Count>0)
            {
                sum = usedDuration.Sum(item => Convert.ToInt32(item.MeetingDuration));

            }

            return sum;
        }
    }
}
