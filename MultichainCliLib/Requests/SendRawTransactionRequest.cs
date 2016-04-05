using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class SendRawTransactionRequest : IRequest
	{
		string hex; 
		public SendRawTransactionRequest (string hex)
		{ 
			this.hex = hex;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			SendRawTransactionResponse response = new SendRawTransactionResponse() { TransactionId = jsonOutput };
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("sendrawtransaction \"{0}\"", hex);
		}
	}
}

