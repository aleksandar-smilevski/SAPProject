using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.DTO
{
    public class FillsDTO
    {
        public int survey_id { get; set; }
        public AnswerDTO[] answers { get; set; }
    }
}