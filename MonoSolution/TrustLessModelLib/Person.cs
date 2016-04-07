﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrustLessModelLib
{
    public class Person
    {
        [Key]
		public string Username { get; set; }
		[System.Runtime.Serialization.IgnoreDataMember]
		public string Password { get; set; }
        public string PublicKey { get; set; }
    }
}