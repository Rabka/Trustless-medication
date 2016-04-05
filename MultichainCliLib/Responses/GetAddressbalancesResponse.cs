using MultichainCliLib.Interfaces;

namespace MultichainCliLib.Responses
{
	public class GetAddressbalancesResponse : IResponse
	{
		public GetAddressbalancesResponse ()
		{
		}
		public AddressBalance[] Balances { get; set; }
	}
	public class AddressBalance
	{
		public string name { get; set; }
		public string assetref { get; set; }
		public double qty { get; set; }
	}
}

