using System;
using System.Collections.Generic;

namespace MultichainCliLib
{
	public class GetTxOutResponse : IResponse
	{
		public string bestblock { get; set; }
		public int confirmations { get; set; }
		public double value { get; set; }
		public ScriptPubKey scriptPubKey { get; set; }
		public int version { get; set; }
		public bool coinbase { get; set; }
		public List<Asset> assets { get; set; }
		public List<object> permissions { get; set; }
		public GetTxOutResponse ()
		{ 
		}
	}

	public class ScriptPubKey
	{
		public string asm { get; set; }
		public string hex { get; set; }
		public int reqSigs { get; set; }
		public string type { get; set; }
		public List<string> addresses { get; set; }
	}

	public class Asset
	{
		public string name { get; set; }
		public string issuetxid { get; set; }
		public string assetref { get; set; }
		public double qty { get; set; }
		public int raw { get; set; }
	}

}

