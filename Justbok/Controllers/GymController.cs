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


namespace Justbok.Controllers
{
    public class GymController : Controller
    {
        //
        // GET: /Gym/

        JustbokEntities db = new JustbokEntities();

        public ActionResult AddGym()
        {
            ViewBag.PackageList = GetPackage();
            return View();
        }

        [HttpPost]
        public JsonResult AddGym(GymViewModel objGymInfo)
        {
             GymList objGymlist = new GymList();
            GymLogin objGymLog = new GymLogin();
            GymServiceList objservices = new GymServiceList();
            List<string> lstServices = new List<string>();
            objGymlist = GetGymList(objGymInfo);
            db.GymLists.Add(objGymlist);
            db.SaveChanges();
          System.Web.HttpContext.Current.Session["GYMID"] = objGymlist.Gymid;
            objGymLog=GetGymLogin(objGymInfo);
            db.GymLogins.Add(objGymLog);
            db.SaveChanges();
           // AddAminities();

            lstServices = GetServiceList(objGymInfo);
            if (lstServices.Count > 0)
            {
                foreach (string service in lstServices)
                {
                    GymServiceList objServices = new GymServiceList();
                    objServices.ServiceName = service;
                    objServices.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
                    db.GymServiceLists.Add(objServices);
                    db.SaveChanges();
                }

            }

            return Json(objGymInfo);
        }

        [HttpPost]
        public JsonResult EditGym(GymViewModel objGymInfo)
        {
            GymList objGymlist = new GymList();
            GymLogin objGymLog = new GymLogin();
            GymServiceList objservices = new GymServiceList();
            List<string> lstServices = new List<string>();
            //Update MemberInfo
            objGymlist = GetGymList(objGymInfo);
            objGymlist.Gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            db.Entry(objGymlist).State = EntityState.Modified;
            db.SaveChanges();
            objGymLog = EditGetGymLogin(objGymInfo);
            db.Entry(objGymLog).State = objGymLog.Loginid == 0 ? EntityState.Added : EntityState.Modified;
            db.SaveChanges();
            //Update Login
           //int gymid=Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            //var checkLoginAvailable = (from gymLogin in db.GymLogins

            //                           where gymLogin.GymId == gymid
            //                select gymLogin).ToList();
            //if (checkLoginAvailable.Count > 0)
            //{
            //    objGymLog = GetGymLogin(objGymInfo);
            //    objGymLog.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            //    db.Entry(objGymlist).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //else
            //{
            //    objGymLog = GetGymLogin(objGymInfo);
            //    objGymLog.GymId = gymid;
            //    db.GymLogins.Add(objGymLog);
            //    db.SaveChanges();
            //}
            //Update Service
            lstServices = GetServiceList(objGymInfo);
            if (lstServices.Count > 0)
            {
                foreach (string service in lstServices)
                {
                    GymServiceList objServices = new GymServiceList();
                    objServices.ServiceName = service;
                    objServices.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
                    db.GymServiceLists.Add(objServices);
                    db.SaveChanges();
                }

            }


            return Json(objGymInfo);
        }

        private GymList GetGymList(GymViewModel objGymInfo)
        {
            GymList objGymlists = new GymList();
            try
            {
                objGymlists.GymName = objGymInfo.GymName;
                objGymlists.FirstName = objGymInfo.OwnerFirstName;
                objGymlists.LastName = objGymInfo.OwnerLastName;
                objGymlists.Openat = objGymInfo.Openedat;
                objGymlists.Closeat = objGymInfo.Closeat; ;
                objGymlists.GymAddress = objGymInfo.GymAddress;
                objGymlists.Email = objGymInfo.Email;
                objGymlists.EnrolDate = objGymInfo.EnrolDate;
                objGymlists.MobileNumber = objGymInfo.MobileNumber;
                objGymlists.PhoneNumberGym = objGymInfo.PhoneNumberGym;
                objGymlists.Representative = objGymInfo.Representative;
                objGymlists.InstructionMessage = objGymInfo.InstructionMessage;
                objGymlists.EstablishedYear = objGymInfo.EstablishedYear;
                objGymlists.About = objGymInfo.About;
                objGymlists.FeedBack = objGymInfo.FeedBack;


            }
            catch (Exception ex)
            {

            }
            return objGymlists;
        }

