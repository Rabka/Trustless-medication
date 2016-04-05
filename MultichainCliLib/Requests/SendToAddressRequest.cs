using System;
using System.Collections.Generic;
using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;
using Newtonsoft.Json;

namespace MultichainCliLib.Requests
{
	public class SendToAddressRequest:IRequest
	{
		Dictionary<string, int> amount;
		string address;
		public SendToAddressRequest (string address, Dictionary<string, int> amount)
		{
			this.address = address;
			this.amount = amount;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			SendToAddressResponse response = JsonConvert.DeserializeObject<SendToAddressResponse> (jsonOutput)  ;
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
			return string.Format ("sendtoaddress {0} '{1}'",address, jsonString);
		}
	}
}

