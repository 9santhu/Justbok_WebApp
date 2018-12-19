using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.Globalization;
using Justbok.ADModel;
using log4net;


namespace Justbok.Controllers
{
    public class TimeSheetController : LayoutBaseModel
    {
        //
        // GET: /TimeSheet/
        JustbokEntities db = new JustbokEntities();
        private static readonly ILog Log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //Declaring Log4Net 

        public ActionResult StaffTimeSheet()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult GetMemberList(string Staffdate,int BranchId)
        {

            Log.Info("Get MmeberList");
            Log.Info(Staffdate);
            DateTime date = new DateTime();
            date = Convert.ToDateTime(Staffdate);
            Log.Info(date.ToString("dd/mm/yyyy",CultureInfo.InvariantCulture));
            //Utility.Logger.WriteLog("Date format:"+date);
            List<NewTimeSheet> listTimeSheet = new List<NewTimeSheet>();
            try
            {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            NewTimeSheet objNewTimeSheet = new NewTimeSheet();

            //DateTime test = DateTime.ParseExact(date, "dd/mm/yyyy", CultureInfo.InvariantCulture);
               //string cdate = ConvertDate(date);
               //DateTime formatdate = stringToDateConversion(date, "MM/dd/yyyy");
           // DateTime dt = DateTime.ParseExact(date.ToString(), "dd/mm/yyyy", CultureInfo.InvariantCulture);
            string dt = date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString();
            DateTime convertDate = date.Date;
            DateTime cdate = date;
            DateTime formatdate = date;


               var Members = (from mi in db.Staffs
                              join sb in db.StaffBranches on mi.StaffId equals sb.StaffId
                              where mi.GymId == gymid && sb.BranchId == BranchId
                              select new
                              {
                                  StaffId = mi.StaffId,
                                  FirstName = mi.FirstName,
                                  LastName = mi.LastName
                              }).ToList();

               foreach (var timesheet in Members)
               {

                   int getmemberid = Convert.ToInt32(timesheet.StaffId);
                   var attendecelist = (from ts in db.TimeSheets
                                        where ts.StaffId == getmemberid && ts.AttendenceDate == formatdate
                                        select new
                                        {
                                            TimeSheetId = ts.TimeSheetId,
                                            AttendenceDate = cdate,
                                            Absent = ts.Absent,
                                            Present = ts.Present,
                                            //InTime = p.StaffInTime,
                                            //OutTime = p.StaffOutTime,


                                        }).ToList();

                   if (attendecelist.Count > 0)
                   {
                       int presentId = Convert.ToInt32(attendecelist[0].Present);
                       if (attendecelist[0].Present != null)
                       {
                           var PresesntList = (from p in db.StaffPresentDetails
                                               where p.PresentId == presentId
                                               select new
                                               {

                                                   TimeIn = p.StaffInTime,
                                                   TimeOut = p.StaffOutTime
                                               }
                                               ).ToList();

                           listTimeSheet.Add(new NewTimeSheet()
                       {
                           TimeSheetId=attendecelist[0].TimeSheetId,
                           StaffId = timesheet.StaffId,
                           FirstName = timesheet.FirstName,
                           LastName = timesheet.LastName,
                           AttendenceDate = cdate,
                           Present = attendecelist[0].Present.ToString(),
                           Leave = attendecelist[0].Absent.ToString(),
                           InTime = PresesntList[0].TimeIn,
                           OutTime = PresesntList[0].TimeOut,
                           LeaveDetails = null
                       });
                       }
                       else if (attendecelist[0].Absent != null)
                       {
                           int absentId = Convert.ToInt32(attendecelist[0].Absent);
                           var absentList = (from p in db.StaffAbsentDetails
                                             join l in db.LeaveTypes on p.LeaveId equals l.LeaveTypeId
                                             where p.AbsentId == absentId
                                             select new
                                             {

                                                 LeaveName = l.LeaveName,
                                                 LeaveDay = l.LeaveDay,
                                             }
                                               ).ToList();

                           listTimeSheet.Add(new NewTimeSheet()
                       {
                           TimeSheetId = attendecelist[0].TimeSheetId,
                           StaffId = timesheet.StaffId,
                           FirstName = timesheet.FirstName,
                           LastName = timesheet.LastName,
                           AttendenceDate = cdate,
                           Present = attendecelist[0].Present.ToString(),
                           Leave = attendecelist[0].Absent.ToString(),
                           InTime = null,
                           OutTime = null,
                           LeaveDetails = absentList[0].LeaveDay + " | " + absentList[0].LeaveName
                       });
                       }
                       else
                       {
                           listTimeSheet.Add(new NewTimeSheet()
                           {
                               StaffId = timesheet.StaffId,
                               FirstName = timesheet.FirstName,
                               LastName = timesheet.LastName,
                               AttendenceDate = cdate,
                               Present = null,
                               InTime = null,
                               OutTime = null,
                               Leave = null
                           }
                        );
                       }
                   }
                   else
                   {
                       listTimeSheet.Add(new NewTimeSheet()
                       {
                           StaffId = timesheet.StaffId,
                           FirstName = timesheet.FirstName,
                           LastName = timesheet.LastName,
                           AttendenceDate = cdate,
                           Present = null,
                           InTime = null,
                           OutTime = null,
                           Leave = null
                       }
                      );
                   }
               }
            }
            catch (Exception ex)
            {
                //Utility.Logger.SendErrorToText(ex);
            }
            return Json(listTimeSheet, JsonRequestBehavior.AllowGet);
           
        }

