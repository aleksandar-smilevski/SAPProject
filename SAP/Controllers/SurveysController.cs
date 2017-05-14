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
using Webdiyer.WebControls.Mvc;

namespace SAP.Controllers
{

    public class SurveysController : Controller
    {
        private SAPEntities db = new SAPEntities();
        private ApplicationDbContext db1 = new ApplicationDbContext();
        // GET: Surveys
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string search, string sortBy, string Category, int attr = 0, int id = 1)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortBy) ? "name_desc" : "";
            ViewBag.DateSortParm = sortBy == "Date" ? "date_desc" : "Date";
            ViewBag.Category = new SelectList(db.Category, "Id", "Category1");
            if (attr == 0)
            {
                var surveys = db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).AsQueryable();
                if (!String.IsNullOrEmpty(Category) && !String.IsNullOrEmpty(search))
                {
                    surveys = surveys.Where(x => x.Category1.Id.ToString().Equals(Category)).AsQueryable();
                    surveys = surveys.Where(x => x.Name.ToLower().Contains(search.ToLower())).AsQueryable();

                }
                else if (!String.IsNullOrEmpty(search) && String.IsNullOrEmpty(Category))
                    surveys = surveys.Where(x => x.Name.ToLower().Contains(search.ToLower())).AsQueryable();

                else if (!String.IsNullOrEmpty(Category) && String.IsNullOrEmpty(search))
                {
                    surveys = surveys.Where(x => x.Category1.Id.ToString().Equals(Category)).AsQueryable();
                }

                switch (sortBy)
                {
                    case "name_desc":
                        surveys = surveys.OrderByDescending(s => s.Name);
                        break;
                    case "Date":
                        surveys = surveys.OrderBy(s => s.Date);
                        break;
                    case "date_desc":
                        surveys = surveys.OrderByDescending(s => s.Date);
                        break;
                    default:
                        surveys = surveys.OrderBy(s => s.Name);
                        break;
                }



                return View(surveys.ToPagedList(id, 4));
            }


            var surveys1 = db.Survey.Include(s => s.Category1).Include(s => s.SurveyType).Where(x => x.Survey_type == attr).AsQueryable();


            if (!String.IsNullOrEmpty(Category) && !String.IsNullOrEmpty(search))
            {
                surveys1 = surveys1.Where(x => x.Category1.Id.ToString().Equals(Category)).AsQueryable();
                surveys1 = surveys1.Where(x => x.Name.ToLower().Contains(search.ToLower())).AsQueryable();

            }
            else if (!String.IsNullOrEmpty(search) && String.IsNullOrEmpty(Category))
                surveys1 = surveys1.Where(x => x.Name.ToLower().Contains(search.ToLower())).AsQueryable();

            else if (!String.IsNullOrEmpty(Category) && String.IsNullOrEmpty(search))
            {
                surveys1 = surveys1.Where(x => x.Category1.Id.ToString().Equals(Category)).AsQueryable();
            }

            switch (sortBy)
            {
                case "name_desc":
                    surveys1 = surveys1.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    surveys1 = surveys1.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    surveys1 = surveys1.OrderByDescending(s => s.Date);
                    break;
                default:
                    surveys1 = surveys1.OrderBy(s => s.Name);
                    break;
            }



            return View(surveys1.ToPagedList(id, 4));
        }

        [Authorize]
        public ActionResult Statistics(int id_clicked)
        {

            var questions = db.OfflineQuestion.Where(x => x.OfflineSurvey.Id.Equals(id_clicked)).ToList();
            return View(questions);
        }
        public ActionResult Analyze(int id_clicked) {
            var query = (from OffAnswer in db.OfflineAnswer
                         join OffQuestion in db.OfflineQuestion on OffAnswer.id_question equals OffQuestion.id_question

                         where OffQuestion.id_question.Equals(id_clicked)
                         group OffAnswer by OffAnswer.answer_text into g
                         select new
                         {
                             g
                         }
                );

            var list = new List<SurveyStatistics>();

            foreach (var item in query) {
                var tmp = item.g.Key;
                var pom = item.g.ToList().Count();

                list.Add(new SurveyStatistics()
                {


                    answerName = tmp,
                    count = pom

                });
            }
            
            return View(list);
        }

        // GET: Surveys/Details/5
        [Authorize]
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
                    var values = db.OfflineValues.Where(x => x.OfflineQuestion.id_question.Equals(q.id_question)).ToList();
                    foreach (var v in values) {
                        db.OfflineValues.Remove(v);
                    }
                    var answers = db.OfflineAnswer.Where(x => x.OfflineQuestion.id_question.Equals(q.id_question)).ToList();
                    foreach (var a in answers) {
                        db.OfflineAnswer.Remove(a);
                    }
                    
                    
                    db.OfflineQuestion.Remove(q);

                }
                var papers = db.PaperSurvey.Where(x => x.OfflineSurvey.Id.Equals(id)).ToList();
                foreach (var p in papers) {
                    db.PaperSurvey.Remove(p);
                }
                //db.OfflineSurvey.Remove(db.OfflineSurvey.Where(x => x.Id == id).FirstOrDefault());
                var sur = db.OfflineSurvey.Where(x=>x.Id.Equals(id)).ToList().FirstOrDefault();
                db.OfflineSurvey.Remove(sur);

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

            var addToSurvey = db.AddToSurvey.Where(x => x.Id_survey == id).ToList();
            foreach (var a in addToSurvey)
            {
                db.AddToSurvey.Remove(a);
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

        public ActionResult Answers(int survey_id)
        {

            var query = (
                         from offSurvey in db.OfflineSurvey
                         join papSurvey in db.PaperSurvey on offSurvey.Id equals papSurvey.id_offlinesurvey
                         join offQuestion in db.OfflineQuestion on offSurvey.Id equals offQuestion.id_offline_survey
                         join offAnswer in db.OfflineAnswer on offQuestion.id_question equals offAnswer.id_question
                         where  offAnswer.id_paper.Equals( papSurvey.id) where survey_id.Equals(offSurvey.Id)
                         select new {
                            papSurvey.id,
                            offQuestion.id_question,
                            offQuestion.question_text,
                            offAnswer.answer_text
                        }
                        );
            var surveys = new List<QueryAnswers>();
            foreach (var t in query)
            {
                surveys.Add(new QueryAnswers()
                {
                    paperId = t.id,
                    idQuestion = t.id_question,
                    questionText = t.question_text,
                    answersText=t.answer_text});
            }
            return View(surveys);
        }

        [HttpGet]
        public ActionResult FillOut(int? survey_id)
        {
            if(survey_id == null)
            {
                return null;
            }
            var survey = db.Survey.Where(x => x.Id == survey_id).FirstOrDefault();
            if(survey == null)
            {
                return null;
            }
            var listOfQuestions = db.OfflineQuestion.Where(x => x.id_offline_survey == survey.Id).ToList();
            var model = new FillOutSurveyQuestionsViewModel
            {
                Survey = survey,
                Questions = listOfQuestions
            };
            return View(model);

        }

        [HttpPost]
        public ActionResult FillOut(FillsDTO data)
        {
            var survey = db.Survey.Where(x => x.Id == data.survey_id).FirstOrDefault();
            if(survey == null)
            {
                return Json("Not Ok", JsonRequestBehavior.AllowGet);
            }
            var newPaperSurvey = new PaperSurvey
            {
                id_offlinesurvey = data.survey_id,
                id_interviewer = User.Identity.GetUserId()
            };
            db.PaperSurvey.Add(newPaperSurvey);
            db.SaveChanges();
            foreach (var ans in data.answers)
            {
                var newOffAns = new OfflineAnswer
                {
                    answer_text = ans.Answer,
                    id_question = ans.QuestionId,
                    id_paper = newPaperSurvey.id
                };
                db.OfflineAnswer.Add(newOffAns);
            }
            db.SaveChanges();

            
            
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }
    }
}
