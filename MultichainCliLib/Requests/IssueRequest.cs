using System;

namespace MultichainCliLib
{
	public class IssueRequest : IRequest
	{
		string address; 
		string assetName; 
		int qty; 
		double unit; 

		public IssueRequest (string address,string assetName, int qty, double unit)
		{
			this.address = address;
			this.assetName = assetName;
			this.qty = qty;
			this.unit = unit;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			IssueResponse response = new IssueResponse() { txid = json.Trim() };
			return response;
		}

		public string GetCompleteMethodString()
		{ 
			return string.Format ("issue '{0}' {1} {2} {3}", address,assetName, qty,unit);
		}
	}
}

