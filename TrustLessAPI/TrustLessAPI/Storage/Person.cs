using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrustLessAPI.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PublicKey { get; set; }
    }
}