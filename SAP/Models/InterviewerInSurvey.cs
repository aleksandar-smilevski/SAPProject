using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.Models
{
    public class InterviewerInSurvey
    {
        public AspNetUsers User { get; set; }
        public bool isInSurvey { get; set; }
    }
}