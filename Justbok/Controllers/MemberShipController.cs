using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;
using Justbok.ADModel;
using System.Data;
using log4net;
using System.Web.UI;
using System.Threading.Tasks;
using context = System.Web.HttpContext; 

namespace Justbok.Controllers
{

    public class AddWorkoutPlan
    {
        public string PlaneNameId { get; set; }
        public string PlanName { get; set; }
    }

     public class AddDietPlan
    {
        public string DietPlanId { get; set; }
        public string DietPlanName { get; set; }
    }
 
    public class MemberShipController : LayoutBaseModel
    {
        //
        // GET: /MemberShip/

       

        //public class AddBranchStaff
        //{
        //    public string BranchName { get; set; }
        //    public string BranchId { get; set; }

        //}
        private static readonly ILog Log =
              LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);  //Declaring Log4Net 

        JustbokEntities db = new JustbokEntities();
        MemberViewModel reprentative = new MemberViewModel();




        public ActionResult MemberShip()
        {

            ViewBag.Stafflist = GetStaffList();
            ViewBag.OfferList = BindMembershipList();
            //  ViewBag.Plans = GetPlanList();

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }


            return View();
        }

        [HttpPost]
        public ActionResult MemberShip(MemberViewModel objMember)
        {
            ViewBag.Stafflist = GetStaffList();
            ViewBag.OfferList = BindMembershipList();


            // reprentative.Representatives = EnquiryController.PopulateRepresentative();
            //memberinfo details
            if (!string.IsNullOrEmpty(objMember.FirstName))
            {
                MemberInfo objMemberDetails = new MemberInfo();
                objMemberDetails = GetMemberInfoDetails(objMember);
                db.MemberInfoes.Add(objMemberDetails);
                db.SaveChanges();
                System.Web.HttpContext.Current.Session["MemberId"] = objMemberDetails.MemberID;
                TempData["Result"] = "Data saved successfully";
            }
            //membership details
            if (!string.IsNullOrEmpty(objMember.MembershipType))
            {
                MemberShip objMembership = new MemberShip();
                objMembership = GetMemberShipDetails(objMember);
                db.MemberShips.Add(objMembership);
                db.SaveChanges();
                ViewBag.MembersList = GetMembersData();

            }

            //if (!string.IsNullOrEmpty(objMember.PaymentType))
            //{
            //    Payment objPayment = new Payment();
            //    objPayment = GetPaymentDetails(objMember);
            //    db.Payments.Add(objPayment);
            //    db.SaveChanges();
            //    ViewBag.PaymentList = GetPaymentData();
            //}


            return View();
        }

        [HttpPost]
        public JsonResult AddMember(MemberViewModel objMember)
        {
            MemberInfo objMemberDetails = new MemberInfo();
            objMemberDetails = GetMemberInfoDetails(objMember);
            db.Entry(objMemberDetails).State = objMember.MemberID == 0 ? EntityState.Added : EntityState.Modified;
            //db.MemberInfoes.Add(objMemberDetails);
            db.SaveChanges();
            System.Web.HttpContext.Current.Session["MemberId"] = objMemberDetails.MemberID;
            //System.Web.HttpContext.Current.Session["MobileNumber"] = objMemberDetails.MobileNumber;
            //return Json(objMember);
            return  Json(new { success = true });
        }

        [HttpPost]
        public JsonResult AddMemberShip(MemberViewModel objMember)
        {
            MemberShip objMembership = new MemberShip();
            if (objMember != null)
            {
                objMembership = GetMemberShipDetails(objMember);
                //db.MemberShips.Add(objMembership);
                db.Entry(objMembership).State = objMember.MembershipId == 0 ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            // Session["MembershipId"] = objMembership.MembershipId;
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult Payment(MemberViewModel objMember)
        {

            Payment objPayment = new Payment();
            if (objMember != null)
            {
                objPayment = GetPaymentDetails(objMember);
               // db.Entry(objPayment).State = objPayment.RecieptNumber == 0 ? EntityState.Added : EntityState.Modified;
                 db.Payments.Add(objPayment);
                db.SaveChanges();
            }
            // ViewBag.PaymentList = GetPaymentData();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult EditPayment(MemberViewModel objMember)
        {

            Payment objPayment = new Payment();
            if (objMember != null)
            {
                objPayment = GetEditPaymentDetails(objMember);
                db.Entry(objPayment).State = objPayment.RecieptNumber == 0 ? EntityState.Added : EntityState.Modified;
              //  db.Payments.Add(objPayment);
                db.SaveChanges();
            }
            // ViewBag.PaymentList = GetPaymentData();
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult UploadImage()
        {
            var result = "error";
            string _imgname = string.Empty;
            MemberImage objMemberImage = new MemberImage();
            var user = System.Web.HttpContext.Current.Session["MemberId"].ToString();
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
                    objMemberImage.ImageData = dbpath;
                    objMemberImage.MemberId = int.Parse(user);
                    db.MemberImages.Add(objMemberImage);
                    db.SaveChanges();
                    result = "success";
                }
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateUploadImage()
        {
            Log.Info("---Started Image Uploading----");
          
            var result = "error";
            try
            {
                string _imgname = string.Empty;
                MemberImage objMemberImage = new MemberImage();
                var user = System.Web.HttpContext.Current.Session["EditMemberId"].ToString();
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
                {
                    Log.Info("---Got Image from Jquery----");
                    var pic = System.Web.HttpContext.Current.Request.Files["imageUploadForm"];
                    Log.Info("Pic value: "+pic.ToString());
                    if (pic.ContentLength > 0)
                    {
                        Log.Info("---Got Pic----");
                        var fileName = Path.GetFileName(pic.FileName);
                        Log.Info(fileName);
                        var _ext = Path.GetExtension(pic.FileName);
                        Log.Info(_imgname);
                        _imgname = user + _ext;
                        //var _comPath = Server.MapPath("/Users/") + _imgname;
                        string theDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        Log.Info(theDirectory);
                        var _comPath = theDirectory + "Users\\" + _imgname;
                        Log.Info(_comPath);
                        pic.SaveAs(_comPath);
                        var dbpath = "/Users/" + _imgname;
                        objMemberImage.ImageData = dbpath;
                        objMemberImage.MemberId = Convert.ToInt32(user);

                        var imgfind = (from mi in db.MemberImages
                                       where mi.MemberId == objMemberImage.MemberId
                                       select new { mi.ImageId }).ToList();
                        if (imgfind.Count > 0)
                        {
                            objMemberImage.ImageId = imgfind[0].ImageId;
                            db.Entry(objMemberImage).State = EntityState.Modified;
                            db.SaveChanges();
                            result = "success";
                        }
                        else
                        {
                            db.MemberImages.Add(objMemberImage);
                            db.SaveChanges();
                            result = "success";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
                Log.Error(ex);
            }
          
         


            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Capture()
        {
            Log.Info("---Started Image Uploading----");
            var result = "error";
            MemberImage objMemberImage = new MemberImage();
            var user = System.Web.HttpContext.Current.Session["EditMemberId"].ToString();

            var stream = Request.InputStream;
            string dump;
            Log.Info("---Streaming image----");
            using (var reader = new StreamReader(stream))
                dump = reader.ReadToEnd();
            string path = "/Users/" + user + ".jpg";
            var _comPath = Server.MapPath(path);
            System.IO.File.WriteAllBytes(_comPath, String_To_Bytes2(dump));
            Log.Info("---Converts to byte----");
            objMemberImage.ImageData = path;
            objMemberImage.MemberId = Convert.ToInt32(user);

            var imgfind = (from mi in db.MemberImages
                           where mi.MemberId == objMemberImage.MemberId
                           select new { mi.ImageId }).ToList();
            if (imgfind.Count > 0)
            {
                objMemberImage.ImageId = imgfind[0].ImageId;
                db.Entry(objMemberImage).State = EntityState.Modified;
                db.SaveChanges();
                result = "success";
        }
            else
            {
                db.MemberImages.Add(objMemberImage);
                db.SaveChanges();
                result = "success";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NewCapture()
        {
            var result = "error";
            MemberImage objMemberImage = new MemberImage();
            var user = System.Web.HttpContext.Current.Session["MemberId"].ToString();

            var stream = Request.InputStream;
            string dump;

            using (var reader = new StreamReader(stream))
                dump = reader.ReadToEnd();
            string path = "/Users/" + user + ".jpg";
            var _comPath = Server.MapPath(path);
            System.IO.File.WriteAllBytes(_comPath, String_To_Bytes2(dump));
            objMemberImage.ImageData = path;
            objMemberImage.MemberId = Convert.ToInt32(user);
            db.MemberImages.Add(objMemberImage);
            db.SaveChanges();
            result = "success";
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
        private MemberInfo GetMemberInfoDetails(MemberViewModel objMember)
        {
            MemberInfo objMemberDetails = new MemberInfo();
            try
            {
                objMemberDetails.MemberID = objMember.MemberID;
                objMemberDetails.FirstName = objMember.FirstName;
                objMemberDetails.LastName = objMember.LastName;
                objMemberDetails.Dob = objMember.Dob;
                if (objMember.MemberReference != "")
                {
                    objMemberDetails.MemberReference =Convert.ToInt32(objMember.MemberReference);
                }
                objMemberDetails.Gender = objMember.Gender;
                objMemberDetails.Email = objMember.Email;
                objMemberDetails.MobileNumber = objMember.MobileNumber;
                objMemberDetails.MemberAddress = objMember.MemberAddress;
                objMemberDetails.EnrollDate = objMember.EnrollDate;
                objMemberDetails.Married = objMember.Married ? "Yes" : "No";
                objMemberDetails.SpouseName = objMember.SpouseName;
                objMemberDetails.SpouseBirthDate = objMember.SpouseBirthDate;
                objMemberDetails.AnniversaryDate = objMember.AnniversaryDate;
                objMemberDetails.Occupation = objMember.Occupation;
                objMemberDetails.Designation = objMember.Designation;
                if (objMember.MemberSource.ToUpper().Equals("OTHER"))
                {
                    objMemberDetails.MemberSource = objMember.Other;
                }
                else
                {
                    objMemberDetails.MemberSource = objMember.MemberSource;
                }

                objMemberDetails.PhoneResidence = objMember.PhoneResidence;
                objMemberDetails.PhoneOffice = objMember.PhoneOffice;
                objMemberDetails.ReferredBy = objMember.ReferredBy;
                objMemberDetails.Programme = objMember.Programme;
                objMemberDetails.Representative = objMember.Representative;
                objMemberDetails.BranchId = objMember.BranchId;
                objMemberDetails.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            }
            catch (Exception ex)
            {

            }
            return objMemberDetails;
        }

        private MemberShip GetMemberShipDetails(MemberViewModel objMember)
        {
            MemberShip objMembership = new MemberShip();
            try
            {
                objMembership.MembershipId = objMember.MembershipId;
                objMembership.MembershipType = objMember.MembershipType;
                objMembership.Months = objMember.Months;
                objMembership.Amount = objMember.Amount;
                objMembership.StartDate = objMember.StartDate;
                objMembership.EndDate = objMember.EndDate;
                objMembership.Status = objMember.Status;
                objMembership.Note = objMember.Note;
                if (objMember.MemberID != 0)
                {
                    objMembership.MemberID = objMember.MemberID;
                }
                else
                {
                    objMembership.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);

                }

               
                //if (Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]) != 0)
                //{
                //    objMembership.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
                //}
                //else
                //{
                //    objMembership.MemberID = objMember.MemberID;
                //    System.Web.HttpContext.Current.Session["MemberId"] = objMember.MemberID;
                //}
                objMembership.Status = objMember.Status;
            }
            catch (Exception ex)
            {
            }
            return objMembership;
        }

        private Payment GetPaymentDetails(MemberViewModel objMember)
        {
            Payment objPayment = new Payment();
            try
            {
                objPayment.PaymentType = objMember.PaymentType;
                objPayment.PaymentAmount = objMember.PaymentAmount;
                objPayment.PaymentDate = objMember.PaymentDate;
                objPayment.PaymentDueDate = objMember.PaymentDueDate;
                objPayment.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
                objPayment.MembershipId = objMember.MembershipId;
                objPayment.ReferenceNumber = objMember.ReferenceNumber;
                objPayment.RecieptNumber = objMember.RecieptNumber;

            }
            catch (Exception ex)
            {
            }
            return objPayment;
        }


        private Payment GetEditPaymentDetails(MemberViewModel objMember)
        {
            Payment objPayment = new Payment();
            try
            {
                objPayment.PaymentType = objMember.PaymentType;
                objPayment.PaymentAmount = objMember.PaymentAmount;
                objPayment.PaymentDate = objMember.PaymentDate;
                objPayment.PaymentDueDate = objMember.PaymentDueDate;
                objPayment.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
                objPayment.MembershipId = objMember.MembershipId;
                objPayment.ReferenceNumber = objMember.ReferenceNumber;
                objPayment.RecieptNumber = objMember.RecieptNumber;

            }
            catch (Exception ex)
            {
            }
            return objPayment;
        }


        public List<string> GetStaffList()
        {
            List<string> lstStaffList = new List<string>();
            JustbokEntities objStaff = new JustbokEntities();
            try
            {
                lstStaffList = (from staff in objStaff.Staffs
                                select (staff.FirstName + " " + staff.LastName)).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstStaffList;

        }

        private List<string> BindMembershipList()
        {
            List<string> lstoffers = new List<string>();
            JustbokEntities objStaff = new JustbokEntities();
            try
            {
                lstoffers = (from offer in objStaff.MembershipOffers
                             select (offer.OfferName + " " + offer.Months + " Month (" + offer.Amount.ToString() + ")")).ToList();
            }
            catch (Exception ex)
            {

            }
            return lstoffers;
        }



        [HttpGet]
        public JsonResult GetPlanList(int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var plannames = (from plans in db.WorkoutPlanNames
                            where plans.GymId == gymid && plans.BranchId == BranchId
                            select new
                            {
                                PlaneNameId = plans.PlaneNameId,
                                PlanName = plans.PlanName,
                            }).ToList();

            return Json(plannames, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDietPlanList(int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var DietPlans = (from plans in db.DietPlanNames
                             where plans.GymId == gymid && plans.BranchId == BranchId
                             select new
                             {
                                 PlaneNameId = plans.DietPlanId,
                                 PlanName = plans.DietPlanName1,
                             }).ToList();

            return Json(DietPlans, JsonRequestBehavior.AllowGet);
        }

        private List<MemberShip> GetMembersData()
        {
            List<MemberShip> lstMemberData = new List<MemberShip>();
            JustbokEntities objMembersdata = new JustbokEntities();
            int memberId = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
            try
            {
                lstMemberData = (from membership in objMembersdata.MemberShips
                                 where membership.MemberID == memberId
                                 select membership).ToList<MemberShip>();
            }
            catch (Exception ex)
            {

            }
            return lstMemberData;
        }

        [HttpGet]
        public JsonResult BindMembership()
        {

            JustbokEntities objMembersdata = new JustbokEntities();
            NewMemberShip objMemberShip = new NewMemberShip();
            List<NewMemberShip> listMemberships = new List<NewMemberShip>();
            int totalAmount = 0;
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
            var memberShip = (from ms in db.MemberShips
                              where ms.MemberID == memberid
                              select new
                              {
                                  MembershipID = ms.MembershipId,
                                  MemershipType = ms.MembershipType,
                                  StartDate = ms.StartDate.ToString(),
                                  EndDate = ms.EndDate.ToString(),
                                  Months = ms.Months,
                                  Amount = ms.Amount,
                                  Status = ms.Status,
                                  PaidAmount = ""
                              }).ToList();

            foreach (var memberships in memberShip)
            {
                totalAmount = 0;
                int getmembershipid = Convert.ToInt32(memberships.MembershipID);
                var paidAmount = (from amount in db.Payments
                                  where amount.MembershipId == getmembershipid
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }


                listMemberships.Add(new NewMemberShip()
                {
                    MembershipID = memberships.MembershipID.ToString(),
                    MemershipType = memberships.MemershipType,
                    Months = memberships.Months.ToString(),
                    StartDate = memberships.StartDate.ToString(),
                    EndDate = memberships.EndDate.ToString(),
                    Amount = memberships.Amount.ToString(),
                    Status = memberships.Status.ToString(),
                    PaidAmount = totalAmount.ToString()
                });

            }

            return Json(listMemberships, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult BindMembershipEdit()
        {

            JustbokEntities objMembersdata = new JustbokEntities();
            NewMemberShip objMemberShip = new NewMemberShip();
            List<NewMemberShip> listMemberships = new List<NewMemberShip>();
            int totalAmount = 0;
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            var memberShip = (from ms in db.MemberShips
                              where ms.MemberID == memberid
                              select new
                              {
                                  MembershipID = ms.MembershipId,
                                  MemershipType = ms.MembershipType,
                                  StartDate = ms.StartDate.ToString(),
                                  EndDate = ms.EndDate.ToString(),
                                  Months = ms.Months,
                                  Amount = ms.Amount,
                                  Status = ms.Status,
                                  Note=ms.Note,
                                  PaidAmount = ""
                              }).ToList();

            foreach (var memberships in memberShip)
            {
                totalAmount = 0;
                int getmembershipid = Convert.ToInt32(memberships.MembershipID);
                var paidAmount = (from amount in db.Payments
                                  where amount.MembershipId == getmembershipid
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }


                listMemberships.Add(new NewMemberShip()
                {
                    MembershipID = memberships.MembershipID.ToString(),
                    MemershipType = memberships.MemershipType,
                    Months = memberships.Months.ToString(),
                    StartDate = memberships.StartDate.ToString(),
                    EndDate = memberships.EndDate.ToString(),
                    Amount = memberships.Amount.ToString(),
                    Status = memberships.Status.ToString(),
                    Notes=memberships.Note,
                    PaidAmount = totalAmount.ToString()
                });

            }

            return Json(listMemberships, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindPayment(string membershipid)
        {
            JustbokEntities objPaymentdata = new JustbokEntities();
            int membership = Convert.ToInt32(membershipid);
            var payments = db.Payments
                .Where(x => x.MembershipId == membership)
                .Select(a => new
                {
                    PaymentDate = a.PaymentDate.ToString(),
                    PaidAmount = a.PaymentAmount,
                    PaymentType = a.PaymentType,
                    RecieptNumber = a.RecieptNumber,
                    PaymentDueDate = a.PaymentDueDate.ToString(),
                    PaymentAmount = a.PaymentAmount,
                    ReferenceNumber = a.ReferenceNumber

                }).ToList();

            return Json(payments, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteMembership(string MembershipId)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            if (MembershipId != null)
            {
                int membershipid = int.Parse(MembershipId);
                var membership = db.MemberShips.Where(x => x.MembershipId == membershipid).SingleOrDefault();

                if (membership != null)
                {
                    db.MemberShips.Remove(membership);
                    db.SaveChanges();
                    result = new { Success = "True", Message = "Deleted record." };
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeletePayment(string RecieptNumber)
        {
            var result = new { Success = "False", Message = "Unable To Save Information." };
            if (RecieptNumber != null)
            {
                int receiptNumber = int.Parse(RecieptNumber);
                var payments = db.Payments.Where(x => x.RecieptNumber == receiptNumber).SingleOrDefault();

                if (payments != null)
                {
                    db.Payments.Remove(payments);
                    db.SaveChanges();
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UpdateMembership(string membershipid)
        {
            int totalAmount = 0;
            int membership = Convert.ToInt32(membershipid);
            var paidAmount = (from amount in db.Payments
                              where amount.MembershipId == membership
                              select new { amount.PaymentAmount }).ToList();
            foreach (var amount in paidAmount)
            {
                totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
            }

            var updaterow = new { MembershipId = membershipid, TotalAmout = totalAmount };

            return Json(updaterow, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvoice(string membershipid, string receiptnumber)
        {

            JustbokEntities objPaymentdata = new JustbokEntities();
            int memberId = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
            int membership = Convert.ToInt32(membershipid);
            int receiptno = Convert.ToInt32(receiptnumber);

            //var result1 = from m in db.MemberInfoes
            //              where m.MemberID == memberId
            //              select new { FirstName = m.FirstName, LastName = m.LastName };
            //var resul2=from s in db.MemberShips
            //           where s.MembershipId

            var invoice = (from m in db.MemberInfoes
                           join s in db.MemberShips on m.MemberID equals s.MemberID
                           join p in db.Payments on s.MembershipId equals p.MembershipId

                           select new
                           {
                               FirstName = m.FirstName + " " + m.LastName,
                               Address = m.MemberAddress,
                               Package = s.MembershipType,
                               StartDate = s.StartDate.ToString(),
                               EndDate = s.EndDate.ToString(),
                               Amount = s.Amount,
                               Membershipid = s.MembershipId,
                               RecieptNo = p.RecieptNumber
                           }).ToList();

            var result = invoice.Find(s => (s.Membershipid == membership) && (s.RecieptNo == receiptno));


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        private JsonResult GetPaymentData()
        {

            List<Payment> lstPaymentData = new List<Payment>();
            JustbokEntities objPaymentdata = new JustbokEntities();
            int memberId = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
            try
            {
                lstPaymentData = (from membership in objPaymentdata.Payments
                                  where membership.MemberID == memberId
                                  select membership).ToList<Payment>();
            }
            catch (Exception ex)
            {

            }

            return new JsonResult { Data = lstPaymentData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult GetMembersList()
        {

            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }


            return View();
        }


        [HttpGet]
        [OutputCache(CacheProfile = "tenMin")] 
        public JsonResult TotalMembersList(int? page, float pagesize,string sortBy, string sortDirection, int BranchId)
        {

            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var query = (from members in db.MemberInfoes
                                 where members.GymId == gymid && members.BranchId == BranchId
                                 select new
                                 {
                                     MemberID = members.MemberID,
                                     FirstName = members.FirstName,
                                     LastName = members.LastName,
                                     Email = members.Email,
                                     MemberAddress = members.MemberAddress,
                                     MobileNumber = members.MobileNumber
                                 });
            if (sortBy.ToUpper() == "MEMBERID" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.MemberID);
            }
            else if (sortBy.ToUpper() == "MEMBERID" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.MemberID);
            }
            else if(sortBy.ToUpper() == "MEMBERNAME" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.FirstName);
            }
            else if (sortBy.ToUpper() == "MEMBERNAME" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.FirstName);
            }
            else if(sortBy.ToUpper() == "EMAIL" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.Email);
            }
            else if (sortBy.ToUpper() == "EMAIL" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.Email);
            }
            else if (sortBy.ToUpper() == "MOBILE" && sortDirection.ToUpper() == "ASC")
            {
                query = query.OrderBy(s => s.MobileNumber);
            }
            else if (sortBy.ToUpper() == "MOBILE" && sortDirection.ToUpper() == "DESC")
            {
                query = query.OrderByDescending(s => s.MobileNumber);
            }


            var lstMemberList = query.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMemberList.Count / pagesize);

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
           

            return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            
        }




        [HttpGet]
        public ActionResult EditMember(string id)
        {
            MemberViewModel memberBind = new MemberViewModel();

            ViewBag.Stafflist = GetStaffList();
            ViewBag.OfferList = BindMembershipList();
            int memberId = Convert.ToInt32(id);
            System.Web.HttpContext.Current.Session["EditMemberId"] = memberId;
            //Utility.Logger.WriteLog("Member id converted");

            //Utility.Logger.WriteLog("Added to session");

            MemberInfo member = db.MemberInfoes.Find(memberId);
            //Utility.Logger.WriteLog("Find member");
            memberBind = EditMemberInfo(member);
            if (Request.IsAjaxRequest())
            {
                return PartialView(memberBind);
            }
            return View(memberBind);
            //return View();
        }
        
        [HttpPost]
        public JsonResult EditMember(MemberViewModel member)
        {
            //GetMemberInfoDetails
            MemberInfo objMemberInfo = new MemberInfo();
            objMemberInfo = GetMemberInfoDetails(member);
            //member.GymId = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            db.Entry(objMemberInfo).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true });

        }
       
        public ActionResult GetMembershipList()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }


          
            //int pagesize = pagesize = 10;
            //int pageIndex = 1;
            //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //IPagedList membershiplist = null;
            //int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            //var lstMembershipList = (from m in db.MemberInfoes
            //                         join s in db.MemberShips on m.MemberID equals s.MemberID
            //                         join p in db.Payments on s.MembershipId equals p.MembershipId
            //                         where m.GymId == gymid
            //                         select new MemberViewModel
            //                         {
            //                             MemberID = m.MemberID,
            //                             FirstName = m.FirstName + " " + m.LastName,
            //                             MobileNumber = m.MobileNumber,
            //                             MembershipType = s.MembershipType,
            //                             StartDate = s.StartDate,
            //                             EndDate = s.EndDate,
            //                             Amount = s.Amount,
            //                             PaymentAmount = p.PaymentAmount,

            //                         }).ToList();



            //return View(lstMembershipList.ToPagedList(pageIndex, pagesize));

            return View();
        }



        [HttpGet]
        public JsonResult MembershipList(int? page, float pagesize, int BranchId)
        {

            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstMembershipList = (from m in db.MemberInfoes
                                     join s in db.MemberShips on m.MemberID equals s.MemberID
                                     join p in db.Payments on s.MembershipId equals p.MembershipId
                                     where m.GymId == gymid && m.BranchId == BranchId
                                     select new 
                                     {
                                         MemberID = m.MemberID,
                                         FirstName = m.FirstName + " " + m.LastName,
                                         MobileNumber = m.MobileNumber,
                                         MembershipType = s.MembershipType,
                                         StartDate = s.StartDate.ToString(),
                                         EndDate = s.EndDate.ToString(),
                                         Amount = s.Amount,
                                         PaymentAmount = p.PaymentAmount,

                                     }).ToList();


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(lstMembershipList.Count / pagesize);

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


            return Json(new { Pages = pages, Result = lstMembershipList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

        }


        private MemberViewModel EditMemberInfo(MemberInfo objMember)
        {
            MemberViewModel objMemberDetails = new MemberViewModel();
            try
            {
                objMemberDetails.MemberID = objMember.MemberID;
                objMemberDetails.FirstName = objMember.FirstName;
                objMemberDetails.LastName = objMember.LastName;
                objMemberDetails.Dob = objMember.Dob;
                objMemberDetails.Gender = objMember.Gender;
                objMemberDetails.Email = objMember.Email;
                objMemberDetails.MobileNumber = objMember.MobileNumber;
                if(objMember.MemberReference!=null)
                {
                     objMemberDetails.MemberReference = objMember.MemberReference.ToString();
                }
               
                objMemberDetails.MemberAddress = objMember.MemberAddress;
                objMemberDetails.EnrollDate = objMember.EnrollDate;
                if (objMember.Married.Equals("Yes"))
                {
                    objMemberDetails.Married = true;
                }
                else
                {
                    objMemberDetails.Married = true;
                }

                objMemberDetails.SpouseName = objMember.SpouseName;
                objMemberDetails.SpouseBirthDate = objMember.SpouseBirthDate;
                objMemberDetails.AnniversaryDate = objMember.AnniversaryDate;
                objMemberDetails.Occupation = objMember.Occupation;
                objMemberDetails.Designation = objMember.Designation;
                objMemberDetails.MemberSource = objMember.MemberSource;
                objMemberDetails.PhoneResidence = objMember.PhoneResidence;
                objMemberDetails.PhoneOffice = objMember.PhoneOffice;
                objMemberDetails.ReferredBy = objMember.ReferredBy;
                objMemberDetails.Programme = objMember.Programme;
                objMemberDetails.Representative = objMember.Representative;
            }
            catch (Exception ex)
            {
                //Utility.Logger.SendErrorToText(ex);
            }
            return objMemberDetails;
        }


        public JsonResult EditBindMembership(string MemberId)
        {
            JustbokEntities objMembersdata = new JustbokEntities();
            NewMemberShip objMemberShip = new NewMemberShip();
            List<NewMemberShip> listMemberships = new List<NewMemberShip>();
            int totalAmount = 0;
            int memberid = Convert.ToInt32(MemberId);
            var memberShip = (from ms in db.MemberShips
                              where ms.MemberID == memberid
                              select ms).ToList();

            foreach (var memberships in memberShip)
            {
                totalAmount = 0;
                int getmembershipid = Convert.ToInt32(memberships.MembershipId);
                var paidAmount = (from amount in db.Payments
                                  where amount.MembershipId == getmembershipid
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }

                string Status = "";
                if (memberships.MemberFreezes != null && memberships.MemberFreezes.Count > 0)
                {
                    var freeze = memberships.MemberFreezes.Where(x => Convert.ToDateTime(x.StartDate).Date <= DateTime.Now.Date && Convert.ToDateTime(x.EndDate).Date >= DateTime.Now.Date).ToList();
                    if (freeze != null && freeze.Count > 0)
                    {
                        Status = "Freeze";
                    }
                    else
                    {
                        Status = memberships.Status.ToString();
                    }
                }
                else
                {
                    Status = memberships.Status.ToString();
                }


                listMemberships.Add(new NewMemberShip()
                {
                    MembershipID = memberships.MembershipId.ToString(),
                    MemershipType = memberships.MembershipType,
                    Months = memberships.Months.ToString(),
                    StartDate = Convert.ToDateTime(memberships.StartDate).ToString("MMM dd, yyyy"),
                    EndDate = Convert.ToDateTime(memberships.EndDate).ToString("MMM dd, yyyy"),
                    Amount = memberships.Amount.ToString(),
                    Status = Status,
                    PaidAmount = totalAmount.ToString(),
                    Notes = memberships.Note,
                    RemainingDays = Convert.ToInt32(Convert.ToDateTime(memberships.EndDate).Subtract(DateTime.Now).TotalDays).ToString(),
                });

            }

            return Json(listMemberships, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditMembership(string membershipId)
        {

            int membershipid = Convert.ToInt32(membershipId);
            var memberShip = (from ms in db.MemberShips
                              where ms.MembershipId == membershipid
                              select new
                              {
                                  MembershipID = ms.MembershipId,
                                  MemershipType = ms.MembershipType,
                                  StartDate = ms.StartDate.ToString(),
                                  EndDate = ms.EndDate.ToString(),
                                  Months = ms.Months,
                                  Amount = ms.Amount,
                                  Status = ms.Status,
                                  Note=ms.Note
                              }).ToList();

            return Json(memberShip, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddMeasurement(MemberViewModel measurement)
        {
            Measurement addMeasurement = new Measurement();
            addMeasurement = GetMeasurement(measurement);
            db.Measurements.Add(addMeasurement);
            db.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private Measurement GetMeasurement(MemberViewModel measurement)
        {
            int memberId = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
            Measurement addMeasurement = new Measurement();
            addMeasurement.Height = measurement.Height;
            addMeasurement.weight = measurement.weight;
            addMeasurement.UpperArm = measurement.UpperArm;
            addMeasurement.ForeArm = measurement.ForeArm;
            addMeasurement.Calves = measurement.Calves;
            addMeasurement.BMI = measurement.BMI;
            addMeasurement.VFat = measurement.VFat;
            addMeasurement.Shoulder = measurement.Shoulder;
            addMeasurement.Chest = measurement.Chest;
            addMeasurement.Arms = measurement.Arms;
            addMeasurement.UpperABS = measurement.UpperABS;
            addMeasurement.WaistABS = measurement.WaistABS;
            addMeasurement.LowerABS = measurement.LowerABS;
            addMeasurement.Glutes = measurement.Glutes;
            addMeasurement.Thighs = measurement.Thighs;
            addMeasurement.MeasurementDate = measurement.MeasurementDate;
            addMeasurement.NextMeasurementDate = measurement.NextMeasurementDate;
            addMeasurement.MemberID = memberId;

            return addMeasurement;

        }

        [HttpPost]
        public JsonResult UpdateMeasurement(MemberViewModel measurement)
        {
            Measurement addMeasurement = new Measurement();
            addMeasurement = UpdateMeasurementDetails(measurement);
            db.Entry(addMeasurement).State = measurement.MeasurementId == 0 ? EntityState.Added : EntityState.Modified;
            //db.Entry(addMeasurement).State = EntityState.Modified;
            db.SaveChanges();

            return Json(measurement);
        }

        private Measurement UpdateMeasurementDetails(MemberViewModel measurement)
        {
            int memberId = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            Measurement addMeasurement = new Measurement();
            if (measurement.MeasurementId > 0)
            {
                addMeasurement.MeasurementId = measurement.MeasurementId;
            }
            addMeasurement.Height = measurement.Height;
            addMeasurement.weight = measurement.weight;
            addMeasurement.UpperArm = measurement.UpperArm;
            addMeasurement.ForeArm = measurement.ForeArm;
            addMeasurement.Calves = measurement.Calves;
            addMeasurement.BMI = measurement.BMI;
            addMeasurement.VFat = measurement.VFat;
            addMeasurement.Shoulder = measurement.Shoulder;
            addMeasurement.Chest = measurement.Chest;
            addMeasurement.Arms = measurement.Arms;
            addMeasurement.UpperABS = measurement.UpperABS;
            addMeasurement.WaistABS = measurement.WaistABS;
            addMeasurement.LowerABS = measurement.LowerABS;
            addMeasurement.Glutes = measurement.Glutes;
            addMeasurement.Thighs = measurement.Thighs;
            addMeasurement.MeasurementDate = measurement.MeasurementDate;
            addMeasurement.NextMeasurementDate = measurement.NextMeasurementDate;
            addMeasurement.MemberID = memberId;

            return addMeasurement;

        }

        [HttpGet]
        public JsonResult BindMeasurement()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            var lstMeasurements = (from mi in db.Measurements
                         where mi.MemberID == memberid
                         orderby mi.MeasurementDate descending 
                         select new
                         {
                             MeasurementId = mi.MeasurementId,
                             Height = mi.Height,
                             weight = mi.weight,
                             UpperArm = mi.UpperArm,
                             ForeArm = mi.ForeArm,
                             Calves = mi.Calves,
                             BMI = mi.BMI,
                             VFat = mi.VFat,
                             Shoulder = mi.Shoulder,
                             Chest = mi.Chest,
                             Arms = mi.Arms,
                             UpperABS = mi.UpperABS,
                             WaistABS = mi.WaistABS,
                             LowerABS = mi.LowerABS,
                             Glutes = mi.Glutes,
                             Thighs = mi.Thighs,
                             MeasurementDate = mi.MeasurementDate.ToString(),
                             NextMeasurementDate = mi.NextMeasurementDate.ToString(),
                         }).ToList();

            return Json(lstMeasurements, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditMeasurement(string measurementid)
        {
            int measurementId = Convert.ToInt32(measurementid);
            var lstMeasurements = (from mi in db.Measurements
                                   where mi.MeasurementId == measurementId

                                   select new
                                   {
                                       MeasurementId = mi.MeasurementId,
                                       Height = mi.Height,
                                       weight = mi.weight,
                                       UpperArm = mi.UpperArm,
                                       ForeArm = mi.ForeArm,
                                       Calves = mi.Calves,
                                       BMI = mi.BMI,
                                       VFat = mi.VFat,
                                       Shoulder = mi.Shoulder,
                                       Chest = mi.Chest,
                                       Arms = mi.Arms,
                                       UpperABS = mi.UpperABS,
                                       WaistABS = mi.WaistABS,
                                       LowerABS = mi.LowerABS,
                                       Glutes = mi.Glutes,
                                       Thighs = mi.Thighs,
                                       MeasurementDate = mi.MeasurementDate.ToString(),
                                       NextMeasurementDate = mi.NextMeasurementDate.ToString(),
                                   }).ToList();

            return Json(lstMeasurements, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindImageToPrintMeasurement()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstGymImage = (from mi in db.GymImages
                                   where mi.GymId == gymid
                                   select new
                                   {
                                       GymImageId = mi.GymImageId,
                                       ImageData = mi.ImageData,
                                      
                                   }).ToList();


            return Json(lstGymImage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindMemberDetailsMeasurement()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            var lstGymImage = (from mi in db.MemberInfoes
                               join me in db.Measurements on mi.MemberID equals me.MemberID
                               where mi.MemberID == memberid
                               orderby me.NextMeasurementDate descending
                               select new
                               {
                                   FirstName = mi.FirstName,
                                   LastName = mi.LastName,
                                   EnrollDate = mi.EnrollDate.ToString(),
                                   Gender = mi.Gender,
                                   NextMeasurementDate = me.NextMeasurementDate.ToString(),

                               }).ToList();


            return Json(lstGymImage, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult BindWorkoutPlan(string palnid)
        {
           
            //int memberId = Convert.ToInt32(Session["MemberId"]);
            int Palnid = Convert.ToInt32(palnid);


            var workouts = (from mi in db.WorkoutPlanNames
                            join ms in db.WorkoutPlans on mi.PlaneNameId equals ms.PlaneNameId
                            where mi.PlaneNameId == Palnid

                            select new
                            {
                                PlaneNameId = mi.PlaneNameId,
                                PlanName = mi.PlanName,
                                Planid = ms.Planid,
                                Workout = ms.Workout,
                                NumberOfSets = ms.NumberOfSets,
                                NumberOfMinutes = ms.NumberOfMinutes,
                                NumberofDays = ms.NumberofDays,
                                ExcerciseOrder = ms.ExcerciseOrder,
                                Repeats = ms.Repeats
                            }).ToList();

            return Json(workouts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult BindDietWorkoutPlan(string palnid)
        {

            //int memberId = Convert.ToInt32(Session["MemberId"]);
            int dietid = Convert.ToInt32(palnid);


            var diets = (from mi in db.DietPlanNames
                         join ms in db.Diets on mi.DietPlanId equals ms.DietPlanId
                         where mi.DietPlanId == dietid

                         select new
                         {
                             DietPlanId = mi.DietPlanId,
                             DietPlanName1 = mi.DietPlanName1,
                             DietId = ms.DietId,
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

        [HttpPost]
        public JsonResult AddMemberWorkoutPlan(List<WorkoutPlanViewModel> workoutplan)
        {
            int PlaneNameId = 0;

            if (workoutplan != null)
            {
                //WorkoutPlanName addWorkoutplanname = new WorkoutPlanName();


                // addWorkoutplanname.PlanName=workoutpla

                for (int plan = 0; plan < workoutplan.Count; plan++)
                {
                    if (plan == 0 && workoutplan[plan].Planid != 0)
                    {

                        PlaneNameId = workoutplan[plan].Planid;
                    }

                    if (plan > 0)
                    {
                        MemberWorkoutPlan addworkoutplan = new MemberWorkoutPlan();
                        addworkoutplan.Workout = workoutplan[plan].Workout;
                        addworkoutplan.NumberOfSets = workoutplan[plan].NumberOfSets;
                        addworkoutplan.NumberOfMinutes = workoutplan[plan].NumberOfMinutes;
                        addworkoutplan.NumberofDays = workoutplan[plan].NumberofDays;
                        addworkoutplan.ExcerciseOrder = workoutplan[plan].ExcerciseOrder;
                        addworkoutplan.Repeats = workoutplan[plan].Repeats;
                        addworkoutplan.PlaneNameId = PlaneNameId;
                        addworkoutplan.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
                        db.MemberWorkoutPlans.Add(addworkoutplan);
                        db.SaveChanges();
                    }
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddMemberDietPlan(List<DietViewModel> diet)
        {
            int PlaneNameId = 0;

            if (diet != null)
            {
                for (int plan = 0; plan < diet.Count; plan++)
                {
                    if (plan == 0 && diet[plan].DietPlanName1 != null)
                    {

                        PlaneNameId = Convert.ToInt32(diet[plan].DietPlanName1);
                    }

                    if (plan > 0)
                    {
                        MemberDietPlan addDietplan = new MemberDietPlan();
                        addDietplan.DietTime = diet[plan].MealTime1;
                        addDietplan.MondayDiet = diet[plan].MondayDiet;
                        addDietplan.TuesdayDiet = diet[plan].TuesdayDiet;
                        addDietplan.WednesdayDiet = diet[plan].WednesdayDiet;
                        addDietplan.ThursdayDiet = diet[plan].ThursdayDiet;
                        addDietplan.FridayDiet = diet[plan].FridayDiet;
                        addDietplan.SaturdayDiet = diet[plan].SaturdayDiet;
                        addDietplan.SundayDiet = diet[plan].SundayDiet;
                        addDietplan.DietPlanId = PlaneNameId;
                        addDietplan.MemberID = Convert.ToInt32(System.Web.HttpContext.Current.Session["MemberId"]);
                        db.MemberDietPlans.Add(addDietplan);
                        db.SaveChanges();
                    }
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditMemberWorkoutPlan()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
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


        [HttpGet]
        public JsonResult EditMemberDietPlan()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
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


        public JsonResult UpdateWorkout(List<WorkoutPlanViewModel> workoutplan)
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            int PlaneNameId = 0;
           // bool delWorkout = true;
            if (workoutplan != null)
            {
                PlaneNameId = Convert.ToInt32(workoutplan[0].PlaneNameId);


                var savedMemberWorkoutPlanid = (from memberWorkout in db.MemberWorkoutPlans
                                                where memberWorkout.MemberID == memberid
                                                select new { memberWorkout.MemberWorkoutPlanid }).ToList();

                //check if any workout removed if yes delete those records from db
               
                foreach(var workout in savedMemberWorkoutPlanid)
                {
                  
                    var workoutid = workoutplan.Where(x => x.MemberWorkoutPlanid == workout.MemberWorkoutPlanid).ToList();
                    if (workoutid.Count == 0)
                    {
                        int memberWorkOutId = Convert.ToInt32(workout.MemberWorkoutPlanid);
                        var memberWorkout = db.MemberWorkoutPlans.Where(x => x.MemberWorkoutPlanid == memberWorkOutId).SingleOrDefault();
                        db.MemberWorkoutPlans.Remove(memberWorkout);
                        db.SaveChanges();
                    }
                }

                savedMemberWorkoutPlanid = (from memberWorkout in db.MemberWorkoutPlans
                                            where memberWorkout.MemberID == memberid
                                            select new { memberWorkout.MemberWorkoutPlanid }).ToList();
                
                //update records exits in enitity
                if (savedMemberWorkoutPlanid.Count > 0)
                {
                    foreach (var workout in workoutplan)
                    {
                        var isExistingWorkout = savedMemberWorkoutPlanid.Where(m => m.MemberWorkoutPlanid == workout.MemberWorkoutPlanid).ToList();
                        if (isExistingWorkout.Count > 0)
                        {
                            MemberWorkoutPlan addworkoutplan = new MemberWorkoutPlan();
                            addworkoutplan.Workout = workout.Workout;
                            if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "sets")
                            {
                                addworkoutplan.NumberOfSets = workout.NumberOfSets;
                                addworkoutplan.Repeats = workout.Repeats;
                            }
                            else if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "minutes")
                            {
                                addworkoutplan.NumberOfMinutes = workout.NumberOfSets;
                            }
                            addworkoutplan.ExcerciseOrder = workout.ExcerciseOrder;
                            addworkoutplan.MemberID = memberid;
                            addworkoutplan.PlaneNameId = PlaneNameId;
                            addworkoutplan.MemberWorkoutPlanid = workout.MemberWorkoutPlanid;
                            db.Entry(addworkoutplan).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            MemberWorkoutPlan addworkoutplan = new MemberWorkoutPlan();
                            addworkoutplan.Workout = workout.Workout;
                            if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "sets")
                            {
                                addworkoutplan.NumberOfSets = workout.NumberOfSets;
                                addworkoutplan.Repeats = workout.Repeats;
                            }
                            else if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "minutes")
                            {
                                addworkoutplan.NumberOfMinutes = workout.NumberOfSets;
                            }
                            addworkoutplan.NumberofDays = workout.NumberofDays;
                            addworkoutplan.ExcerciseOrder = workout.ExcerciseOrder;
                            addworkoutplan.PlaneNameId = PlaneNameId;
                            addworkoutplan.MemberID = memberid;
                            db.MemberWorkoutPlans.Add(addworkoutplan);
                            db.SaveChanges();
                        }
                    }

                }
                else
                {
                    foreach (var workout in workoutplan)
                    {
                        MemberWorkoutPlan addworkoutplan = new MemberWorkoutPlan();
                        addworkoutplan.Workout = workout.Workout;
                        if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "sets")
                        {
                            addworkoutplan.NumberOfSets = workout.NumberOfSets;
                            addworkoutplan.Repeats = workout.Repeats;
                        }
                        else if (workout.SetMin != null && workout.SetMin.ToLower().Trim() == "minutes")
                        {
                            addworkoutplan.NumberOfMinutes = workout.NumberOfSets;
                        }
                        addworkoutplan.NumberofDays = workout.NumberofDays;
                        addworkoutplan.ExcerciseOrder = workout.ExcerciseOrder;
                        addworkoutplan.PlaneNameId = PlaneNameId;
                        addworkoutplan.MemberID = memberid;
                        db.MemberWorkoutPlans.Add(addworkoutplan);
                        db.SaveChanges();
                    }

                
                }

            }


         
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

      
        public JsonResult UpdateDiet(List<DietViewModel> diet)
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            int PlaneNameId = 0;
            bool delDiet = true;
            if (diet != null)
            {
                PlaneNameId = Convert.ToInt32(diet[0].DietPlanId);
                for (int plan = 0; plan < diet.Count; plan++)
                {
                    MemberDietPlan adddietplan = new MemberDietPlan();
                    if (diet[plan].MemberDietPlanid > 0)
                    {
                        delDiet = false;
                        adddietplan.DietTime = diet[plan].MealTime1;
                        adddietplan.MondayDiet = diet[plan].MondayDiet;
                        adddietplan.TuesdayDiet = diet[plan].TuesdayDiet;
                        adddietplan.WednesdayDiet = diet[plan].WednesdayDiet;
                        adddietplan.ThursdayDiet = diet[plan].ThursdayDiet;
                        adddietplan.FridayDiet = diet[plan].FridayDiet;
                        adddietplan.SaturdayDiet = diet[plan].SaturdayDiet;
                        adddietplan.SundayDiet = diet[plan].SundayDiet;
                        adddietplan.DietPlanId = diet[plan].DietPlanId;
                        adddietplan.MemberID = memberid;
                        adddietplan.MemberDietPlanid = diet[plan].MemberDietPlanid;
                        db.Entry(adddietplan).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (delDiet)
                        {
                            //this will work if total changing of diet set  

                            var saveddietPlanid = (from memberDiet in db.MemberDietPlans
                                                   where memberDiet.MemberID == memberid
                                                   select new { memberDiet.MemberDietPlanid }).ToList();
                            if (saveddietPlanid.Count > 0)
                            {
                                foreach (var getdietId in saveddietPlanid)
                                {
                                    int MemberDietid = Convert.ToInt32(getdietId.MemberDietPlanid);
                                    var dietid = new MemberDietPlan { MemberDietPlanid = MemberDietid };
                                    db.Entry(dietid).State = EntityState.Deleted;
                                    db.SaveChanges();
                                    delDiet = false;
                                }
                            }
                            adddietplan.DietTime = diet[plan].MealTime1;
                            adddietplan.MondayDiet = diet[plan].MondayDiet;
                            adddietplan.TuesdayDiet = diet[plan].TuesdayDiet;
                            adddietplan.WednesdayDiet = diet[plan].WednesdayDiet;
                            adddietplan.ThursdayDiet = diet[plan].ThursdayDiet;
                            adddietplan.FridayDiet = diet[plan].FridayDiet;
                            adddietplan.SaturdayDiet = diet[plan].SaturdayDiet;
                            adddietplan.SundayDiet = diet[plan].SundayDiet;
                            adddietplan.DietPlanId = PlaneNameId;
                            adddietplan.MemberID = memberid;

                            db.MemberDietPlans.Add(adddietplan);
                            db.SaveChanges();
                        }
                        else
                        {
                            //this will work if already diet is there and adding new diet at last 
                            adddietplan.DietTime = diet[plan].MealTime1;
                            adddietplan.MondayDiet = diet[plan].MondayDiet;
                            adddietplan.TuesdayDiet = diet[plan].TuesdayDiet;
                            adddietplan.WednesdayDiet = diet[plan].WednesdayDiet;
                            adddietplan.ThursdayDiet = diet[plan].ThursdayDiet;
                            adddietplan.FridayDiet = diet[plan].FridayDiet;
                            adddietplan.SaturdayDiet = diet[plan].SaturdayDiet;
                            adddietplan.SundayDiet = diet[plan].SundayDiet;
                            adddietplan.DietPlanId = PlaneNameId;
                            adddietplan.MemberID = memberid;

                            db.MemberDietPlans.Add(adddietplan);
                            db.SaveChanges();
                        }

                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ResetMemberListSearch(int BranchId)
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstMemberList = (from members in db.MemberInfoes
                                 where members.GymId == gymid && members.BranchId == BranchId

                                 select new
                                 {
                                     MemberID = members.MemberID,
                                     FirstName = members.FirstName,
                                     LastName = members.LastName,
                                     Email = members.Email,
                                     MemberAddress = members.MemberAddress,
                                     MobileNumber = members.MobileNumber,
                                 }).ToList();
            return Json(lstMemberList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public JsonResult SearchMemberList(int? page, float pagesize, string Memberid, string Member, string Fromdate, string Todate, int Branchid)
        {
            int memberid=0;
            long mobilenumber = 0;
            string memberName = "";
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);

            if(Memberid!="")
            {
               memberid=Convert.ToInt32(Memberid);
            }

            try
            {
                mobilenumber = long.Parse(Member);
            }
            catch (Exception ex)
            {
                memberName = Member;
            }

            if (Fromdate != "")
            {
                startDate = DateTime.ParseExact(Fromdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            if (Todate != "")
            {
                endDate = DateTime.ParseExact(Todate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }



            if (memberName != "")
            {

                var lstMemberList = (from members in db.MemberInfoes
                                     where members.GymId == gymid && members.BranchId == Branchid && (members.FirstName.ToLower() + " " + members.LastName.ToLower()).Contains(memberName.Trim().ToLower())
                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         Email = members.Email,
                                         MemberAddress = members.MemberAddress,
                                         MobileNumber = members.MobileNumber,
                                     }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if (mobilenumber != 0)
            {
                var lstMemberList = (from members in db.MemberInfoes
                                     where members.GymId == gymid && members.BranchId==Branchid && members.MobileNumber.ToString().Contains(mobilenumber.ToString().Trim())
                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         Email = members.Email,
                                         MemberAddress = members.MemberAddress,
                                         MobileNumber = members.MobileNumber,
                                     }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if(memberid!=0)
            {
                var lstMemberList = (from members in db.MemberInfoes
                                     where members.GymId == gymid && members.BranchId == Branchid  && members.MemberID==memberid
                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         Email = members.Email,
                                         MemberAddress = members.MemberAddress,
                                         MobileNumber = members.MobileNumber,
                                     }).ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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
                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else if(Fromdate!="")
            {
                if (Todate == "")
                {
                    endDate = DateTime.Today.Date;
                }
                var lstMemberList = (from members in db.MemberInfoes
                                     where members.GymId == gymid && members.BranchId == Branchid && members.EnrollDate>=startDate && members.EnrollDate<=endDate
                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         Email = members.Email,
                                         MemberAddress = members.MemberAddress,
                                         MobileNumber = members.MobileNumber,
                                     }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var lstMemberList = (from members in db.MemberInfoes
                                     where members.GymId == gymid && members.BranchId == Branchid 

                                     select new
                                     {
                                         MemberID = members.MemberID,
                                         FirstName = members.FirstName,
                                         LastName = members.LastName,
                                         Email = members.Email,
                                         MemberAddress = members.MemberAddress,
                                         MobileNumber = members.MobileNumber,
                                     }).ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                double pages = Math.Ceiling(lstMemberList.Count / pagesize);
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

                return Json(new { Pages = pages, Result = lstMemberList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateMemberInformation()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstInfo = (from mi in db.MemberImages
                           //join ms in db.MemberShips on mi.MemberId equals ms.MemberID
                           where mi.MemberId == memberid
                           select new
                           {
                               ImageData = mi.ImageData,

                           }).ToList();

            return Json(lstInfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateMemberShipInformation()
        {
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstInfo = (from mi in db.MemberShips
                           join p in db.Payments on mi.MemberID equals p.MemberID
                           where mi.MemberID == memberid 
                           select new
                           {
                               MemberID = mi.MemberID,
                               MembershipType = mi.MembershipType,
                               EndDate = mi.EndDate.ToString(),
                               Amount = mi.Amount,
                               PaymentAmount = p.PaymentAmount

                           }).ToList();

            if (lstInfo.Count > 0)
            {
                var lstMembershipinfo = lstInfo.OrderByDescending(x => x.EndDate).First();
                return Json(lstMembershipinfo, JsonRequestBehavior.AllowGet);
            }

            return Json("error", JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        public JsonResult WorkoutMemberDetail()
        {
            //int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
            var memberdetails = (from member in db.MemberInfoes
                            where member.MemberID == memberid
                            select new
                            {
                                MemberID = member.MemberID,
                                FirstName = member.FirstName,
                                LastName = member.LastName,
                                EnrollDate = member.EnrollDate
                                
                            }).ToList();

            return Json(memberdetails, JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
         public JsonResult WorkoutGymDetail()
         {
             int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
             var gymdetails = (from gym in db.GymLists
                               join image in db.GymImages on gym.Gymid equals image.GymId
                               where gym.Gymid == gymid
                                  select new
                                  {
                                      GymName = gym.GymName,
                                      GymAddress = gym.GymAddress,
                                      ImageData = image.ImageData

                                  }).ToList();

             return Json(gymdetails, JsonRequestBehavior.AllowGet);
         }

         [HttpGet]
         public JsonResult WorkoutMembershipDetail()
         {
             int memberid = Convert.ToInt32(System.Web.HttpContext.Current.Session["EditMemberId"]);
             var startDate = (from d in db.MemberShips where d.MemberID == memberid select d.StartDate).Max();
             var membershipdetails = (from ms in db.MemberShips
                                      where ms.MemberID == memberid && ms.StartDate == startDate
                               select new
                               {
                                   MembershipType = ms.MembershipType,
                                   StartDate = ms.StartDate.ToString(),
                                   EndDate = ms.EndDate.ToString()

                               }).ToList();

             return Json(membershipdetails, JsonRequestBehavior.AllowGet);
         }

        [HttpGet]
        public JsonResult GetMemberDetails(string searchCharacter, int? page, int branchId)
        {
            int gymid = SessionManger.GymId;
            var Memebers = (from MemberInfo in db.MemberInfoes
                            where MemberInfo.GymId == gymid
                            where MemberInfo.BranchId == branchId
                            && ((MemberInfo.FirstName.ToUpper() + " " + MemberInfo.LastName.ToUpper()).Contains(searchCharacter.Trim().ToUpper())
                            || MemberInfo.MemberID.ToString().Contains(searchCharacter.Trim())
                            || MemberInfo.MobileNumber.ToString().Contains(searchCharacter.Trim()))

                            select new
                            {
                                FirstName = MemberInfo.FirstName,
                                LastName = MemberInfo.LastName,
                                MemberID = MemberInfo.MemberID,
                                MobileNumber = MemberInfo.MobileNumber,
                            }).ToList();

            int pagesize = pagesize = 6;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            return Json(new { Pages = Math.Ceiling(Memebers.Count / 6.0), Result = Memebers.ToPagedList(pageIndex, pagesize) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TransferMemberShip(int membershipId, int memberId, string Notes, DateTime StartDate, DateTime EndDate)
        {
            var membership = db.MemberShips.Find(membershipId);

            membership.Status = "Transfered";

            db.Entry(membership).State = EntityState.Modified;
            db.SaveChanges();

            MemberShip objMemberShip = new MemberShip();
            objMemberShip.MembershipType = membership.MembershipType;
            objMemberShip.Months = membership.Months;
            objMemberShip.Amount = membership.Amount;
            objMemberShip.StartDate = StartDate;
            objMemberShip.EndDate = EndDate;
            objMemberShip.Note = Notes;
            objMemberShip.Status = "Active";
            objMemberShip.MemberID = memberId;

            var paidAmount = (from amount in db.Payments
                              where amount.MembershipId == membershipId
                              select amount).ToList();

            List<Payment> payments = null;

            if (paidAmount != null)
            {
                payments = new List<Payment>();

                foreach (Payment amount in paidAmount)
                {
                    Payment objPayment = new Payment();
                    objPayment.PaymentType = amount.PaymentType;
                    objPayment.PaymentAmount = amount.PaymentAmount;
                    objPayment.PaymentDate = amount.PaymentDate;
                    objPayment.PaymentDueDate = amount.PaymentDueDate;
                    objPayment.ReferenceNumber = amount.ReferenceNumber;
                    objPayment.MemberID = memberId;
                    payments.Add(objPayment);
                }
            }

            objMemberShip.Payments = payments;

            db.Entry(objMemberShip).State = EntityState.Added;
            db.SaveChanges();




            return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FreezeMemberShip(MemberFreeze objMemberFreeze)
        {
            int diffDays = 0, addDays = 0;
            objMemberFreeze.IsActive = true;
            if (objMemberFreeze.FreezeId != 0)
            {

                var MemberFreez = db.MemberFreezes.Find(objMemberFreeze.FreezeId);
                diffDays = Convert.ToInt32(Convert.ToDateTime(MemberFreez.EndDate).Subtract(Convert.ToDateTime(MemberFreez.StartDate)).TotalDays) + 1;
                addDays = Convert.ToInt32(Convert.ToDateTime(objMemberFreeze.EndDate).Subtract(Convert.ToDateTime(objMemberFreeze.StartDate)).TotalDays) + 1;
                MemberFreez.StartDate = objMemberFreeze.StartDate;
                MemberFreez.EndDate = objMemberFreeze.EndDate;
                db.Entry(MemberFreez).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                diffDays = 0;
                addDays = Convert.ToInt32(Convert.ToDateTime(objMemberFreeze.EndDate).Subtract(Convert.ToDateTime(objMemberFreeze.StartDate)).TotalDays) + 1;
                db.Entry(objMemberFreeze).State = EntityState.Added;
                db.SaveChanges();
            }

            var membership = db.MemberShips.Find(objMemberFreeze.MemberShipId);

            DateTime EndDate = Convert.ToDateTime(membership.EndDate);

            EndDate = EndDate.AddDays(diffDays * -1);
            EndDate = EndDate.AddDays(addDays);
            membership.EndDate = EndDate;
            db.Entry(membership).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFreezeList(int MemberId)
        {
            var freezes = (from f in db.MemberFreezes
                           where f.Member_Id == MemberId
                           select f).ToList();

            return Json(FreezeList(freezes), JsonRequestBehavior.AllowGet);
        }

        private List<MemberFreezeModel> FreezeList(List<MemberFreeze> Freezes)
        {
            List<MemberFreezeModel> lstFreezes = new List<MemberFreezeModel>();

            if (Freezes != null && Freezes.Count > 0)
            {
                foreach (MemberFreeze objMemberFreeze in Freezes)
                {
                    MemberFreezeModel objMemberFreezeModel = new MemberFreezeModel();
                    objMemberFreezeModel.FreezeId = objMemberFreeze.FreezeId;
                    objMemberFreezeModel.MemberShipId = Convert.ToInt32(objMemberFreeze.MemberShipId);
                    objMemberFreezeModel.StartDate = Convert.ToDateTime(objMemberFreeze.StartDate).ToString("MMM d,yyyy");
                    objMemberFreezeModel.EndDate = Convert.ToDateTime(objMemberFreeze.EndDate).ToString("MMM d,yyyy");
                    objMemberFreezeModel.MemershipType = objMemberFreeze.MemberShip.MembershipType;
                    objMemberFreezeModel.Month = Convert.ToInt32(objMemberFreeze.MemberShip.Months);
                    objMemberFreezeModel.Amount = Convert.ToDecimal(objMemberFreeze.MemberShip.Amount);
                    if (Convert.ToDateTime(objMemberFreeze.EndDate).Date < DateTime.Now.Date)
                    {
                        objMemberFreezeModel.Status = "Freeze Completed";
                    }
                    else
                    {
                        objMemberFreezeModel.Status = "Freeze";
                    }
                    lstFreezes.Add(objMemberFreezeModel);
                }
            }

            return lstFreezes;
        }

        public JsonResult DeleteFreeze(int FreezeId)
        {

            var MemberFreez = db.MemberFreezes.Find(FreezeId);

            var EndDate = MemberFreez.EndDate;
            var StartDate = MemberFreez.StartDate;
            var MemberShipId = MemberFreez.MemberShipId;

            db.Entry(MemberFreez).State = EntityState.Deleted;
            db.SaveChanges();

            if (Convert.ToDateTime(EndDate).Date >= DateTime.Now.Date)
            {
                int diffDays = 0;
                if (Convert.ToDateTime(StartDate).Date > DateTime.Now.Date)
                {
                    diffDays = Convert.ToInt32(Convert.ToDateTime(EndDate).Subtract(Convert.ToDateTime(StartDate)).TotalDays)+1;
                }
                else
                {
                    diffDays = Convert.ToInt32(Convert.ToDateTime(EndDate).Subtract(Convert.ToDateTime(DateTime.Now.Date)).TotalDays);
                }

                var membership = db.MemberShips.Find(MemberShipId);

                DateTime MEndDate = Convert.ToDateTime(membership.EndDate);

                MEndDate = MEndDate.AddDays(diffDays*-1);
                membership.EndDate = MEndDate;
                db.Entry(membership).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SendMembershipMessage(string RequestType)
        {
            var member = "";
            if (RequestType.Equals("NewMembership"))
            {
                member = System.Web.HttpContext.Current.Session["MemberId"].ToString();
            }
            else if (RequestType.Equals("EditMembership"))
            {
                member = System.Web.HttpContext.Current.Session["EditMemberId"].ToString();
            }
           
            if(member!=""){
                var memberid = Convert.ToInt32(member);
                var lstmobile = (from m in db.MemberInfoes
                                 join ms in db.MemberShips on m.MemberID equals ms.MemberID
                                 join gn in db.GymLists on m.GymId equals gn.Gymid 
                                 where m.MemberID == memberid
                                 orderby ms.StartDate descending  
                               select new{
                                  MemberName=m.FirstName+" "+m.LastName,
                                 Mobile=m.MobileNumber,
                                 GenderDetails=m.Gender,
                                 MembershipName=ms.MembershipType,
                                 EnrollDate=m.EnrollDate.ToString(),
                                 GymName=gn.GymName
                               } 
                              )
                              .ToList();
                return Json(lstmobile, JsonRequestBehavior.AllowGet);
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public JsonResult ReferenceMemberSearch(int? page, float pagesize, int BranchId,string CustomerName)
        { 
           int gymid=Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            int memberid=0;
            string memberName="";
            try
            {
            memberid=Convert.ToInt32(CustomerName);
             }
            catch(Exception ex)
            {
               memberName=CustomerName;
            }

            if(memberid>0)
            {
                var customerList = (from cust in db.MemberInfoes
                                    where cust.GymId == gymid && cust.BranchId == BranchId && cust.MemberID == memberid
                                    select new
                                    {
                                        MemberID=cust.MemberID,
                                        MemberName = cust.FirstName+" "+cust.LastName,
                                        MobileNumber = cust.MobileNumber
                                    }).ToList();
                int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(customerList.Count / pagesize);
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
               return Json(new { Pages = pages, Result = customerList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);

               }
            else if(memberName!="")
            {
                var customerList=(from cust in db.MemberInfoes
                                      where cust.GymId==gymid && cust.BranchId==BranchId && 
                                      (cust.FirstName.ToLower()+" "+cust.LastName.ToLower()).Contains(memberName.ToLower())                                     
                                      select new {
                                          MemberID=cust.MemberID,
                                        MemberName = cust.FirstName+" "+cust.LastName,
                                        MobileNumber = cust.MobileNumber
                                      }
                                      ).ToList();
                int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            double pages = Math.Ceiling(customerList.Count / pagesize);
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
               return Json(new { Pages = pages, Result = customerList.ToPagedList(pageIndex, (int)pagesize) }, JsonRequestBehavior.AllowGet);
            }

            return Json("",JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult OnLoadBindEditMembership()
        {
            try
            {
                var user = System.Web.HttpContext.Current.Session["EditMemberId"].ToString();
                int memberId = Convert.ToInt32(user);
                var bindMembership = MembershipDetails(memberId);
                var bindMembershipHeaderDetails = HeaderMembershipDetails(memberId);
                var updateMemberInfo = UpdateMemberInfo(memberId);
                var bindWorkoutPlan = EditWorkoutPlan(memberId);
                var bindDietPlan = EditdietPlan(memberId);
                var bindFreeze = EditFreezeList(memberId);
                var bindMeasurement = EditMeasurements(memberId);
                var bindImageMeasurement = ImagePrintMeasurement();
                var bindMemberInfoMeasurement = MemberDetailsMeasurements(memberId);
                var sms = SMSLog(memberId);

                return Json(new
                {
                    BindMembership = bindMembership,
                    MembershipHeader = bindMembershipHeaderDetails,
                    UpdateMemberInfo = updateMemberInfo,
                    Workoutplan = bindWorkoutPlan,
                    DietPlan = bindDietPlan,
                    Freeze = bindFreeze,
                    MeasurementList = bindMeasurement,
                    ImageMeasurement = bindImageMeasurement,
                    InfoMeasurement = bindMemberInfoMeasurement,
                    SMS=sms

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { 
            
            }

            return Json("", JsonRequestBehavior.AllowGet);
          }

        private async Task<dynamic> MembershipDetails(int memberid)
        {
          
            JustbokEntities objMembersdata = new JustbokEntities();
            NewMemberShip objMemberShip = new NewMemberShip();
            List<NewMemberShip> listMemberships = new List<NewMemberShip>();
            int totalAmount = 0;
            var memberShip = (from ms in db.MemberShips
                              where ms.MemberID == memberid
                              select ms).ToList();

            foreach (var memberships in memberShip)
            {
                totalAmount = 0;
                int getmembershipid = Convert.ToInt32(memberships.MembershipId);
                var paidAmount = (from amount in db.Payments
                                  where amount.MembershipId == getmembershipid
                                  select new { amount.PaymentAmount }).ToList();
                foreach (var amount in paidAmount)
                {
                    totalAmount = totalAmount + Convert.ToInt32(amount.PaymentAmount);
                }

                string Status = "";
                if (memberships.MemberFreezes != null && memberships.MemberFreezes.Count > 0)
                {
                    var freeze = memberships.MemberFreezes.Where(x => Convert.ToDateTime(x.StartDate).Date <= DateTime.Now.Date && Convert.ToDateTime(x.EndDate).Date >= DateTime.Now.Date).ToList();
                    if (freeze != null && freeze.Count > 0)
                    {
                        Status = "Freeze";
                    }
                    else
                    {
                        Status = memberships.Status.ToString();
                    }
                }
                else
                {
                    Status = memberships.Status.ToString();
                }


                listMemberships.Add(new NewMemberShip()
                {
                    MembershipID = memberships.MembershipId.ToString(),
                    MemershipType = memberships.MembershipType,
                    Months = memberships.Months.ToString(),
                    StartDate = Convert.ToDateTime(memberships.StartDate).ToString("MMM dd, yyyy"),
                    EndDate = Convert.ToDateTime(memberships.EndDate).ToString("MMM dd, yyyy"),
                    Amount = memberships.Amount.ToString(),
                    Status = Status,
                    PaidAmount = totalAmount.ToString(),
                    Notes = memberships.Note,
                    RemainingDays = Convert.ToInt32(Convert.ToDateTime(memberships.EndDate).Subtract(DateTime.Now).TotalDays).ToString(),
                });

            }

            return listMemberships;
        
        }

        private async Task<dynamic> HeaderMembershipDetails(int memberid)
        {
            var lstInfo = (from mi in db.MemberShips
                           join p in db.Payments on mi.MemberID equals p.MemberID
                           where mi.MemberID == memberid
                           select new
                           {
                               MemberID = mi.MemberID,
                               MembershipType = mi.MembershipType,
                               EndDate = mi.EndDate.ToString(),
                               Amount = mi.Amount,
                               PaymentAmount = p.PaymentAmount

                           }).ToList();
            if (lstInfo.Count > 0)
            {
                var lstMembershipinfo = lstInfo.OrderByDescending(x => x.EndDate).First();
                return lstMembershipinfo;
            }
            return "";
                
            }
        private async Task<dynamic> UpdateMemberInfo(int memberid)
        {
            var lstInfo = (from mi in db.MemberImages
                           where mi.MemberId == memberid
                           select new
                           {
                               ImageData = mi.ImageData,

                           }).ToList();
           
            return lstInfo;
        }


        private async Task<dynamic> EditWorkoutPlan(int memberid)
        {
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
            return workouts;
        }

        private async Task<dynamic> EditdietPlan(int memberid)
        {
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
            return diets;

        }

        private async Task<dynamic> EditFreezeList(int memberid)
        {
          
            var freezes = (from f in db.MemberFreezes
                           where f.Member_Id == memberid
                           select f).ToList();
            return freezes;
        }

        private async Task<dynamic> EditMeasurements(int memberid)
        { 
            var lstMeasurements = (from mi in db.Measurements
                         where mi.MemberID == memberid
                         orderby mi.MeasurementDate descending 
                         select new
                         {
                             MeasurementId = mi.MeasurementId,
                             Height = mi.Height,
                             weight = mi.weight,
                             UpperArm = mi.UpperArm,
                             ForeArm = mi.ForeArm,
                             Calves = mi.Calves,
                             BMI = mi.BMI,
                             VFat = mi.VFat,
                             Shoulder = mi.Shoulder,
                             Chest = mi.Chest,
                             Arms = mi.Arms,
                             UpperABS = mi.UpperABS,
                             WaistABS = mi.WaistABS,
                             LowerABS = mi.LowerABS,
                             Glutes = mi.Glutes,
                             Thighs = mi.Thighs,
                             MeasurementDate = mi.MeasurementDate.ToString(),
                             NextMeasurementDate = mi.NextMeasurementDate.ToString(),
                         }).ToList();
            return lstMeasurements;
        }

        private async Task<dynamic> ImagePrintMeasurement()
        {
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            var lstGymImage = (from mi in db.GymImages
                               where mi.GymId == gymid
                               select new
                               {
                                   GymImageId = mi.GymImageId,
                                   ImageData = mi.ImageData,

                               }).ToList();
            return lstGymImage;
        }

        private async Task<dynamic> MemberDetailsMeasurements(int memberid)
        {
            var lstGymImage = (from mi in db.MemberInfoes
                               join me in db.Measurements on mi.MemberID equals me.MemberID
                               where mi.MemberID == memberid
                               orderby me.NextMeasurementDate descending
                               select new
                               {
                                   FirstName = mi.FirstName,
                                   LastName = mi.LastName,
                                   EnrollDate = mi.EnrollDate.ToString(),
                                   Gender = mi.Gender,
                                   NextMeasurementDate = me.NextMeasurementDate.ToString(),

                               }).ToList();
            return lstGymImage;
        }

        private async Task<dynamic> SMSLog(int memberid)
        {
            var lstSMS= (from mi in db.SMSHistories
                               where mi.MemberId == memberid
                               select new
                               {
                                   SmsDate = mi.SMSDate,
                                   Message = mi.SMSMessage,
                                   PhoneNumber = mi.PhoneNumber,
                               }).ToList();
            return lstSMS;
        }

        public JsonResult EnrolledMembersDetails(int EnquiryId)
        {
            if (EnquiryId > 0)
            {

                var details = (from enquiry in db.Enquiries
                               where enquiry.EnquiryId == EnquiryId
                               select new
                               {
                                   FirstName = enquiry.FirstName,
                                   LastName = enquiry.LastName,
                                   MobileNumber = enquiry.MobileNumber,
                                   DOB = enquiry.DOB.ToString(),
                                   EmailId=enquiry.EmailId,
                                   Gender=enquiry.Gender,
                                   Address=enquiry.Address,
                                   PhoneNumberOffice=enquiry.PhoneNumberOffice,
                                   PhoneNumberResidence=enquiry.PhoneNumberResidence,
                                   EnquiryDate = enquiry.EnquiryDate.ToString()
                               }).ToList();
                //ViewBag.EnquiryMemberInfo = Newtonsoft.Json.JsonConvert.SerializeObject(details);

                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
