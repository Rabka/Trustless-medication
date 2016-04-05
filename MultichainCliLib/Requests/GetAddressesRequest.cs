using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;
using Newtonsoft.Json;

namespace MultichainCliLib.Requests
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

