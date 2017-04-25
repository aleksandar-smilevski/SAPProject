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
using Microsoft.AspNet.Identity;


namespace SAP.Controllers
{
    
    public class SurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        // GET: Surveys
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
        public ActionResult Index(int attr = 0)
        {
            if(attr == 0)
            {
                return View(db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).ToList());
            }
            return View(db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).Where(x => x.Survey_type == attr).ToList());
        }

        // GET: Surveys/Details/5
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin, Interviewer")]
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
            return View(survey);
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
        public ActionResult AddInterviewers(int? id)
        {
            Survey survey = db.Survey.Find(id);
            if(survey == null)
            {
                return HttpNotFound();
            }
            List<AspNetUsers> allInterviewers = db.AspNetUsers.Where(x => x.AspNetRoles.Any(y => y.Name == "Interviewer")).ToList();
            List<AspNetUsers> interviewersInSurvey = db.AddToSurvey.Where(x => x.Id_survey == survey.Id).Select(x => x.AspNetUsers).ToList();
            List<InterviewerInSurvey> interviewers = new List<InterviewerInSurvey>();
            foreach (var i in allInterviewers)
            {
                if(interviewersInSurvey.Where(x => x.Id == i.Id).Any())
                {
                    interviewers.Add(new InterviewerInSurvey { isInSurvey = true, User = i });
                }
                else
                {
                    interviewers.Add(new InterviewerInSurvey { isInSurvey = false, User = i });
                }
            }
            var model = new InterviewersToSurveys(survey, interviewers);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AddInterviewersPartial", model);
            }
            return View(model);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
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
        [SAP.Attributes.AccessDeniedAuthorize(Roles = "Admin")]
        public ActionResult SwitchState(string id, int survey_id, bool canEdit)
        {
            if (canEdit)
            {
                AddToSurvey entry = db.AddToSurvey.Where(x => x.Id_survey == survey_id && x.Id_interviewer == id).FirstOrDefault();
                if(entry == null)
                {
                    return View();
                }
                db.AddToSurvey.Remove(entry);
            }
            else
            {
                var entry = new AddToSurvey
                {
                    Id_interviewer = id,
                    Id_survey = survey_id
                };
                db.AddToSurvey.Add(entry);
            }
            db.SaveChanges();
            return RedirectToAction("AddInterviewers", new { id = survey_id });
        }
    }
}
