using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustLessAPI.Models
{
    /// <summary>
    /// Statement model class.
    /// </summary>
    public class Statement
    {
        public int Id { get; set; }
        public Person Person { get; set; }
        public string MedicinOne { get; set; }
        public string MedicinTwo { get; set; }
        public string Description { get; set; }
    }
}