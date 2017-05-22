using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAP.Models;
using SAP.DTO;

namespace SAP.Controllers
{
    public class fController : Controller
    {
        private SAPEntities db = new SAPEntities();

        // GET: f
        [HttpGet]
        public ActionResult S(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var survey = db.Links.Where(x => x.UID == id).Select(x => x.Survey).SingleOrDefault();
            if (survey == null)
            {
                return HttpNotFound();
            }
            var listOfQuestions = db.OnlineQuestion.Where(x => x.id_online_survey == survey.Id).ToList();
            var model = new FillOutOnlineSurveyViewModel
            {
                Survey = survey,
                Questions = listOfQuestions
            };
            return View("Index", model);

        }

        [HttpPost]
        public ActionResult FillOut(FillsDTO data)
        {
            var survey = db.Survey.Where(x => x.Id == data.survey_id).FirstOrDefault();
            if (survey == null)
            {
                return Json("Not Ok", JsonRequestBehavior.AllowGet);
            }
            //var newPaperSurvey = new PaperSurvey
            //{
            //    id_offlinesurvey = data.survey_id,
            //    id_interviewer = User.Identity.GetUserId()
            //};
            //db.PaperSurvey.Add(newPaperSurvey);
            //db.SaveChanges();
            foreach (var ans in data.answers)
            {
                var newOnAns = new OnlineAnswer
                {
                    answer_text = ans.Answer,
                    id_question = ans.QuestionId
                };
                db.OnlineAnswer.Add(newOnAns);
            }
            db.SaveChanges();

            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        // GET: f/Details/5
        public ActionResult ThankYou()
        {
            return View("Details");
        }

        // GET: f/Create
        //public ActionResult Create()
        //{
        //    ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name");
        //    return View();
        //}

        //// POST: f/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UID,SurveyID")] Links links)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Links.Add(links);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
        //    return View(links);
        //}

        //// GET: f/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Links links = db.Links.Find(id);
        //    if (links == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
        //    return View(links);
        //}

        //// POST: f/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UID,SurveyID")] Links links)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(links).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.SurveyID = new SelectList(db.Survey, "Id", "Name", links.SurveyID);
        //    return View(links);
        //}

        //// GET: f/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Links links = db.Links.Find(id);
        //    if (links == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(links);
        //}

        //// POST: f/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Links links = db.Links.Find(id);
        //    db.Links.Remove(links);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
