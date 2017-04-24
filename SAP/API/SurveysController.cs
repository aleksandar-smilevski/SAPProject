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
            var questions = db.OfflineQuestion.Where(x => x.id_offline_survey == offlineSurvey.Id).ToList();
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
    }
}