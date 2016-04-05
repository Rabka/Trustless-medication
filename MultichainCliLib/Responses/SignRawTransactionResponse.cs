using MultichainCliLib.Interfaces;

namespace MultichainCliLib.Responses
{
	public class SignRawTransactionResponse :IResponse
	{
		public string hex { get; set; }
		public bool complete { get; set; }
	}
}

