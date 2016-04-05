using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;
using Newtonsoft.Json;

namespace MultichainCliLib.Requests
{
	public class GetAddressbalancesRequest : IRequest
	{
		string address; 
		public GetAddressbalancesRequest (string address)
		{
			this.address = address;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			GetAddressbalancesResponse response = new GetAddressbalancesResponse() { Balances = JsonConvert.DeserializeObject<AddressBalance[]> (jsonOutput) } ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("getaddressbalances '{0}'", address);
		}
	}
}

