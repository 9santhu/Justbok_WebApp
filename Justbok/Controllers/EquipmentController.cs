using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;
using System.IO;
using PagedList;

namespace Justbok.Controllers
{
    public class EquipmentController : Controller
    {
        //
        // GET: /Equipment/
        JustbokEntities db = new JustbokEntities();

        public ActionResult EquipmentList(int? page)
        {
            int pagesize = pagesize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //  IPagedList memberlist = null;
            int gymid = Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedGym"]);
            List<Equipment> lstEquipmentList = (from equipment in db.Equipments
                                              select equipment).ToList();
            return View(lstEquipmentList.ToPagedList(pageIndex, pagesize));
        }

        [HttpPost]
        public JsonResult AddEquipment(Equipment equipment)
        {
            Equipment addEquipment = new Equipment();
            addEquipment.EquipmentId = equipment.EquipmentId;
            addEquipment.EquipmentName = equipment.EquipmentName;
            db.Entry(addEquipment).State = addEquipment.EquipmentId == 0 ? EntityState.Added : EntityState.Modified;
            // db.Amenities.Add(addAmenity);
            db.SaveChanges();


            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditEquipment(string EquipmentId)
        {
            int equipId = Convert.ToInt32(EquipmentId);
            var equipmentList = (from mi in db.Equipments
                                 where mi.EquipmentId == equipId

                               select new
                               {
                                   EquipmentId = mi.EquipmentId,
                                   EquipmentName = mi.EquipmentName,
                               }).ToList();


            return Json(equipmentList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteEquipment(string EquipmentId)
        {
            if (EquipmentId != null)
            {
                int equipId = Convert.ToInt32(EquipmentId);
                var equipment = db.Equipments.Where(x => x.EquipmentId == equipId).SingleOrDefault();
                if (equipment != null)
                {
                    db.Equipments.Remove(equipment);
                    db.SaveChanges();
                }

            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

    }
}
