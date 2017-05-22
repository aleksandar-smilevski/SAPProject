using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SAP.Models;
using SAP.DTO;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SAP.API
{
    [RoutePrefix("api/surveys")]
    public class SurveysController : ApiController
    {
        private SAPEntities db = new SAPEntities();

        public SurveysController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetSurvey(int id)
        {
            Survey survey = db.Survey.Where(x => x.Id == id).FirstOrDefault();
            if(survey == null)
            {
                return NotFound();
            }
            if(survey.Survey_type == 1)
            {
                return GetOfflineSurvey(id);
            }
            else
            {
                return GetOnlineSurvey(id);
            }
        }

        // GET: api/OfflineSurveys/5
        [HttpGet]
        [Route("1/{id:int}")]
        public IHttpActionResult GetOfflineSurvey(int id)
        {
            Survey offlineSurvey = db.Survey.Where(x => x.Id == id).FirstOrDefault();
            if (offlineSurvey == null)
            {
                return NotFound();
            }
            var questions = db.OfflineQuestion.Include(x => x.OfflineValues).Where(x => x.id_offline_survey == offlineSurvey.Id).ToList();
            List<QuestionDTO> questionsList = new List<QuestionDTO>();
            foreach(var q in questions)
            {
                var newQ = new QuestionDTO
                {
                    description = q.question_desc,
                    required = q.is_required,
                    title = q.question_text,
                    type = q.question_type,
                    survey_id = q.id_offline_survey,
                    Id = q.id_question
                };

                if (q.question_type != "text" && q.question_type != "textarea")
                {
                    var listOfValues = db.OfflineValues.Where(x => x.question_id == q.id_question).ToList();
                    List<string> values = new List<string>();
                    foreach(var val in listOfValues)
                    {
                        values.Add(val.Value);
                    }
                    newQ.values = values;
                }
                
                questionsList.Add(newQ);
            }
            var model = new SurveyDTO
            {
                Id = offlineSurvey.Id,
                Name = offlineSurvey.Name,
                Questions = questionsList,
                Survey_type = offlineSurvey.Survey_type
            };

            return Ok(model);
        }
        [HttpGet]
        [Route("2/{id:int}")]
        public IHttpActionResult GetOnlineSurvey(int id)
        {
            Survey onlineSurvey = db.Survey.Where(x => x.Id == id).FirstOrDefault();
            if (onlineSurvey == null)
            {
                return NotFound();
            }
            var questions = db.OnlineQuestion.Where(x => x.id_online_survey == onlineSurvey.Id).ToList();
            List<QuestionDTO> questionsList = new List<QuestionDTO>();
            foreach (var q in questions)
            {
                var newQ = new QuestionDTO
                {
                    description = q.question_desc,
                    required = q.is_required,
                    title = q.question_text,
                    type = q.question_type,
                    survey_id = q.id_online_survey,
                    Id = q.id_question
                };

                if (q.question_type != "text" && q.question_type != "textarea")
                {
                    var listOfValues = db.OnlineValues.Where(x => x.question_id == q.id_question).ToList();
                    List<string> values = new List<string>();
                    foreach (var val in listOfValues)
                    {
                        values.Add(val.Value);
                    }
                    newQ.values = values;
                }

                questionsList.Add(newQ);
            }
            var model = new SurveyDTO
            {
                Id = onlineSurvey.Id,
                Name = onlineSurvey.Name,
                Questions = questionsList,
                Survey_type = onlineSurvey.Survey_type
            };

            return Ok(model);
        }
        [HttpGet]
        [Route("link/{id:int}")]
        public IHttpActionResult GenerateLink(int id)
        {
            Survey survey = db.Survey.Find(id);
            Guid newGUID = Guid.NewGuid();
            var surDTO = new SurveyDTO
            {
                Id = survey.Id,
                Name = survey.Name
            };
            var model = new LinkDTO
            {
                UID = GuidToBase64(newGUID),
                Survey = surDTO
            };
            return Ok(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OfflineSurveyExists(int id)
        {
            return db.Survey.Count(e => e.Id == id) > 0;
        }

        public string GuidToBase64(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
        }

        [HttpPost]
        [Route("link")]
        public async Task<IHttpActionResult> SendingLinks(MessageDTO message)
        {
            if (message == null)
            {
                return BadRequest();
            }
            var survey = db.Survey.Find(message.SurveyId);
            if (survey == null)
            {
                return BadRequest();
            }
            var users = db.AspNetUsers.Find(User.Identity.GetUserId());
            if (users == null)
            {
                return BadRequest();
            }
            var link = new Links
            {
                SurveyID = message.SurveyId,
                UID = message.UID
            };
            var sends = new SendsSurvey
            {
                id_onlinesurvey = message.SurveyId,
                id_interviewer = users.Id,
            };
            db.Links.Add(link);
            db.SendsSurvey.Add(sends);
            await SendEmail(message.Email, "Invitation to Survey", "You have been invited to fill in the survey " + survey.Name + "<br> To answer the survey follow this link: " + message.URL);
            db.SaveChanges();
            return Ok();
        }

        public async Task SendEmail(string toEmailAddress, string emailSubject, string emailMessage)
        {
            var message = new MailMessage();
            message.To.Add(toEmailAddress);
            message.IsBodyHtml = true;
            message.Subject = emailSubject;
            message.Body = emailMessage;

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(message);
            }
        }

    }
}