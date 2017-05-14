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
    public class fController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: f
        public ActionResult s(string id)
        {
            var links = db.Links.Include(l => l.Survey);
            return View(links.ToList());
        }

        // GET: f/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Links links = db.Links.Find(id);
            if (links == null)
            {
                return HttpNotFound();
            }
            return View(links);
        }

        // GET: f/Create
        public ActionResult Create()
        {
            ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name");
            return View();
        }

        // POST: f/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UID,SurveyID")] Links links)
        {
            if (ModelState.IsValid)
            {
                db.Links.Add(links);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
            return View(links);
        }

        // GET: f/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Links links = db.Links.Find(id);
            if (links == null)
            {
                return HttpNotFound();
            }
            ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
            return View(links);
        }

        // POST: f/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UID,SurveyID")] Links links)
        {
            if (ModelState.IsValid)
            {
                db.Entry(links).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
            return View(links);
        }

        // GET: f/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Links links = db.Links.Find(id);
            if (links == null)
            {
                return HttpNotFound();
            }
            return View(links);
        }

        // POST: f/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Links links = db.Links.Find(id);
            db.Links.Remove(links);
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
