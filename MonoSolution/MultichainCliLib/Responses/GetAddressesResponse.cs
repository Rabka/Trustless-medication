using System;

namespace MultichainCliLib
{
	public class GetAddressesResponse : IResponse
	{
		public GetAddressesResponse ()
		{
		}
		public string[] Addresses { get; set; }
	}
}

