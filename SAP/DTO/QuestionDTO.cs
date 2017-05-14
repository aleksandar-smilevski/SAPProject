using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAP.DTO
{
    public class QuestionDTO
    {
        public int? Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters")]
        public string title { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The type cannot be longer than 20 characters")]
        public string type { get; set; }
        public string description { get; set; }
        [Required]
        public bool required { get; set; }
        [Required]
        public int survey_id { get; set; }
        public List<string> values { get; set; }
    }
}