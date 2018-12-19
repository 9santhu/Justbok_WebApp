using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.ADModel;


namespace Justbok.Controllers
{
    public class UserDashBoardController : LayoutBaseModel
    {
        //
        // GET: /UserDashBoard/

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetMemberInfo(string MemberId )
        {
          var  memberId = Convert.ToInt32(MemberId);
            var memberinfo = (from m in db.MemberInfoes
                            join s in db.MemberShips on m.MemberID equals s.MemberID
                              where m.MemberID == memberId

                            select new
                            {
                                MemberID = m.MemberID,
                                FirstName = m.FirstName + " " + m.LastName,
                                MobileNumber = m.MobileNumber,
                                EnrollDate = m.EnrollDate.ToString(),
                                Package = s.MembershipType,
                                Status = s.Status

                            }).ToList();
            return Json(memberinfo, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMemberImage(string MemberId)
        {
            var memberId = Convert.ToInt32(MemberId);
            var memberimage = (from m in db.MemberImages
                              where m.MemberId == memberId
                              select new
                              {
                                  ImageData = m.ImageData

                              }).ToList();
            return Json(memberimage, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UserWorkout()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        [HttpGet]
        public JsonResult EditMemberWorkoutPlan(string Memberid)
        {
            int memberid = Convert.ToInt32(Memberid);
            var workouts = (from mi in db.WorkoutPlanNames
                            join ms in db.MemberWorkoutPlans on mi.PlaneNameId equals ms.PlaneNameId
                            where ms.MemberID == memberid

                            select new
                            {
                                PlaneNameId = mi.PlaneNameId,
                                PlanName = mi.PlanName,
                                MemberWorkoutPlanid = ms.MemberWorkoutPlanid,
                                Workout = ms.Workout,
                                NumberOfSets = ms.NumberOfSets,
                                NumberOfMinutes = ms.NumberOfMinutes,
                                NumberofDays = ms.NumberofDays,
                                ExcerciseOrder = ms.ExcerciseOrder,
                                Repeats = ms.Repeats
                            }).ToList();


            return Json(workouts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserDiet()
        {

            return View();
        }

        [HttpGet]
        public JsonResult EditMemberDietPlan(string Memberid)
        {
            int memberid = Convert.ToInt32(Memberid);
            var diets = (from mi in db.DietPlanNames
                         join ms in db.MemberDietPlans on mi.DietPlanId equals ms.DietPlanId
                         where ms.MemberID == memberid

                         select new
                         {
                             DietPlanId = mi.DietPlanId,
                             DietPlanName1 = mi.DietPlanName1,
                             MemberDietPlanid = ms.MemberDietPlanid,
                             DietTime = ms.DietTime,
                             MondayDiet = ms.MondayDiet,
                             TuesdayDiet = ms.TuesdayDiet,
                             WednesdayDiet = ms.WednesdayDiet,
                             ThursdayDiet = ms.ThursdayDiet,
                             FridayDiet = ms.FridayDiet,
                             SaturdayDiet = ms.SaturdayDiet,
                             SundayDiet = ms.SundayDiet
                         }).ToList();


            return Json(diets, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult BindPayment(string membershipid)
        //{
        //    int membership = Convert.ToInt32(membershipid);
        //    var payments = db.Payments
        //        .Where(x => x.MemberID == membership)
        //        .Select(a => new
        //        {
        //            PaymentDate = a.PaymentDate.ToString(),
        //            PaidAmount = a.PaymentAmount,
        //            PaymentType = a.PaymentType,
        //            RecieptNumber = a.RecieptNumber,
        //            PaymentDueDate = a.PaymentDueDate.ToString(),
        //            PaymentAmount = a.PaymentAmount,
        //            ReferenceNumber = a.ReferenceNumber

        //        }).ToList();

        //    return Json(payments, JsonRequestBehavior.AllowGet);
        //}
    }
}
