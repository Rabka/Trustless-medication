using MultichainCliLib.Interfaces;

namespace MultichainCliLib.Responses
{
	public class CreateRawTransactionResponse: IResponse
	{
		public string Hex { get; set;}  
	}
}

