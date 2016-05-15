using System;
using System.Collections.Generic;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class LockUnspentRequest : IRequest
	{ 
		bool unlock;
		string txid;
		int vout;
		public LockUnspentRequest (bool unlock, string txid,int vout)
		{
			this.unlock = unlock;
			this.txid = txid;
			this.vout = vout;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			LockUnspentResponse response = new LockUnspentResponse() { Success = Convert.ToBoolean(jsonOutput) };
			return response;
		}

		public string GetCompleteMethodString()
		{

			string jsonTxString = "[{\"txid\":\""+txid + "\",\"vout\":"+vout + "}]";
 
			return string.Format ("lockunspent {0} '{1}'", unlock, jsonTxString);
		}
	}
}

