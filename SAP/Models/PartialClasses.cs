using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAP.Models
{
    [MetadataType(typeof(SurveysMetadata))]
    public partial class Survey
    {
    }
    [MetadataType(typeof(QuestionsMetadata))]
    public partial class OfflineQuestion
    {

    }
    [MetadataType(typeof(QuestionsMetadata))]
    public partial class OnlineQuestion
    {

    }
}