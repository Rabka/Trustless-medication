using System;
using MultichainCliLib.Interfaces;
using Newtonsoft.Json;
using MultichainCliLib.Responses;

namespace MultichainCliLib.Requests
{
	public class ListAssetsRequest : IRequest
	{
		public ListAssetsRequest ()
		{
		}
 
		public IResponse GenerateResponse(string json)
		{
			string[] lines = json.Split ('\n');
			string inputCmd = lines [0];
			string jsonOutput = json.Substring (inputCmd.Length);	

			ListAssetsResponse response = new ListAssetsResponse(){ Assets= JsonConvert.DeserializeObject<ListAssetsResponseAsset[]> (jsonOutput)} ;
			return response;
		}

		public string GetCompleteMethodString()
		{
			return string.Format ("listassets");
		}
	}
}

