using System.Collections.Generic;
using System.Web.Mvc;
using Justbok.Models;
using Justbok.Controllers;
using System;
using System.Linq;

namespace Justbok.ADModel
{
    [CheckSessionFilter]
    [ErrorHandler]
    public abstract class LayoutBaseModel : Controller
    {
        public JustbokEntities db = null;

        
        public LayoutBaseModel()
        {
            db = new JustbokEntities();
            if (Branches == null || Branches.Count==0)
            {
                Branches = GetBranchList();
            }

            if (StaffBranches == null || StaffBranches.Count == 0)
            {
                StaffBranches = GetStaffBranch();
            }

            if (Shifts == null || Shifts.Count == 0)
            {
                Shifts = GetStaffShift();
            }
                WorkoutPlan = GetWorkoutPlan();
                DietPlan = GetDietPlan();

            ViewBag.Branches = Branches;
            ViewBag.StaffBranch = StaffBranches;
            ViewBag.ShiftStaff = Shifts;
            ViewBag.WorkoutPlan = WorkoutPlan;
            ViewBag.DietPlan = DietPlan;

        }

        public List<AddBranch> GetBranchList()
        {
            var role = "";
            int staffId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedStaffId"]);
            if (System.Web.HttpContext.Current.Session["Role"] != null)
            {
                 role = System.Web.HttpContext.Current.Session["Role"].ToString();
            }
            
            var gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<AddBranch> listAddBranch = new List<AddBranch>();

            if (role.ToUpper().Equals("GYM"))
            {
                var branch = (from b in db.Branches
                              where b.GymId == gymid
                              select new { BranchId = b.BranchId, BranchName = b.BranchName }).ToList();

                if (branch.Count > 0)
                {
                    foreach (var br in branch)
                    {
                        listAddBranch.Add(new AddBranch()
                        {
                            BranchId = br.BranchId.ToString(),
                            BranchName = br.BranchName
                        });
                    }


                }
            }
            else
            {
                var branches = (from branch in db.StaffBranches
                                where branch.StaffId == staffId
                                select new
                                {
                                    BranchId = branch.BranchId
                                }).ToList();

                if (branches != null)
                {
                    foreach (var allBranch in branches)
                    {
                        var brnch = Convert.ToInt32(allBranch.BranchId);
                        var branch = (from b in db.Branches
                                      where b.BranchId == brnch
                                      select new { BranchId = b.BranchId, BranchName = b.BranchName }).ToList();

                        if (branch.Count > 0)
                        {
                            listAddBranch.Add(new AddBranch()
                            {
                                BranchId = branch[0].BranchId.ToString(),
                                BranchName = branch[0].BranchName
                            });

                        }
                    }
                }
            }

            return listAddBranch;
        }

        public List<AddBranchStaff> GetStaffBranch()
        {
            var gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<AddBranchStaff> lstStaffBranch = new List<AddBranchStaff>();
            var branhList = (from branch in db.Branches
                             where branch.GymId == gymid
                             select new
                             {
                                 BranchId = branch.BranchId,
                                 BranchName = branch.BranchName,

                             }).ToList();

            if (branhList.Count > 0)
            {
                foreach (var br in branhList)
                {
                    lstStaffBranch.Add(new AddBranchStaff()
                    {
                        BranchId = br.BranchId.ToString(),
                        BranchName = br.BranchName
                    });
                }


            }

            return lstStaffBranch;
        }

        public List<ShiftStaff> GetStaffShift()
        {
            var gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<ShiftStaff> lstStaffShift = new List<ShiftStaff>();
            var ShiftList = db.ShiftTypes
                .Select(a => new
                {
                    ShiftId = a.ShiftId,
                    ShiftName = a.ShiftName,
                }).ToList();

            if (ShiftList.Count > 0)
            {
                foreach (var sl in ShiftList)
                {
                    lstStaffShift.Add(new ShiftStaff()
                    {
                        ShiftId = sl.ShiftId.ToString(),
                        ShiftName = sl.ShiftName
                    });
                }


            }

            return lstStaffShift;
        }
        public List<AddWorkoutPlan> GetWorkoutPlan()
        {
            var gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var branchid = Convert.ToInt32(System.Web.HttpContext.Current.Session["BranchId"]);
            List<AddWorkoutPlan> lstPlan = new List<AddWorkoutPlan>();
            if (branchid > 0)
            {
                var plannames = (from plans in db.WorkoutPlanNames
                                 where plans.GymId == gymid && plans.BranchId == branchid
                                 select new
                                 {
                                     PlaneNameId = plans.PlaneNameId,
                                     PlanName = plans.PlanName,
                                 }).ToList();

                if (plannames.Count > 0)
                {
                    foreach (var sl in plannames)
                    {
                        lstPlan.Add(new AddWorkoutPlan()
                        {
                            PlaneNameId = sl.PlaneNameId.ToString(),
                            PlanName = sl.PlanName
                        });
                    }


                }
            }


            return lstPlan;
        }

        public List<AddDietPlan> GetDietPlan()
        {
            var gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var branchid = Convert.ToInt32(System.Web.HttpContext.Current.Session["BranchId"]);
            List<AddDietPlan> lstPlan = new List<AddDietPlan>();
            if (branchid > 0)
            {
                var DietPlans = (from plans in db.DietPlanNames
                                 where plans.GymId == gymid && plans.BranchId == branchid
                                 select new
                                 {
                                     PlaneNameId = plans.DietPlanId,
                                     PlanName = plans.DietPlanName1,
                                 }).ToList();

                if (DietPlans.Count > 0)
                {
                    foreach (var sl in DietPlans)
                    {
                        lstPlan.Add(new AddDietPlan()
                        {
                            DietPlanId = sl.PlaneNameId.ToString(),
                            DietPlanName = sl.PlanName
                        });
                    }


                }
            }


            return lstPlan;
        }

       

        public static List<Justbok.Controllers.AddBranch> Branches { get; set; }
        public static List<Justbok.Controllers.AddBranchStaff> StaffBranches { get; set; }
        public static List<Justbok.Controllers.ShiftStaff> Shifts { get; set; }
        public static List<Justbok.Controllers.AddWorkoutPlan> WorkoutPlan { get; set; }

        public static List<Justbok.Controllers.AddDietPlan> DietPlan { get; set; }
      

       

      
    }
}