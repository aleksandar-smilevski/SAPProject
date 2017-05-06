using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAP.Models
{
    public class SurveysMetadata
    {
        [Required]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters")]
        public string Name { get; set; }
    }

    public class QuestionsMetadata
    {
        [Required]
        [StringLength(50, ErrorMessage = "The title cannot be longer than 50 characters")]
        public string question_text { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The type cannot be longer than 20 characters")]
        public string question_type { get; set; }

        [Required]
        public bool is_required { get; set; }
    }
}