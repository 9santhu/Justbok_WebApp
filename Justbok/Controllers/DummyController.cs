using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Justbok.Models;
using System.Data.Entity;

namespace Justbok.Controllers
{
    public class DummyController : Controller
    {
        //
        // GET: /Dummy/

        JustbokEntities db = new JustbokEntities();

        public ActionResult Index()
        {
            //Testtddsty
            return View(db.MemberInfoes.ToList());
        }
        [HttpGet]
        public ActionResult edit(string id)
        {
            MemberInfo member = db.MemberInfoes.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(MemberInfo member)
        {
            db.Entry(member).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

           // return View(member);
        }



        [HttpGet]
        public ActionResult Delete(string id)
        {
            MemberInfo member = db.MemberInfoes.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);

        }
        [HttpPost]
        public ActionResult DeleteConfirmed(string id)
        {
            MemberInfo member = db.MemberInfoes.Find(id);
            db.MemberInfoes.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
