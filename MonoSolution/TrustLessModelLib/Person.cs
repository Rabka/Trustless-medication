using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrustLessModelLib
{
    public class Person
    {
		public Person()
		{

		}

		[Key]
		public string Username {get;set;}
		public string Password { get; set; }
		public string PublicKey { get; set; }

		[NotMapped]
		public double Reputation { get; set; }

		[ForeignKey("LoginSession")]
		public string LoginSessionToken { get; set; } 
		public virtual LoginSession LoginSession { get; set; }
    }
}