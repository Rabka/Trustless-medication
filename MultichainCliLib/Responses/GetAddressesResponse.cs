using MultichainCliLib.Interfaces;

namespace MultichainCliLib.Responses
{
	public class GetAddressesResponse : IResponse
	{
		public GetAddressesResponse ()
		{
		}
		public string[] Addresses { get; set; }
	}
}

