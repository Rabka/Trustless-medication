using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;
using Newtonsoft.Json;

namespace MultichainCliLib.Requests
{
	public class SignRawTransactionRequest : IRequest
	{
		string hex; 
		public SignRawTransactionRequest (string hex)
		{ 
			this.hex = hex;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			SignRawTransactionResponse response = JsonConvert.DeserializeObject<SignRawTransactionResponse> (jsonOutput)  ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("signrawtransaction \"{0}\"", hex);
		}
	}
}

