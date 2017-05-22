using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAP.DTO
{
    public class SurveyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Survey_type { get; set; }
        public List<QuestionDTO> Questions { get; set; }
    }
}