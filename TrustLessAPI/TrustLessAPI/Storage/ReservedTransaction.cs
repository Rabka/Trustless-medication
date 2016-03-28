using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrustLessAPI.Storage
{
    public class ReservedTransaction
    {
        public string Tx { get; set; }

        public int Vout { get; set; }
    }
}