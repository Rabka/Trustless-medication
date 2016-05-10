using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultichainCliLib
{
	public class CreateRawTransactionRequest: IRequest
	{
		string address;
		string txid;
		int vout;
		Dictionary<string, int> amount;
		public CreateRawTransactionRequest (string address, string txid, int vout, Dictionary<string, int> amount)
		{
			this.address = address;
			this.amount = amount;
			this.txid = txid;
			this.vout = vout;
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
			string jsonTxString = "[{\"txid\":\""+txid + "\",\"vout\":"+vout + "}]";

			string jsonString = "";
			foreach (KeyValuePair<string,int> kv in amount)
			{
				jsonString += string.Format (",\"{0}\":{1}",kv.Key,kv.Value);
			}
			jsonString = string.Format("{{\"{0}\":{1}}}",address,"{" + jsonString.Substring (1) + "}");
				
			return string.Format ("createrawtransaction '{0}' '{1}'", jsonTxString, jsonString);
		}
	}
}

