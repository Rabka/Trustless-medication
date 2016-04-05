using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web; 

namespace TrustLessModelLib
{
    public class Recommendation
    {
		[Key]
		[Column(Order = 1)] 
		[ForeignKey("Person")]
		public string PersonUsername { get; set; } 
		public Person Person { get; set; }

		[Key]
		[Column(Order = 2)] 
		[ForeignKey("Statement")]
		public int StatementId { get; set; } 
		public Statement Statement { get; set; }

        public bool IsRecommended { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.None)] //REMEMBER THAT YOU ARE MAKING RECOMMEND AND IT CANNOT INSERT THIS AS NULL
        public ReservedTransaction Transaction { get; set; }

    }
}