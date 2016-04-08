using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MultichainCliLib
{
	public class GetAddressesRequest : IRequest
	{ 

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			GetAddressesResponse response = new GetAddressesResponse() { Addresses = JsonConvert.DeserializeObject<string[]> (jsonOutput) } ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("getaddresses");
		}
	}
}

