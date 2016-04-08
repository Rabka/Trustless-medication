using System;

namespace MultichainCliLib
{
	public interface IRequest
	{ 
		IResponse GenerateResponse(string json);
		string GetCompleteMethodString();
	}
}

