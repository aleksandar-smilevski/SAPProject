using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.Models
{
    public class InterviewersToSurveys
    {
        public List<InterviewerInSurvey> Interviewers { get; set; }
        public Survey survey { get; set; }
        public InterviewersToSurveys(Survey survey, List<InterviewerInSurvey> Interviewers)
        {
            this.survey = survey;
            this.Interviewers = Interviewers;
        }
    }
}