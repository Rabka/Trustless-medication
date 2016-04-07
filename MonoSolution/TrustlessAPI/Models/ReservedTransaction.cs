﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TrustLessAPI.Storage
{
    public class ReservedTransaction
    {
		[Key]
        public string Tx { get; set; }

        public int Vout { get; set; }
    }
}