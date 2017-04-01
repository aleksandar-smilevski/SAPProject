using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAP.Models;

namespace SAP.Controllers
{
    public class OfflineSurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: OfflineSurveys
        public ActionResult Index()
        {
            var offlineSurvey = db.OfflineSurvey.Include(o => o.Survey);
            return View(offlineSurvey.ToList());
        }

        // GET: OfflineSurveys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfflineSurvey offlineSurvey = db.OfflineSurvey.Find(id);
            if (offlineSurvey == null)
            {
                return HttpNotFound();
            }
            return View(offlineSurvey);
        }

        // GET: OfflineSurveys/Create
        public ActionResult Create()
        {
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name");
            return View();
        }

        // POST: OfflineSurveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Id_survey")] OfflineSurvey offlineSurvey)
        {
            if (ModelState.IsValid)
            {
                db.OfflineSurvey.Add(offlineSurvey);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", offlineSurvey.Id_survey);
            return View(offlineSurvey);
        }

        // GET: OfflineSurveys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfflineSurvey offlineSurvey = db.OfflineSurvey.Find(id);
            if (offlineSurvey == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", offlineSurvey.Id_survey);
            return View(offlineSurvey);
        }

        // POST: OfflineSurveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Id_survey")] OfflineSurvey offlineSurvey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(offlineSurvey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", offlineSurvey.Id_survey);
            return View(offlineSurvey);
        }

        // GET: OfflineSurveys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfflineSurvey offlineSurvey = db.OfflineSurvey.Find(id);
            if (offlineSurvey == null)
            {
                return HttpNotFound();
            }
            return View(offlineSurvey);
        }

        // POST: OfflineSurveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OfflineSurvey offlineSurvey = db.OfflineSurvey.Find(id);
            db.OfflineSurvey.Remove(offlineSurvey);
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
