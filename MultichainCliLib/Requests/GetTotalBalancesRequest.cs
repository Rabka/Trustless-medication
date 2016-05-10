using System;
using Newtonsoft.Json;

namespace MultichainCliLib
{
	public class GetTotalBalancesRequest : IRequest
	{ 
		public GetTotalBalancesRequest ()
		{ 
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			GetTotalBalancesResponse response = new GetTotalBalancesResponse() { Balances = JsonConvert.DeserializeObject<AddressBalance[]> (jsonOutput) } ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("gettotalbalances");
		}
	}
}

