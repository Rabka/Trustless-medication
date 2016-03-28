using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TrustLessAPI.Storage;

namespace TrustLessAPI.Models
{
    public class Recommendation
    {
        [Key]
        [Column(Order = 1)] 
        [ForeignKey("Person")]
        public int PersonId { get; set; } 
        public Person Person { get; set; }

        [Key]
        [Column(Order = 2)] 
        [ForeignKey("Statement")]
        public int StatementId { get; set; } 
        public Statement Statement { get; set; }
        public bool IsRecommended { get; set; }
        public ReservedTransaction Transaction { get; set; }

    }
}