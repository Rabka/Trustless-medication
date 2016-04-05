using System.Collections.Generic;
using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class CreateRawTransactionRequest: IRequest
	{
		string address;
		Dictionary<string, object> txVout;
		Dictionary<string, int> amount;
		public CreateRawTransactionRequest (string address, Dictionary<string, object> txVout, Dictionary<string, int> amount)
		{
			this.address = address;
			this.amount = amount;
			this.txVout = txVout;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			CreateRawTransactionResponse response = new CreateRawTransactionResponse() { Hex = jsonOutput };
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

			string jsonString = "";
			foreach (KeyValuePair<string,int> kv in amount)
			{
				jsonString += string.Format (",\"{0}\":{1}",kv.Key,kv.Value);
			}
			jsonString = string.Format("{{\"{0}\":,{1}}}",address,"{" + jsonString.Substring (1) + "}");
				
			return string.Format ("createrawtransaction '{0}' '{1}'", jsonTxString, jsonString);
		}
	}
}

