using System;

namespace MultichainCliLib
{
	public class SignRawTransactionResponse :IResponse
	{
		public string hex { get; set; }
		public bool complete { get; set; }
	}
}

