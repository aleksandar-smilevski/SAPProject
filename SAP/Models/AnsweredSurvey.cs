using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.Models
{
    public class AnsweredSurvey
    {
        public int SurveyId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}