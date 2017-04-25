using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAP.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using SAP.DTO;

namespace SAP.Controllers
{
    public class SurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: Surveys
        public ActionResult Index(int attr = 0)
        {
            if(attr == 0)
            {
                return View(db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).ToList());
            }
            return View(db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).Where(x => x.Survey_type == attr).ToList());
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

        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
        public ActionResult Design(int? id)
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
            //switch(survey.Survey_type)
            //{
            //    case 1: return View("OfflineDesign", survey);
            //    case 2: return View("OnlineDesign", survey);
            //    default: return HttpNotFound();
            //}
            return View(survey);
        }

        [HttpPost]
        public ActionResult Design(SurveyDTO survey)
        {
            //dynamic json = JValue.Parse(data);

            //string title = json.title;
            //JArray questions = (JArray)json.questions;
            //foreach(dynamic q in questions)
            //{
            //    string qtitle = q.title;
            //    Debug.WriteLine(qtitle);
            //}
            return null;
        }

        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
        public ActionResult Create(Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.Date = DateTime.Now;
                survey.Is_active = true;
                
                if (survey.Survey_type == 2)
                {
                    survey.OnlineSurvey = new OnlineSurvey();
                }
                else if (survey.Survey_type == 1)
                {
                    survey.OfflineSurvey = new OfflineSurvey();
                }
                db.Survey.Add(survey);
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
            //ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type", survey.Survey_type);
            return View(survey);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Survey survey)
        {
            if (ModelState.IsValid)
            {
                db.Entry(survey).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category = new SelectList(db.Category, "Id", "Category1", survey.Category);
            //ViewBag.Survey_type = new SelectList(db.SurveyType, "Id", "Survey_type", survey.Survey_type);
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
            if(survey == null)
            {
                return HttpNotFound();
            }
            if (survey.Survey_type == 1)
            {
                var questions = db.OfflineQuestion.Where(x => x.id_offline_survey == id).ToList();
                foreach (var q in questions)
                {
                    db.OfflineQuestion.Remove(q);
                }
                db.OfflineSurvey.Remove(db.OfflineSurvey.Where(x => x.Id == id).FirstOrDefault());
            }
            else if (survey.Survey_type == 2)
            {
                var questions = db.OnlineQuestion.Where(x => x.id_online_survey == id).ToList();
                foreach (var q in questions)
                {
                    db.OnlineQuestion.Remove(q);
                }
                db.OnlineSurvey.Remove(db.OnlineSurvey.Where(x => x.Id == id).FirstOrDefault());
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
