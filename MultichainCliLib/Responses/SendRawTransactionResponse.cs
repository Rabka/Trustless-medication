using MultichainCliLib.Interfaces;

namespace MultichainCliLib.Responses
{
	public class SendRawTransactionResponse : IResponse
	{
		public SendRawTransactionResponse ()
		{
		}

		public string TransactionId {get; set; }
	}
}

