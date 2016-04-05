using System;
using System.Collections.Generic;
using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class LockUnspentRequest : IRequest
	{ 
		bool unlock;
		Dictionary<string, object> txVout;
		public LockUnspentRequest (bool unlock, Dictionary<string, object> txVout)
		{
			this.unlock = unlock;
			this.txVout = txVout;
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
			string jsonTxString = "";
			foreach (KeyValuePair<string,object> kv in txVout)
			{
				jsonTxString += string.Format (",{{\"{0}\":{1}}}",kv.Key,typeof(string) == kv.Value.GetType() ? "\"" + kv.Value + "\"" : kv.Value);
			}
			jsonTxString = "[" + jsonTxString.Substring (1) + "]";
 
			return string.Format ("lockunspent {0} '{1}'", unlock, jsonTxString);
		}
	}
}

