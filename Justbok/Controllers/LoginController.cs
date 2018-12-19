using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;

namespace Justbok.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        JustbokEntities db = new JustbokEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(StaffLogin objGym)
         {
             var result = new { Success = "False", Message = "Unable To Save Information." };
            //if (ModelState.IsValid)
            //{
             //var user = await db.GymLogins.FindAsync(objGym.Uname, objGym.pwd);

                var validate = (from gymLogin in db.GymLogins
                                join gymlist in db.GymLists on gymLogin.GymId equals gymlist.Gymid
                                //join staff in db.Staffs on gymlist.Gymid equals staff.GymId
                                where gymLogin.UserName == objGym.Uname && gymLogin.Password == objGym.pwd && gymLogin.IsLoginActive == "Yes"
                                select new {UserName=gymLogin.UserName ,GymFirstName = gymlist.FirstName, GymLastName = gymlist.LastName, GymId = gymlist.Gymid,  Role = gymLogin.Role,StaffId=gymLogin.StaffId,MemberId=gymLogin.MemberId}).ToList();
                if (validate.FirstOrDefault() != null)
                {
                    System.Web.HttpContext.Current.Session["LoggedGym"] = validate[0].GymId;
                    System.Web.HttpContext.Current.Session["UserName"] = validate[0].UserName;
                    SessionManger.GymId = validate[0].GymId;
                    System.Web.HttpContext.Current.Session["Role"] = validate[0].Role;
                    if (validate[0].Role.Equals("Gym"))
                    {
                        System.Web.HttpContext.Current.Session["GymName"] = validate[0].GymFirstName + " " + validate[0].GymLastName;
                        //return Redirect("/Dashboard/Index");
                        result = new { Success = "True", Message = "/Dashboard/Index" };
                      
                    }
                    else if (validate[0].Role.Equals("Admin"))
                    {
                        System.Web.HttpContext.Current.Session["GymName"] = "Admin";
                        //return Redirect("/Admin/Index");
                        //result = new { Success = "True", Message = "/Admin/Index" };
                        result = new { Success = "True", Message = "/Login/AdminDashboard" };
                        
                        
                    }
                    else if (validate[0].Role.Equals("User"))
                    {
                        System.Web.HttpContext.Current.Session["UserName"] = validate[0].MemberId;
                        result = new { Success = "True", Message = "/UserDashBoard/Index" };
                        //return Redirect("/UserDashBoard/Index");
                    }
                    else
                    {

                        if (validate[0].StaffId != null)
                        {
                            var staffID = Convert.ToInt32(validate[0].StaffId);
                            var lstStaff = (from staff in db.Staffs
                                            where staff.StaffId == staffID
                                            select new { StaffName = staff.FirstName + " " + staff.LastName }).ToList();
                            if (lstStaff != null)
                            {
                                System.Web.HttpContext.Current.Session["LoggedStaffId"] = staffID;
                                System.Web.HttpContext.Current.Session["GymName"] = lstStaff[0].StaffName;
                                //return Redirect("/Dashboard/Index");
                                result = new { Success = "True", Message = "/Dashboard/Index" };
                            }

                        }
                    }
                }
                else
                {
                    result = new { Success = "True", Message = "Invalid" };
                }            


            //}
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Admin(GymAdmin objAdmin)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var validate = (from adminLogin in db.GymAdmins
        //                        where adminLogin.AdminName == objAdmin.AdminName && adminLogin.AdminPwd == objAdmin.AdminPwd 
        //                        select adminLogin).ToList();
        //        if (validate.FirstOrDefault() != null)
        //        {
                   
        //            return Redirect("/Dashboard/AdminDashboard");
        //        }
        //        else
        //        {
        //            //
        //        }


        //    }
        //    return View();

        //}

        

    }
}
