using System;
using Newtonsoft.Json;

namespace MultichainCliLib
{
	public class GetTxOutRequest : IRequest
	{
		string txid; 
		int vout; 
		public GetTxOutRequest (string txid,int vout)
		{
			this.txid = txid;
			this.vout = vout;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			GetTxOutResponse response = JsonConvert.DeserializeObject<GetTxOutResponse> (jsonOutput)  ;
			return response;
		}

		public string GetCompleteMethodString()
		{ 
			return string.Format ("gettxout '{0}' {1} {2}", txid,vout,"true");
		}
	}
}

