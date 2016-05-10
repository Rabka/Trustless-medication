using System;

namespace MultichainCliLib
{
	public class GetTotalBalancesResponse : IResponse
	{
		public GetTotalBalancesResponse ()
		{
		}
		public AddressBalance[] Balances { get; set; }
	}
}