        private GymLogin GetGymLogin(GymViewModel objGymInfo)
        {
            GymLogin objGymLogin = new GymLogin();
            try
            {
                objGymLogin.UserName = objGymInfo.UserName;
                objGymLogin.Password = objGymInfo.Password;
                objGymLogin.IsLoginActive = objGymInfo.IsLoginActive ? "Yes" : "No";
                objGymLogin.Role = "Gym";
                objGymLogin.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            }
            catch (Exception ex)
            {

            }
            return objGymLogin;
        }
        private GymLogin EditGetGymLogin(GymViewModel objGymInfo)
        {
            GymLogin objGymLogin = new GymLogin();
            try
            {
                objGymLogin.Loginid = objGymInfo.Loginid;
                objGymLogin.UserName = objGymInfo.UserName;
                objGymLogin.Password = objGymInfo.Password;
                objGymLogin.Role = "Gym";
                objGymLogin.IsLoginActive = objGymInfo.IsLoginActive?"Yes":"No";
                objGymLogin.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);


            }
            catch (Exception ex)
            {

            }
            return objGymLogin;
        }

        [HttpGet]
        public JsonResult BindGymList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            var gymDetails =  (from mi in db.GymLists
                          join ms in db.GymLogins on mi.Gymid equals ms.GymId
                          where mi.Gymid == gymid

                          select new
                          {
                              GymId = mi.Gymid,
                              EstablishedYear = mi.EstablishedYear,
                              InstructionMessage = mi.InstructionMessage,
                              About = mi.About,
                              FeedBack = mi.FeedBack,
                              UserName = ms.UserName,
                              Password = ms.Password,
                              IsLoginActive = ms.IsLoginActive,
                              Loginid = ms.Loginid
                          }).ToList();

