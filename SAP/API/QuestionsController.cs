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

namespace SAP.API
{
    [Authorize]
    [RoutePrefix("api/questions")]
    public class QuestionsController : ApiController
    {
        private SAPEntities db = new SAPEntities();

        public QuestionsController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }
        
        // GET: api/OfflineQuestions/5
        [ResponseType(typeof(OfflineQuestion))]
        [Route("1/{id:int}")]
        public IHttpActionResult GetOfflineQuestion(int id)
        {
            OfflineQuestion offlineQuestion = db.OfflineQuestion.SingleOrDefault(x => x.id_question == id);
            if (offlineQuestion == null)
            {
                return NotFound();
            }
            var model = new QuestionDTO
            {
                description = offlineQuestion.question_desc,
                required = offlineQuestion.is_required,
                title = offlineQuestion.question_text,
                type = offlineQuestion.question_type,
                survey_id = offlineQuestion.id_offline_survey,
                Id = offlineQuestion.id_question
            };
            return Ok(model);
        }
        [Route("2/{id:int}")]
        public IHttpActionResult GetOnlineQuestion(int id)
        {
            OnlineQuestion onlineQuestion = db.OnlineQuestion.SingleOrDefault(x => x.id_question == id);
            if (onlineQuestion == null)
            {
                return NotFound();
            }
            var model = new QuestionDTO
            {
                description = onlineQuestion.question_desc,
                required = onlineQuestion.is_required,
                title = onlineQuestion.question_text,
                type = onlineQuestion.question_type,
                survey_id = onlineQuestion.id_online_survey,
                Id = onlineQuestion.id_question
            };
            return Ok(model);
        }

        [Route("1")]
        // POST: api/OfflineQuestions
        [HttpPost]
        public IHttpActionResult PostOfflineQuestion(QuestionDTO question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(question == null)
            {
                return NotFound();
            }

            if (question.Id != null)
            {
                var offQ = db.OfflineQuestion.Find(question.Id);

                offQ.is_required = question.required;
                offQ.question_desc = question.description;
                offQ.question_text = question.title;
                offQ.question_type = question.type;

                db.Entry(offQ).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(offQ);
            }
            else
            {
                var newQ = new OfflineQuestion
                {
                    question_desc = question.description,
                    question_text = question.title,
                    question_type = question.type,
                    is_required = question.required,
                    id_offline_survey = question.survey_id
                };

                db.OfflineQuestion.Add(newQ);
                db.SaveChanges();

                return Ok(newQ);
            }
        }

        [Route("2")]
        // POST: api/OfflineQuestions
        [HttpPost]
        public IHttpActionResult PostOnlineQuestion(QuestionDTO question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (question == null)
            {
                return NotFound();
            }
            if (question.Id != null)
            {
                var onQ = db.OnlineQuestion.Find(question.Id);

                onQ.is_required = question.required;
                onQ.question_desc = question.description;
                onQ.question_text = question.title;
                onQ.question_type = question.type;

                db.Entry(onQ).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(onQ);
            }
            else
            {
                var newQ = new OnlineQuestion
                {
                    question_desc = question.description,
                    question_text = question.title,
                    question_type = question.type,
                    is_required = question.required,
                    id_online_survey = question.survey_id
                };

                db.OnlineQuestion.Add(newQ);
                db.SaveChanges();

                return Ok(newQ);
            }
        }

        [Route("1/{id:int}")]
        [HttpDelete]
        public IHttpActionResult DeleteOfflineQuestion(int id)
        {
            OfflineQuestion offlineQuestion = db.OfflineQuestion.Find(id);
            if (offlineQuestion == null)
            {
                return NotFound();
            }

            db.OfflineQuestion.Remove(offlineQuestion);
            db.SaveChanges();

            return Ok(offlineQuestion);
        }

        [Route("2/{id:int}")]
        [HttpDelete]
        public IHttpActionResult DeleteOnlineQuestion(int id)
        {
            OnlineQuestion onlineQuestion = db.OnlineQuestion.Find(id);
            if (onlineQuestion == null)
            {
                return NotFound();
            }

            db.OnlineQuestion.Remove(onlineQuestion);
            db.SaveChanges();

            return Ok(onlineQuestion);
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