using System;

namespace MultichainCliLib
{
	public class SendRawTransactionResponse : IResponse
	{
		public SendRawTransactionResponse ()
		{
		}

		public string TransactionId {get; set; }
	}
}

