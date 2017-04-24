using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.DTO
{
    public class QuestionDTO
    {
        public int? Id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        public int survey_id { get; set; }
        public List<string> radios { get; set; }
    }
}