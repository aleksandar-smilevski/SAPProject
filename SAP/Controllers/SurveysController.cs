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
    public class SurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: Surveys
        public ActionResult Index(int attr = 0)
        {
            
            if(attr == 1)
            {
                return View(db.Survey.Include(s => s.Category1).Where(x => x.Survey_type == attr).ToList());
            }
            else if(attr == 2)
            {
                return View(db.Survey.Include(s => s.Category1).Where(x => x.Survey_type == attr).ToList());
            }

            var list = db.Survey.Include(s => s.Category1).Include(s => s.SurveyType);
            return View(list.ToList());
        }

        // GET: Surveys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Survey.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // GET: Surveys/Create
        public ActionResult Create()
        {

            ViewBag.Category = new SelectList(db.Category, "Id", "Category1");
            ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type");
            
            return View();
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.Date = DateTime.Now;
                survey.Is_active = true;
                db.Survey.Add(survey);
                if(survey.Survey_type == 2)
                {
                    var onlinesurvey = new OnlineSurvey
                    {
                        Id_survey = survey.Id
                    };
                    db.OnlineSurvey.Add(onlinesurvey);
                }
                else if(survey.Survey_type==1)
                {
                    var offlinesurvey = new OfflineSurvey
                    {
                        Id_survey = survey.Id
                    };
                    db.OfflineSurvey.Add(offlinesurvey);
                }
              
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category = new SelectList(db.Category, "Id", "Category1", survey.Category);
            ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type", survey.Survey_type);
            return View(survey);
        }

        // GET: Surveys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Survey.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(db.Category, "Id", "Category1", survey.Category);
            ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type", survey.Survey_type);
            return View(survey);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Category,Survey_type,Date")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(survey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Category, "Id", "Category1", survey.Category);
            ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type", survey.Survey_type);
            return View(survey);
        }

        // GET: Surveys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = db.Survey.Find(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            Survey survey = db.Survey.Find(id);
            if (survey.Survey_type == 1)
            {
                var off = db.OfflineSurvey.Where(x => x.Id_survey == id).First();
                db.OfflineSurvey.Remove(off);
            }
            else if (survey.Survey_type == 2) {
                var on = db.OnlineSurvey.Where(x => x.Id_survey == id).First();
                db.OnlineSurvey.Remove(on);
            }
           
            db.Survey.Remove(survey);
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
