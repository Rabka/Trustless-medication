using System;
using System.Collections.Generic;
using MultichainCliLib.Interfaces;
using Newtonsoft.Json;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class PrepareLockUnspentRequest : IRequest
	{
		Dictionary<string, int> amount;
		public PrepareLockUnspentRequest (Dictionary<string, int> amount)
		{
			this.amount = amount;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			PrepareLockUnspentResponse response = JsonConvert.DeserializeObject<PrepareLockUnspentResponse> (jsonOutput)  ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			string jsonString = "";
			foreach (KeyValuePair<string,int> kv in amount)
			{
				jsonString += string.Format (",\"{0}\":{1}",kv.Key,kv.Value);
			}
			jsonString = "{" + jsonString.Substring (1) + "}";
			return string.Format ("preparelockunspent '{0}'", jsonString);
		}
	}
}

