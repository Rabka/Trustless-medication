using System;
using System.ComponentModel.DataAnnotations;

namespace TrustLessModelLib
{
	public class LoginSession
	{
		[Key]
		public string Token { get; set; }
		public DateTime LastUpdatedDateTime { get; set; }
		public LoginSession()
		{
		}
		public LoginSession(string token,DateTime lastUpdatedDateTime)
		{
			Token = token;
			LastUpdatedDateTime = lastUpdatedDateTime;
		}
	}
}

