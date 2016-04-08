using System;

namespace MultichainCliLib.Responses
{

	[Serializable]
	public class ListAssetsResponseAsset : IResponse
	{
		public string name { get; set; }
		public string issuetxid { get; set; }
		public object assetref { get; set; }
		public int multiple { get; set; }
		public int units { get; set; }
		public ListAssetsResponseDetails details { get; set; }
		public double issueqty { get; set; }
		public int issueraw { get; set; }
	}

	[Serializable]
	public class ListAssetsResponseDetails
	{
	}
 
	public class ListAssetsResponse : IResponse
	{
		public ListAssetsResponse ()
		{
			
		}
		public ListAssetsResponseAsset[] Assets {get;set;}
	}
}

