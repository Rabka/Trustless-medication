namespace MultichainCliLib.Interfaces
{
	public interface IRequest
	{ 
		IResponse GenerateResponse(string json);
		string GetCompleteMethodString();
	}
}

