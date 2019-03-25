using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GroupFinder.Models;

namespace GroupFinder.Controllers
{
    public class ClassMatesController : Controller
    {
        private GROUP_FINDEREntities db = new GROUP_FINDEREntities();

        // GET: ClassMates
        public ActionResult Index()
        {
            return View(db.ClassMates.ToList());
        }

        // GET: ClassMates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassMate classMate = db.ClassMates.Find(id);
            if (classMate == null)
            {
                return HttpNotFound();
            }
            return View(classMate);
        }

        // GET: ClassMates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClassMates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassMateId,email,fullname,loginhash,symbol")] ClassMate classMate)
        {
            if (ModelState.IsValid)
            {
                db.ClassMates.Add(classMate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(classMate);
        }

        // GET: ClassMates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassMate classMate = db.ClassMates.Find(id);
            if (classMate == null)
            {
                return HttpNotFound();
            }
            return View(classMate);
        }

        // POST: ClassMates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassMateId,email,fullname,loginhash,symbol")] ClassMate classMate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classMate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(classMate);
        }

        // GET: ClassMates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassMate classMate = db.ClassMates.Find(id);
            if (classMate == null)
            {
                return HttpNotFound();
            }
            return View(classMate);
        }

        // POST: ClassMates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassMate classMate = db.ClassMates.Find(id);
            db.ClassMates.Remove(classMate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