            return Json(gymDetails, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AddPackages(GymViewModel objGymPackage)
        {
            GymPackage objAddedGym = new GymPackage();
            //List<string> lstServices = new List<string>();
           // lstServices = GetServiceList(objGymPackage);
            objAddedGym = GetGymPackageList(objGymPackage);
            db.GymPackages.Add(objAddedGym);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult EditAddPackages(GymViewModel objGymPackage)
        {
            GymPackage objAddedGym = new GymPackage();
            objAddedGym = EditGetGymPackageList(objGymPackage);
            db.Entry(objAddedGym).State = objAddedGym.PackageId == 0 ? EntityState.Added : EntityState.Modified;
            //db.GymPackages.Add(objAddedGym);
            db.SaveChanges();
            return Json(new { success = true });
        }

        private List<string> GetServiceList(GymViewModel objGymPackage)
        {
            GymServiceList objServicelists = new GymServiceList();
            List<string> lstServices = new List<string>();
            try
            {
                string [] arrservices = objGymPackage.Services.Split(',');
                foreach(string service in arrservices)
                {
                    if (!service.Contains("multiselect-all"))
                    {
                        lstServices.Add(service);
                    }
                }
                
                //objGymlists.GymId = Convert.ToInt32(Session["GYMID"]);



            }
            catch (Exception ex)
            {

            }
            return lstServices;
        }
        private GymPackage GetGymPackageList(GymViewModel objGymPackage)
        {
            GymPackage objGymlists = new GymPackage();
            try
            {
                objGymlists.PackageType = objGymPackage.PackageType;
                objGymlists.PackageAmount = objGymPackage.PackageAmount;
                objGymlists.Months = objGymPackage.Months;
                objGymlists.StartDate = objGymPackage.StartDate;
                objGymlists.EndDate = objGymPackage.EndDate;
                objGymlists.Note = objGymPackage.Note;
                objGymlists.GymStatus = objGymPackage.GymStatus;
                objGymlists.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
                


            }
            catch (Exception ex)
            {

            }
            return objGymlists;
        }


        private GymPackage EditGetGymPackageList(GymViewModel objGymPackage)
        {
            GymPackage objGymlists = new GymPackage();
            try
            {
                objGymlists.PackageId = objGymPackage.PackageId;
                objGymlists.PackageType = objGymPackage.PackageType;
                objGymlists.PackageAmount = objGymPackage.PackageAmount;
                objGymlists.Months = objGymPackage.Months;
                objGymlists.StartDate = objGymPackage.StartDate;
                objGymlists.EndDate = objGymPackage.EndDate;
                objGymlists.Note = objGymPackage.Note;
                objGymlists.GymStatus = objGymPackage.GymStatus;
                objGymlists.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);

            }
            catch (Exception ex)
            {

            }
            return objGymlists;
        }


        [HttpGet]
        public JsonResult DeletePackage(string PackageId)
        {
            var result = new { Success = "False", Message = "Unable To delete Information." };
            if (PackageId != null)
            {
                int packageId = int.Parse(PackageId);
                var package = db.GymPackages.Where(x => x.PackageId == packageId).SingleOrDefault();

                if (package != null)
                {
                    db.GymPackages.Remove(package);
                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public  List<string> GetPackage()
        {
            List<string> lstPackages = new List<string>();
            JustbokEntities objPackage = new JustbokEntities();
            try
            {
                lstPackages = (from pack in objPackage.GymAllPackages
                             select (pack.OfferName + " " + pack.Months + " Month (" + pack.Amount.ToString() + ")")).ToList();
            }
            catch (Exception ex)
            {

            }
            return lstPackages;
        }

        [HttpGet]
        public JsonResult BindGymPackage()
        {
           
            JustbokEntities objMembersdata = new JustbokEntities();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            List<NewGymPackages> listGymPackages = new List<NewGymPackages>();
            int totalAmount = 0;

            var gymPackages = (from ms in db.GymPackages
                               where ms.GymId == gymid
                               select new
                               {
                                   PackageId = ms.PackageId,
                                   PackageType = ms.PackageType,
                                   StartDate = ms.StartDate.ToString(),
                                   EndDate = ms.EndDate.ToString(),
                                   Months = ms.Months,
                                   Amount = ms.PackageAmount,
                                   Status = ms.GymStatus,
                                   PaidAmount = ""
                               }).ToList();

            foreach (var packages in gymPackages)
            {
                totalAmount = 0;
                int getpackage = Convert.ToInt32(packages.PackageId);
                var paidAmount = (from amount in db.GymPayments
                                  where amount.PackageId == getpackage
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }

                listGymPackages.Add(new NewGymPackages()
                {
                    PackageId = packages.PackageId.ToString(),
                    PackageType = packages.PackageType,
                    Months = packages.Months.ToString(),
                    StartDate = packages.StartDate.ToString(),
                    EndDate = packages.EndDate.ToString(),
                    PackageAmount = packages.Amount.ToString(),
                    GymStatus = packages.Status.ToString(),
                    PaymentAmount = totalAmount.ToString()
                });

            }

            //var member = (from mi in db.GymLists
            //              join ms in db.GymPackages on mi.Gymid equals ms.GymId
            //              join p in db.GymPayments on ms.PackageId equals p.PackageId
            //              into all
            //              from T in all.DefaultIfEmpty()
            //              where mi.Gymid == gymid

            //              select new
            //              {
            //                  GymId = mi.Gymid,
            //                  PackageId = ms.PackageId,
            //                  PackageType = ms.PackageType,
            //                  StartDate = ms.StartDate.ToString(),
            //                  EndDate = ms.EndDate.ToString(),
            //                  Months = ms.Months,
            //                  Amount = ms.PackageAmount,
            //                  Status = ms.GymStatus,
            //                  PaidAmount = T.PaymentAmount
            //              }).ToList();

            return Json(gymPackages, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditBindGymPackage()
        {

            JustbokEntities objMembersdata = new JustbokEntities();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            List<NewGymPackages> listGymPackages = new List<NewGymPackages>();
            int totalAmount = 0;

            var gymPackages = (from ms in db.GymPackages
                               where ms.GymId == gymid
                              select new
                              {
                                  PackageId = ms.PackageId,
                                  PackageType = ms.PackageType,
                                  StartDate = ms.StartDate.ToString(),
                                  EndDate = ms.EndDate.ToString(),
                                  Months = ms.Months,
                                  Amount = ms.PackageAmount,
                                  Status = ms.GymStatus,
                                  PaidAmount = ""
                              }).ToList();

            foreach (var packages in gymPackages)
            {
                totalAmount = 0;
                int getpackage = Convert.ToInt32(packages.PackageId);
                var paidAmount = (from amount in db.GymPayments
                                  where amount.PackageId == getpackage
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }

                listGymPackages.Add(new NewGymPackages()
                {
                    PackageId = packages.PackageId.ToString(),
                    PackageType = packages.PackageType,
                    Months = packages.Months.ToString(),
                    StartDate = packages.StartDate.ToString(),
                    EndDate = packages.EndDate.ToString(),
                    PackageAmount = packages.Amount.ToString(),
                    GymStatus = packages.Status.ToString(),
                    PaymentAmount = totalAmount.ToString()
                });

            }

            return Json(listGymPackages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GymPayment(GymViewModel objGym)
        {
            
            GymPayment objPayment = new GymPayment();
            if (objGym != null)
            {
                objPayment = GetPaymentDetails(objGym);
                db.GymPayments.Add(objPayment);
                db.SaveChanges();
                
            }
            return Json(new { success = true });
        }

        public JsonResult EditGymPayment(GymViewModel objGym)
        {

            GymPayment objPayment = new GymPayment();
            if (objGym != null)
            {
                objPayment = GetEditPaymentDetails(objGym);
                db.Entry(objPayment).State = objPayment.ReferenceNumber == 0 ? EntityState.Added : EntityState.Modified;
               // db.GymPayments.Add(objPayment);
                db.SaveChanges();
                
            }
            return Json(new { success = true });
        }

        private GymPayment GetEditPaymentDetails(GymViewModel objGym)
        {
            GymPayment objPayment = new GymPayment();
            try
            {
                objPayment.RecieptNumber = objGym.RecieptNumber;
                objPayment.PaymentType = objGym.PaymentType;
                objPayment.PaymentAmount = objGym.PaymentAmount;
                objPayment.PaymentDate = objGym.PaymentDate;
                objPayment.PaymentDueDate = objGym.PaymentDueDate;
                objPayment.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
                objPayment.PackageId = objGym.PackageId;
                objPayment.ReferenceNumber = objGym.ReferenceNumber;

            }
            catch (Exception ex)
            {
            }
            return objPayment;
        }


        private GymPayment GetPaymentDetails(GymViewModel objGym)
        {
            GymPayment objPayment = new GymPayment();
            try
            {
                objPayment.PaymentType = objGym.PaymentType;
                objPayment.PaymentAmount = objGym.PaymentAmount;
                objPayment.PaymentDate = objGym.PaymentDate;
                objPayment.PaymentDueDate = objGym.PaymentDueDate;
                objPayment.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
                objPayment.PackageId = objGym.PackageId;
                objPayment.ReferenceNumber = objGym.ReferenceNumber;

            }
            catch (Exception ex)
            {
            }
            return objPayment;
        }


        [HttpGet]
        public JsonResult GymBindPayment(string PackageId)
        {
            //List<MemberShip> lstMemberData = new List<MemberShip>();
            JustbokEntities objPaymentdata = new JustbokEntities();
            //int memberId = Convert.ToInt32(Session["MemberId"]);
            int packageid = Convert.ToInt32(PackageId);
            //var   lstMemberData = (from membership in objMembersdata.MemberShips
            //                    where membership.MemberID == memberId
            //                    select membership).ToList();

            var payments = db.GymPayments
                .Where(x => x.PackageId == packageid)
                .Select(a => new
                {

                    PaymentDate = a.PaymentDate.ToString(),
                    PaidAmount = a.PaymentAmount,
                    PaymentType = a.PaymentType,
                    PaymentDueDate = a.PaymentDueDate.ToString(),
                    RecieptNumber = a.RecieptNumber


                }).ToList();

            return Json(payments, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeletePayment(string RecieptNumber)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            if (RecieptNumber != null)
            {
                int receiptNumber = int.Parse(RecieptNumber);
                var payments = db.GymPayments.Where(x => x.RecieptNumber == receiptNumber).SingleOrDefault();

                if (payments != null)
                {
                    db.GymPayments.Remove(payments);
                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditGetGymInvoice(string packageid, string receiptnumber)
        {

           // JustbokEntities objPaymentdata = new JustbokEntities();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            int packageId = Convert.ToInt32(packageid);
            int receiptno = Convert.ToInt32(receiptnumber);
            var invoice = (from m in db.GymLists
                           join s in db.GymPackages on m.Gymid equals s.GymId
                           join p in db.GymPayments on s.PackageId equals p.PackageId

                           select new
                           {
                               FirstName = m.FirstName + " " + m.LastName,
                               GymAddress = m.GymAddress,
                               PackageType = s.PackageType,
                               StartDate = s.StartDate.ToString(),
                               EndDate = s.EndDate.ToString(),
                               Amount = s.PackageAmount,
                               PackageId = s.PackageId,
                               RecieptNumber = p.RecieptNumber
                           }).ToList();

            var result = invoice.Find(s => (s.PackageId == packageId) && (s.RecieptNumber == receiptno));


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult EditGymInvoice(string PackageId, string receiptnumber)
        {

            JustbokEntities objPaymentdata = new JustbokEntities();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            int packageid = Convert.ToInt32(PackageId);
            int receiptno = Convert.ToInt32(receiptnumber);

            var invoice = (from m in db.GymLists
                           join s in db.GymPackages on m.Gymid equals s.GymId
                           join p in db.GymPayments on s.PackageId equals p.PackageId

                           select new
                           {
                               FirstName = m.FirstName + " " + m.LastName,
                               Address = m.GymAddress,
                               Package = s.PackageType,
                               StartDate = s.StartDate.ToString(),
                               EndDate = s.EndDate.ToString(),
                               Amount = s.PackageAmount,
                               PackageId = s.PackageId,
                               RecieptNo = p.RecieptNumber
                           }).ToList();

            var result = invoice.Find(s => (s.PackageId == packageid) && (s.RecieptNo == receiptno));


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGymList(int? page)
        {
            int pagesize = pagesize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //int gymid = Convert.ToInt32(Session["LoggedGym"]);
            List<GymList> lstGymList = (from gymlist in db.GymLists
                                            select gymlist).ToList();
            return View(lstGymList.ToPagedList(pageIndex, pagesize));
        }

        public ActionResult EditGymList(string id)
        {
            ViewBag.PackageList = GetPackage();
            int Gymid = Convert.ToInt32(id);
            System.Web.HttpContext.Current.Session["EditGymID"] = Gymid;
           
            GymList gym = db.GymLists.Find(Gymid);
            GymViewModel GymList = EditGetGymList(gym);
            return View(GymList);
        }

        private GymViewModel EditGetGymList(GymList objGymInfo)
        {
            GymViewModel objGymlists = new GymViewModel();
            try
            {
                objGymlists.GymName = objGymInfo.GymName;
                objGymlists.OwnerFirstName = objGymInfo.FirstName;
                objGymlists.OwnerLastName = objGymInfo.LastName;
                objGymlists.Openedat = objGymInfo.Openat;
                objGymlists.Closeat = objGymInfo.Closeat;
                objGymlists.GymAddress = objGymInfo.GymAddress;
                objGymlists.Email = objGymInfo.Email;
                objGymlists.EnrolDate = objGymInfo.EnrolDate;
                objGymlists.MobileNumber = objGymInfo.MobileNumber;
                objGymlists.PhoneNumberGym = objGymInfo.PhoneNumberGym;
                objGymlists.Representative = objGymInfo.Representative;


            }
            catch (Exception ex)
            {

            }
            return objGymlists;
        }

        public ActionResult GetAllPackages(int? page)
        {
            int pagesize = pagesize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //int gymid = Convert.ToInt32(Session["LoggedGym"]);
            List<GymAllPackage> lstGymPackage = (from gympackage in db.GymAllPackages
                                                 select gympackage).ToList();
            return View(lstGymPackage.ToPagedList(pageIndex, pagesize));
        }

        [HttpGet]
        public ActionResult AddGymOffers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGymOffers(GymAllPackage gympackage)
        {
            db.GymAllPackages.Add(gympackage);
            db.SaveChanges();
            return RedirectToAction("GetAllPackages");
        }

        public ActionResult EditGymOffers(string id)
        {
            
            int offerid = Convert.ToInt32(id);
            GymAllPackage offers = db.GymAllPackages.Find(offerid);
            return View(offers);
           
        }

        [HttpPost]
        public ActionResult EditGymOffers(GymAllPackage package)
        {

            db.Entry(package).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("GetAllPackages");

        }

        [HttpPost]
        public JsonResult AddServices(List<string> services)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            GymServiceList objServiceList = new GymServiceList();
            List<string> Services = new List<string>() { "Gym", "Membership", "Enquiry", "Setting", "Expense", "TimeSheet",
                "Reports","Inventory","SMS","Workout","Diet","QuickPrint","Attendance","Calender","POS","DashBoard","MemberInfo" };

            foreach(var srvc in Services)
            {
                objServiceList.ServiceName = srvc;
                objServiceList.GymId = gymid;
                objServiceList.Enable = false;
                db.GymServiceLists.Add(objServiceList);
                db.SaveChanges();
            }

            System.Threading.Thread.Sleep(2000);
            List<GymServiceList> lstGymServices = (from am in db.GymServiceLists
                                              where am.GymId == gymid
                                              select am).ToList();

            foreach (var item in services)
            {
                var serviceFound = lstGymServices.Find(x => x.ServiceName.Replace(" ", "").Contains(item));
                if (serviceFound != null)
                {
                    serviceFound.Enable = true;
                    db.GymServiceLists.Attach(serviceFound);
                    db.Entry(serviceFound).Property(x => x.Enable).IsModified = true;
                    db.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //List<GymServiceList>

        [HttpPost]
        public JsonResult EditServices(List<string> services)
        {

            int editGymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
              List<string> Services = new List<string>() { "Gym", "Membership", "Enquiry", "Setting", "Expense", "TimeSheet",
                "Reports","Inventory","SMS","Workout","Diet","QuickPrint","Attendance","Calender","POS","DashBoard","MemberInfo" };
              if (services != null)
              {

                  List<GymServiceList> lstServiceCheck = (from am in db.GymServiceLists
                                                          where am.GymId == editGymid
                                                          select am).ToList();

                  //check if aminity exists not exits add new 

                  if (lstServiceCheck.Count != Services.Count)
                  {
                      foreach (var srvc in Services)
                      {
                          var addService = lstServiceCheck.Find(x => x.ServiceName.Contains(srvc));
                          if (addService == null)
                          {
                              GymServiceList objService = new GymServiceList();
                              objService.Enable = false;
                              objService.ServiceName = srvc;
                              objService.GymId = editGymid;
                              db.GymServiceLists.Add(objService);
                              db.SaveChanges();
                          }

                      }


                  }

                  List<GymServiceList> lstServiceList = (from am in db.GymServiceLists
                                                 where am.GymId == editGymid
                                                 select am).ToList();


                  foreach (var item in lstServiceList)
                  {

                      var serviceFound = services.Any(str => str.Contains(item.ServiceName.ToString().Replace(" ", "")));
                      if (serviceFound)
                      {
                          if (item.Enable == false || item.Enable==null)
                          {
                              item.Enable = true;
                              db.GymServiceLists.Attach(item);
                              db.Entry(item).Property(x => x.Enable).IsModified = true;
                              db.SaveChanges();

                          }
                      }
                      else
                      {
                          if (item.Enable == true)
                          {
                              item.Enable = false;
                              db.GymServiceLists.Attach(item);
                              db.Entry(item).Property(x => x.Enable).IsModified = true;
                              db.SaveChanges();
                          }


                      }

                  }

              }
                
               

    //        if (editGymid != 0)
    //        {
    //            var deleteServices =
    //from service in db.GymServiceLists
    //where service.GymId == editGymid
    //select service;

    //            if (deleteServices != null)
    //            {

    //                foreach (var service in deleteServices)
    //                {
    //                    db.GymServiceLists.Remove(service);

    //                }
    //                try
    //                {
    //                    db.SaveChanges();
    //                    if (services.Count > 0)
    //                    {
    //                        foreach (var item in services)
    //                        {
    //                            GymServiceList objService = new GymServiceList();
    //                            objService.ServiceName = item;
    //                            objService.GymId = editGymid;
    //                            db.GymServiceLists.Add(objService);
    //                            db.SaveChanges();
    //                        }
    //                    }

    //                }
    //                catch (Exception e)
    //                {

    //                }
    //            }
    //            else
    //            {
    //                //if no services are there this work 
    //                if (services.Count>0)
    //                { 
    //                foreach (var item in services)
    //                {
    //                    GymServiceList objService = new GymServiceList();
    //                    objService.ServiceName = item;
    //                    objService.GymId = editGymid;
    //                    db.GymServiceLists.Add(objService);
    //                    db.SaveChanges();
    //                }
    //                }
    //            }


                
    //        }

            return Json("Success", JsonRequestBehavior.AllowGet);

        }

        public JsonResult EditServiceList()
        {
            int editGymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
                var Services = (from services in db.GymServiceLists
                                where services.GymId == editGymid && services.Enable==true

                                select new
                                {
                                    ServiceName = services.ServiceName
                                }).ToList();

                return Json(Services, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAmenitiesList()
        {
           // int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            var AmenitiesList = (from amenities in db.Amenities
                                 select new
                                 {
                                     AmenitiesId = amenities.AmenitiesId,
                                     AmenitiesName = amenities.AmenitiesName
                                 }).ToList();

            return Json(AmenitiesList, JsonRequestBehavior.AllowGet);
        }

        //[HttpGet]
        //public JsonResult GetAmenitiesList()
        //{
        //    int gymid=Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
        //    var AmenitiesList = (from amenities in db.GymAmenities
        //                         where amenities.GymId == gymid
        //                    select new
        //                    {
        //                        GymAmenitiesId = amenities.GymAmenitiesId,
        //                        GymAmenitiesName = amenities.GymAmenitiesName,
        //                        AminityEnable = amenities.AminityEnable
        //                    }).ToList();

        //    return Json(AmenitiesList, JsonRequestBehavior.AllowGet);
        //}

        //private void AddAminities()
        //{
            
        //    GymAmenity objAminity = new GymAmenity();

        //  List<Amenity> AmenitiesList = (from aminities in db.Amenities
        //                                select aminities).ToList();

        //  if (AmenitiesList.Count > 0)
        //  {
        //      foreach (var aminity in AmenitiesList)
        //      {
        //          objAminity.GymAmenitiesName = aminity.AmenitiesName;
        //          objAminity.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
        //          objAminity.AminityEnable = false;
        //          db.GymAmenities.Add(objAminity);
        //          db.SaveChanges();
                  
              
        //      }
        //  }


         
        //}

        [HttpPost]
        public JsonResult UpdateAmenityandEnquiry(List<string> Aminities, List<string> Equipment)
        {
          
            AddAminity(Aminities);
            AddEquipment(Equipment);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private void AddAminity(List<string> Aminities)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            if (Aminities != null)
            {
                List<Amenity> lstAmenity = (from amenities in db.Amenities
                                            select amenities).ToList();

                if (lstAmenity.Count > 0)
                {
                    foreach (var aminity in lstAmenity)
              {
                  GymAmenity objAminity = new GymAmenity();
                  objAminity.GymAmenitiesName = aminity.AmenitiesName;
                  objAminity.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
                  objAminity.AminityEnable = false;
                  db.GymAmenities.Add(objAminity);
                  db.SaveChanges();
              }
                    System.Threading.Thread.Sleep(2000);
                    List<GymAmenity> lstGymAmenity = (from am in db.GymAmenities
                                                   where am.GymId == gymid
                                                   select am).ToList();

                    foreach (var item in Aminities)
                    {
                        var amenityFound = lstGymAmenity.Find(x => x.GymAmenitiesName.Replace(" ", "").Contains(item));
                        if (amenityFound != null)
                        {
                                amenityFound.AminityEnable = true;
                                db.GymAmenities.Attach(amenityFound);
                                db.Entry(amenityFound).Property(x => x.AminityEnable).IsModified = true;
                                db.SaveChanges();
                        }
                    }
                }
            }
        }

        private void AddEquipment(List<string> Equipment)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            if (Equipment != null)
            {
                List<Equipment> lstEquipment = (from equipment in db.Equipments
                                                select equipment).ToList();

                if (lstEquipment.Count > 0)
                {
                    foreach (var equipment in lstEquipment)
                    {
                        GymEquipment objEquipment = new GymEquipment();
                        objEquipment.GymEquipmentName = equipment.EquipmentName;
                        objEquipment.GymId = gymid;
                        objEquipment.EquipmentEnable = false;
                        db.GymEquipments.Add(objEquipment);
                        db.SaveChanges();
                    }
                    System.Threading.Thread.Sleep(2000);
                    List<GymEquipment> lstGymEquipment = (from am in db.GymEquipments
                                                      where am.GymId == gymid
                                                      select am).ToList();

                    foreach (var item in Equipment)
                    {
                        var equipmentFound = lstGymEquipment.Find(x => x.GymEquipmentName.Replace(" ", "").Contains(item));
                        if (equipmentFound != null)
                        {
                            equipmentFound.EquipmentEnable = true;
                            db.GymEquipments.Attach(equipmentFound);
                            db.Entry(equipmentFound).Property(x => x.EquipmentEnable).IsModified = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        [HttpGet]
        public JsonResult EditAmenitiesList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            var AmenitiesList = (from amenities in db.GymAmenities
                                 where amenities.GymId == gymid
                                 select new
                                 {
                                     GymAmenitiesId = amenities.GymAmenitiesId,
                                     GymAmenitiesName = amenities.GymAmenitiesName,
                                     AminityEnable = amenities.AminityEnable
                                 }).ToList();

            return Json(AmenitiesList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditEquipmentList()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
            var EquipmentList = (from equipment in db.GymEquipments
                                 where equipment.GymId == gymid
                                 select new
                                 {
                                     GymEquipmentId = equipment.GymEquipmentId,
                                     GymEquipmentName = equipment.GymEquipmentName,
                                     EquipmentEnable = equipment.EquipmentEnable
                                 }).ToList();

            return Json(EquipmentList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EditUpdateAmenityandEnquiry(List<string> Aminities, List<string> Equipment)
        {
            UpdateAminity(Aminities);
            UpdateEquipment(Equipment);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private void UpdateAminity(List<string> Aminities)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
           // bool isNew = true;
         if (Aminities != null)
         {

             List<GymAmenity> lstAmenityCheck = (from am in db.GymAmenities
                                            where am.GymId == gymid
                                            select am).ToList();

             //check if aminity exists not exits add new 

             List<Amenity> lstMainAmenity = (from am in db.Amenities
                                            select am).ToList();
             if (lstMainAmenity.Count != lstAmenityCheck.Count)
             {
                 foreach(var newAminity in lstMainAmenity)
                 {
                     var addAminity = lstAmenityCheck.Find(x => x.GymAmenitiesName.Contains(newAminity.AmenitiesName));
                     if (addAminity == null)
                     {
                         GymAmenity objAminity = new GymAmenity();
                         objAminity.AminityEnable = false;
                         objAminity.GymAmenitiesName = newAminity.AmenitiesName;
                         objAminity.GymId = gymid;
                         db.GymAmenities.Add(objAminity);
                         db.SaveChanges();
                     }

                 }
                
             
             }
             List<GymAmenity> lstAmenity= (from am in db.GymAmenities
                                                 where am.GymId == gymid
                                                 select am).ToList();


             foreach (var item in lstAmenity)
             { 

             var amenityFound = Aminities.Any(str => str.Contains(item.GymAmenitiesName.ToString().Replace(" ", "")));
             // var amenityFound = lstAmenity.Find(x => x.GymAmenitiesName.Replace(" ", "").Contains(item));
             if (amenityFound)
             {
               //  isNew = false;
                 if (item.AminityEnable == false)
                 {
                     item.AminityEnable = true;
                     db.GymAmenities.Attach(item);
                     db.Entry(item).Property(x => x.AminityEnable).IsModified = true;
                     db.SaveChanges();
                    
                 }
             }
             else
             {
                // isNew = false;
                 if (item.AminityEnable==true)
                 {
                     item.AminityEnable = false;
                     db.GymAmenities.Attach(item);
                     db.Entry(item).Property(x => x.AminityEnable).IsModified = true;
                     db.SaveChanges();
                 }
               

             }
           
             }
            
         }
        }

        private void UpdateEquipment(List<string> Equipment)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]);
           
            if (Equipment != null)
            {

                List<GymEquipment> lstEquipmentCheck = (from am in db.GymEquipments
                                                   where am.GymId == gymid
                                                   select am).ToList();


                List<Equipment> lstMainEquipment = (from am in db.Equipments
                                                select am).ToList();

                if (lstMainEquipment.Count != lstEquipmentCheck.Count)
                {
                    foreach (var newAminity in lstMainEquipment)
                    {
                        var addAminity = lstEquipmentCheck.Find(x => x.GymEquipmentName.Contains(newAminity.EquipmentName));
                        if (addAminity == null)
                        {
                            GymEquipment objequipment = new GymEquipment();
                            objequipment.EquipmentEnable = false;
                            objequipment.GymEquipmentName = newAminity.EquipmentName;
                            objequipment.GymId = gymid;
                            db.GymEquipments.Add(objequipment);
                            db.SaveChanges();
                        }

                    }
                }

                List<GymEquipment> lstEquipment = (from am in db.GymEquipments
                                               where am.GymId == gymid
                                               select am).ToList();

                foreach (var item in lstEquipment)
                {
                    var equipmentFound = Equipment.Any(str => str.Contains(item.GymEquipmentName.ToString().Replace(" ", "")));
                  //  var equipmentFound = lstEquipment.Find(x => x.GymEquipmentName.Replace(" ","").Contains(item));
                    if (equipmentFound)
                    {
                        if (item.EquipmentEnable == false)
                        {
                            item.EquipmentEnable = true;
                            db.GymEquipments.Attach(item);
                            db.Entry(item).Property(x => x.EquipmentEnable).IsModified = true;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (item.EquipmentEnable == true)
                        {
                            item.EquipmentEnable = false;
                            db.GymEquipments.Attach(item);
                            db.Entry(item).Property(x => x.EquipmentEnable).IsModified = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public JsonResult GetEquipmentList()
        {
            // int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            var lstEquipment = (from equipment in db.Equipments
                                 select new
                                 {
                                     EquipmentId = equipment.EquipmentId,
                                     EquipmentName = equipment.EquipmentName
                                 }).ToList();

            return Json(lstEquipment, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddGymBranch(GymViewModel branch)
        {
            Branch objBranch = new Branch();
            objBranch.Branchcode = branch.Branchcode;
            objBranch.BranchName = branch.BranchName;
            objBranch.BranchAdress = branch.BranchAdress;
            objBranch.PhoneNo = branch.PhoneNo;
            objBranch.City = branch.City;
            objBranch.BranchState = branch.BranchState;
            objBranch.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]);
            db.Branches.Add(objBranch);
            db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUploadImage()
        {
            var result = "error";
            string _imgname = string.Empty;
            GymImage objGymImage = new GymImage();
            var user = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditGymID"]).ToString();
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["imageUploadForm"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);
                    _imgname = user + _ext;
                    var _comPath = Server.MapPath("/Users/") + _imgname;
                    pic.SaveAs(_comPath);
                    var dbpath = "/Users/" + _imgname;
                    objGymImage.ImageData = dbpath;
                    objGymImage.GymId = Convert.ToInt32(user);

                    var imgfind = (from mi in db.MemberImages
                                   where mi.MemberId == objGymImage.GymId
                                   select new { mi.ImageId }).ToList();
                    if (imgfind.Count > 0)
                    {
                        objGymImage.GymId = imgfind[0].ImageId;
                        db.Entry(objGymImage).State = EntityState.Modified;
                        db.SaveChanges();
                        result = "success";
                    }
                    else
                    {
                        db.GymImages.Add(objGymImage);
                        db.SaveChanges();
                        result = "success";
                    }
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        public JsonResult UploadImage()
        {
            var result = "error";
            string _imgname = string.Empty;
            GymImage objGymImage = new GymImage();
            var user = Convert.ToInt32(System.Web.HttpContext.Current.Session["GYMID"]).ToString();
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
                    objGymImage.ImageData = dbpath;
                    objGymImage.GymId = int.Parse(user);
                    db.GymImages.Add(objGymImage);
                    db.SaveChanges();
                    result = "success";
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

    }
}
