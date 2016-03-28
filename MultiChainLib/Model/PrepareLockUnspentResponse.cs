using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiChainLib.Model
{
    public class PrepareLockUnspentResponse
    { 
            public Result result { get; set; }
            public object error { get; set; }
            public object id { get; set; } 
    }
    public class Result
    {
        public string txid { get; set; }
        public int vout { get; set; }
    } 
     
}
