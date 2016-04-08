using System;

namespace MultichainCliLib.Responses
{
	public class PrepareLockUnspentResponse : IResponse
	{
		public PrepareLockUnspentResponse ()
		{
		}

		public string txid { get; set; }
		public int vout { get; set; }
	}
}

