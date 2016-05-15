using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrustLessAPI.Models
{
    /// <summary>
    /// Person (user) model class.
    /// </summary>
    public class Person
    {
        [Key]
		public string Username { get; set; }
		public string Password { get; set; }
        public string PublicKey { get; set; }
    }
}