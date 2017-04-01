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
    public class OnlineSurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: OnlineSurveys
        public ActionResult Index()
        {
            var onlineSurvey = db.OnlineSurvey.Include(o => o.Survey);
            return View(onlineSurvey.ToList());
        }

        // GET: OnlineSurveys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSurvey onlineSurvey = db.OnlineSurvey.Find(id);
            if (onlineSurvey == null)
            {
                return HttpNotFound();
            }
            return View(onlineSurvey);
        }

        // GET: OnlineSurveys/Create
        public ActionResult Create()
        {
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name");
            return View();
        }

        // POST: OnlineSurveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Id_survey")] OnlineSurvey onlineSurvey)
        {
            if (ModelState.IsValid)
            {
                db.OnlineSurvey.Add(onlineSurvey);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", onlineSurvey.Id_survey);
            return View(onlineSurvey);
        }

        // GET: OnlineSurveys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSurvey onlineSurvey = db.OnlineSurvey.Find(id);
            if (onlineSurvey == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", onlineSurvey.Id_survey);
            return View(onlineSurvey);
        }

        // POST: OnlineSurveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Id_survey")] OnlineSurvey onlineSurvey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(onlineSurvey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_survey = new SelectList(db.Survey, "Id", "Name", onlineSurvey.Id_survey);
            return View(onlineSurvey);
        }

        // GET: OnlineSurveys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OnlineSurvey onlineSurvey = db.OnlineSurvey.Find(id);
            if (onlineSurvey == null)
            {
                return HttpNotFound();
            }
            return View(onlineSurvey);
        }

        // POST: OnlineSurveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OnlineSurvey onlineSurvey = db.OnlineSurvey.Find(id);
            db.OnlineSurvey.Remove(onlineSurvey);
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
