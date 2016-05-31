using System;

namespace MultichainCliLib
{
	public class AppendRawChangeRequest :IRequest
	{
		string hex;
		string address;
		public AppendRawChangeRequest (string hex, string address)
		{
			this.address = address; 
			this.hex = hex;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			AppendRawChangeResponse response = new AppendRawChangeResponse() { Hex = jsonOutput.Trim() };
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("appendrawchange \"{0}\" \"{1}\"", hex, address);
		}
	}
}