        [HttpPost]
        public JsonResult AddUpdateAttendence(NewTimeSheet timesheet)
        {
            Log.Info("AddUpdateAttendence Started");
            Log.Info(timesheet.AttendenceDate);
            //Utility.Logger.WriteLog("");
            TimeSheet objTimesheet = new TimeSheet();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int memberid = Convert.ToInt32(timesheet.StaffId);


            if (timesheet.Leave != null)
            {
                int absentid = Convert.ToInt32(timesheet.Leave);
                if (absentid > 0)
                {
                    var absent = db.StaffAbsentDetails.FirstOrDefault(s => s.AbsentId == absentid);
                    if (absent != null)
                    {
                        db.StaffAbsentDetails.Remove(absent);
                        db.SaveChanges();
                        objTimesheet.Absent = null;
                    }

                }
            }
           

            if (timesheet.TimeSheetId >0)
            {
                //update here
                long timesheeetid = Convert.ToInt64(timesheet.TimeSheetId);

                //long presentId=Convert.ToInt64(timesheet.Present);
                objTimesheet = AddTimesheet(timesheet);
                objTimesheet.TimeSheetId = timesheeetid;
                objTimesheet.GymId = gymid;
                //objTimesheet.Present = presentId;
                objTimesheet.BranchId = Convert.ToInt32(timesheet.BranchId);
                db.Entry(objTimesheet).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            { 
            //Add new 
               
                objTimesheet = AddTimesheet(timesheet);
                objTimesheet.GymId = gymid;
                objTimesheet.BranchId = Convert.ToInt32(timesheet.BranchId);
                db.TimeSheets.Add(objTimesheet);
                db.SaveChanges();
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private TimeSheet AddTimesheet(NewTimeSheet timesheet)
        {
            TimeSheet objTimesheet = new TimeSheet();
            try
            {
                //objTimesheet.StaffId = timesheet.StaffId;
                //objTimesheet.AttendenceDate = timesheet.AttendenceDate;


                if (timesheet.Present != null && timesheet.Present!="Yes")
                {
                    int presentId=Convert.ToInt32(timesheet.Present);
                    //update here
                    objTimesheet.StaffId = timesheet.StaffId;
                    //objTimesheet.AttendenceDate = DateTime.Parse(timesheet.AttendenceDate);
                    objTimesheet.AttendenceDate = timesheet.AttendenceDate;
                    StaffPresentDetail StaffPresent = new StaffPresentDetail();
                    if (timesheet.InTime != null && timesheet.InTime != "" && timesheet.InTime.Trim() != "In")
                    {
                        StaffPresent.StaffInTime = timesheet.InTime;
                    }
                    
                    if (timesheet.OutTime != null && timesheet.OutTime != "" && timesheet.OutTime.Trim() != "Out")
                    {
                        StaffPresent.StaffOutTime = timesheet.OutTime;
                    }
                    //StaffPresent.StaffOutTime = timesheet.OutTime;
                    StaffPresent.ShiftId = 400001;
                    StaffPresent.StaffId = timesheet.StaffId;
                    StaffPresent.PresentId = presentId;
                    
                    db.Entry(StaffPresent).State = EntityState.Modified;
                    db.SaveChanges();
                    objTimesheet.Present = presentId;
                 }
                else
                { 
                  //add new
                    objTimesheet.StaffId = timesheet.StaffId;

                    //DateTime date = DateTime.ParseExact(timesheet.AttendenceDate, "mm/dd/yyyy", CultureInfo.InvariantCulture);
                    //objTimesheet.AttendenceDate =DateTime.Parse(timesheet.AttendenceDate);
                   // objTimesheet.AttendenceDate = date;
                    objTimesheet.AttendenceDate =timesheet.AttendenceDate;
                    StaffPresentDetail StaffPresent = new StaffPresentDetail();
                    if (timesheet.InTime != null && timesheet.InTime != "" && timesheet.InTime.Trim() != "In")
                    {
                        StaffPresent.StaffInTime = timesheet.InTime;
                    }
                    if (timesheet.OutTime != null && timesheet.OutTime != "" && timesheet.OutTime.Trim() != "Out")
                    {
                        StaffPresent.StaffOutTime = timesheet.OutTime;
                    }

                    //This was hard coded because of morning shift
                    StaffPresent.ShiftId = 400001;
                    StaffPresent.StaffId = timesheet.StaffId;
                    db.StaffPresentDetails.Add(StaffPresent);
                    db.SaveChanges();
                    objTimesheet.Present = StaffPresent.PresentId;
                   
                }

            }
            catch (Exception ex)
            {

            }
            return objTimesheet;
        }

        public JsonResult AddLeave(NewTimeSheet timesheet)
        {
            TimeSheet objTimesheet = new TimeSheet();
            bool success = false;
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            try
            {
               
                if (timesheet.Leave!=null)
                {
                     //Update absent
                    StaffAbsentDetail absent = new StaffAbsentDetail();
                    objTimesheet.TimeSheetId = timesheet.TimeSheetId;
                    //objTimesheet.AttendenceDate = DateTime.Parse(timesheet.AttendenceDate);
                    objTimesheet.AttendenceDate = timesheet.AttendenceDate;
                    int absentid = Convert.ToInt32(timesheet.Leave);
                    if (absentid>0)
                    {
                        //Update StaffAbsent details
                        objTimesheet.Absent = absentid;
                        absent.AbsentId = absentid;
                        absent.LeaveType = timesheet.LeaveType;
                        absent.LeaveId = timesheet.LeaveId;
                        absent.Reason = timesheet.Reason;
                        absent.StaffId = timesheet.StaffId;
                        db.Entry(absent).State = EntityState.Modified;
                        db.SaveChanges();
                      

                      
                    }
                    else {
                        //new StaffAbsent details
                        absent.LeaveType = timesheet.LeaveType;
                        absent.LeaveId = timesheet.LeaveId;
                        absent.Reason = timesheet.Reason;
                        absent.StaffId = timesheet.StaffId;
                        db.StaffAbsentDetails.Add(absent);
                        db.SaveChanges();
                        int absentNewId=Convert.ToInt32(absent.AbsentId);
                        objTimesheet.Absent = absentNewId;

                    }
                    //Delete Present Details
                    if(timesheet.Present!=null)
                    {
                    int presentId = int.Parse(timesheet.Present);
                    if (presentId > 0)
                    {
                        var presentDetails = db.StaffPresentDetails.FirstOrDefault(s => s.PresentId == presentId);
                        if (presentDetails != null)
                        {
                            db.StaffPresentDetails.Remove(presentDetails);
                            db.SaveChanges();
                        }
                    }
                    objTimesheet.Present = null;
                    }
                    
                    objTimesheet.GymId = gymid;
                    objTimesheet.BranchId = Convert.ToInt32(timesheet.BranchId);
                    objTimesheet.StaffId = timesheet.StaffId;
                    db.Entry(objTimesheet).State = EntityState.Modified;
                    db.SaveChanges();
                    
                }
                else
                { 
                   //Add New Absent 
                    StaffAbsentDetail absent = new StaffAbsentDetail();
                    objTimesheet.TimeSheetId = timesheet.TimeSheetId;
                    //objTimesheet.AttendenceDate = DateTime.Parse(timesheet.AttendenceDate);
                    objTimesheet.AttendenceDate = timesheet.AttendenceDate;
                    absent.LeaveType = timesheet.LeaveType;
                    absent.LeaveId = timesheet.LeaveId;
                    absent.Reason = timesheet.Reason;
                    absent.StaffId = timesheet.StaffId;
                    db.StaffAbsentDetails.Add(absent);
                    db.SaveChanges();
                    int absentNewId = Convert.ToInt32(absent.AbsentId);
                    objTimesheet.Absent = absentNewId;
                    //delete present details
                    if(timesheet.Present!=null)
                    {
                        int presentid = int.Parse(timesheet.Present);

                        if (presentid > 0)
                        {
                            var presentDetails = db.StaffPresentDetails.FirstOrDefault(s => s.PresentId == presentid);
                            if (presentDetails != null)
                            {
                                db.StaffPresentDetails.Remove(presentDetails);
                                db.SaveChanges();
                            }
                        }

                        //Adding to timeseet
                        objTimesheet.Present = null;

                    }

                    
                    objTimesheet.GymId = gymid;
                    objTimesheet.BranchId = Convert.ToInt32(timesheet.BranchId);
                    objTimesheet.StaffId = timesheet.StaffId;
                    if (objTimesheet.TimeSheetId > 0)
                    {
                        db.Entry(objTimesheet).State = EntityState.Modified;
                        db.SaveChanges();
                        success = true;
                    }
                    else
                    {
                        db.TimeSheets.Add(objTimesheet);
                        db.SaveChanges();
                        success = true;
                    }
                  
                }
            }
            catch
            {
                success = false;
            }
            return Json(new { success}, JsonRequestBehavior.AllowGet);
        }

        enum Months
        {
            January = 01,
            February = 02,
            March = 03,
            April = 04,
            May = 05,
            June =06,
            July = 07,
            August = 08,
            September = 09,
            October = 10,
            November = 11,
            December = 12
            
        }

        private string ConvertDate(string date)
        {
            string cdate = "";

            try
            {
                var splitDate = date.Split('/');
                Months animal = (Months)Enum.Parse(typeof(Months), splitDate[1], true); // case insensitive
                cdate = splitDate[1] + "/" + splitDate[0] + "/" + splitDate[2];
            }
            catch (Exception ex)
            {

            }
            return cdate;
        }

        public DateTime stringToDateConversion(string date, string format)
        {
            /* Convert Date to Currrnt Culture */
            DateTimeFormatInfo dateTimeFormatterProvider = DateTimeFormatInfo.CurrentInfo.Clone() as DateTimeFormatInfo;
            dateTimeFormatterProvider.ShortDatePattern = format; //source date format
            DateTime NewDate = DateTime.Parse(date, dateTimeFormatterProvider);
            return NewDate;
        }

        public ActionResult MemberTimeSheet()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        public JsonResult MembersAttendanceList(DateTime date,int BranchId)
        {
            //Utility.Logger.WriteLog("Date format:" + date);
            List<NewTimeSheet> listTimeSheet = new List<NewTimeSheet>();
            try
            {
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
                NewTimeSheet objNewTimeSheet = new NewTimeSheet();
                //string cdate = ConvertDate(date);
                //DateTime formatdate = stringToDateConversion(date, "MM/dd/yyyy");
                //Utility.Logger.WriteLog("convert to date:" + formatdate);
                DateTime cdate = date;
                DateTime formatdate = date;
                var Members = (from mi in db.MemberInfoes
                               where mi.GymId == gymid && mi.BranchId == BranchId
                               select new
                               {
                                   MemberID = mi.MemberID,
                                   FirstName = mi.FirstName,
                                   LastName = mi.LastName,
                                   MobileNumber=mi.MobileNumber
                               }).ToList();

                foreach (var timesheet in Members)
                {

                    int getmemberid = Convert.ToInt32(timesheet.MemberID);
                    var attendecelist = (from ts in db.MemberTimeSheets
                                         where ts.MemberID == getmemberid && ts.AttendenceDate == formatdate
                                         select new
                                         {
                                             AttendenceDate = cdate,
                                             Present = ts.IsPresent,
                                            
                                         }).ToList();

                    if (attendecelist.Count > 0)
                    {
                        listTimeSheet.Add(new NewTimeSheet()
                        {
                            MemberID = timesheet.MemberID,
                            FirstName = timesheet.FirstName,
                            LastName = timesheet.LastName,
                            MobileNumber = timesheet.MobileNumber,
                            AttendenceDate = cdate,
                            Present = attendecelist[0].Present
                           
                        }
                        );

                    }
                    else
                    {
                        listTimeSheet.Add(new NewTimeSheet()
                        {
                            MemberID = timesheet.MemberID,
                            FirstName = timesheet.FirstName,
                            LastName = timesheet.LastName,
                             MobileNumber = timesheet.MobileNumber,
                            AttendenceDate = cdate,
                            Present = null,
                           
                        }
                       );
                    }
                }

            }
            catch (Exception ex)
            {
                //Utility.Logger.SendErrorToText(ex);
            }
            return Json(listTimeSheet, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddUpdateMemberAttendence(MemberTimeSheet timesheet)
        {
            //Utility.Logger.WriteLog(timesheet.AttendenceDate.ToString());
            //timesheet.AttendenceDate = DateTime.Now;
            MemberTimeSheet objTimesheet = new MemberTimeSheet();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int memberid = Convert.ToInt32(timesheet.MemberID);
            var checkAttendence = (from attendence in db.MemberTimeSheets
                                   where attendence.GymId == gymid && attendence.MemberID == memberid && attendence.AttendenceDate == timesheet.AttendenceDate
                                   select attendence.MemberTimeSheetId).ToList();
            if (checkAttendence.Count > 0)
            {
                
                objTimesheet.MemberTimeSheetId = checkAttendence[0];
                objTimesheet.MemberID = timesheet.MemberID;
                if (timesheet.IsPresent != null)
                {
                    objTimesheet.IsPresent = timesheet.IsPresent.Trim();
                }
               
                objTimesheet.AttendenceDate = timesheet.AttendenceDate;
                objTimesheet.GymId = gymid;
                db.Entry(objTimesheet).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //objTimesheet.MemberTimeSheetId = checkAttendence[0];
                objTimesheet.MemberID = timesheet.MemberID;
                if (timesheet.IsPresent != null)
                {
                    objTimesheet.IsPresent = timesheet.IsPresent.Trim();
                }
                objTimesheet.AttendenceDate = timesheet.AttendenceDate;
                objTimesheet.GymId = gymid;
                db.MemberTimeSheets.Add(objTimesheet);
                db.SaveChanges();
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult MulitpleMemberAttendance(List<MemberTimeSheet> timesheet)
        {
            if (timesheet != null)
            {
                MemberTimeSheet objTimesheet = new MemberTimeSheet();
                int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

                for (int time = 0; time < timesheet.Count; time++)
                {
                    //Utility.Logger.WriteLog(timesheet[time].AttendenceDate.ToString());
                   DateTime gotDate = Convert.ToDateTime(timesheet[time].AttendenceDate);
                    int memberid = Convert.ToInt32(timesheet[time].MemberID);
                    var checkAttendence = (from attendence in db.MemberTimeSheets
                                           where attendence.GymId == gymid && attendence.MemberID == memberid && attendence.AttendenceDate == gotDate
                                           select attendence.MemberTimeSheetId).ToList();

                    if (checkAttendence.Count > 0)
                    {
                        MemberTimeSheet objTimesheetUpdate = new MemberTimeSheet();
                        objTimesheetUpdate.MemberTimeSheetId = checkAttendence[0];
                        objTimesheetUpdate.MemberID = timesheet[time].MemberID;
                        if (timesheet[time].IsPresent != null)
                        {
                            objTimesheetUpdate.IsPresent = timesheet[time].IsPresent.Trim();
                        }

                        objTimesheetUpdate.AttendenceDate = gotDate;
                        objTimesheetUpdate.GymId = gymid;
                        db.Entry(objTimesheetUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //objTimesheet.MemberTimeSheetId = checkAttendence[0];
                        objTimesheet.MemberID = timesheet[time].MemberID;
                        if (timesheet[time].IsPresent != null)
                        {
                            objTimesheet.IsPresent = timesheet[time].IsPresent.Trim();
                        }
                        objTimesheet.AttendenceDate = gotDate;
                        objTimesheet.GymId = gymid;
                        db.MemberTimeSheets.Add(objTimesheet);
                        db.SaveChanges();
                    }



                }
            
            }


          
          
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BingLeaveType()
        { 
           
            var Leaves=(from L in db.LeaveTypes
                            select L   
                            ).ToList();

            return Json(Leaves, JsonRequestBehavior.AllowGet);

        }

    }
}
