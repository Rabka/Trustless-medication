using MultichainCliLib.Interfaces;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class ImportAddressRequest : IRequest
	{
		string address; 
		public ImportAddressRequest (string address)
		{
			this.address = address;
		}

		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			ImportAddressResponse response = new ImportAddressResponse() { };
			return response;
		}

		public string GetCompleteMethodString()
		{ 
			return string.Format ("importaddress '{0}'", address);
		}
	}
}

