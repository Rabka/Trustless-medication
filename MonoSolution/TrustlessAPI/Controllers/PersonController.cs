using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustLessAPI.Models;
using System.Net;
using Newtonsoft.Json;
using TrustLessModelLib;
using System.Web.Configuration;
using TrustLessAPI.Storage;

namespace TrustlessAPI.Controllers
{
    public class PersonController : Controller
	{
		public ActionResult GetPersons()
		{
			using (DataContext context = new DataContext())
			{
				return Content ( JsonConvert.SerializeObject(context.Persons.ToList().Select(x=> new Person() { Username = x.Username }))); 
			} 
		}

		private bool CreateNewPerson(Person person)
		{
			using (DataContext context = new DataContext())
			{
				var match =
					context.Persons.FirstOrDefault(x => x.PublicKey == person.PublicKey);

				if (match != null)
					return false;

				if (String.IsNullOrEmpty(person.PublicKey) || String.IsNullOrEmpty(person.Username) || String.IsNullOrEmpty(person.Password))
					return false;
 
				person.LoginSession = null;
				context.Persons.Add(person);
				context.SaveChanges();
				BlockChain.ImportPublicKeyToWallet (person);
				return true;
			}

		}
		[HttpPost]
		public ActionResult Login(Person person)
		{
			using (DataContext context = new DataContext())
			{
				var match =
					context.Persons.FirstOrDefault(x => x.PublicKey == person.PublicKey &&x.Username == person.Username && x.Password == person.Password);

				if (match == null) {
					if (!CreateNewPerson (person))
						return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
					match =
						context.Persons.FirstOrDefault(x => x.PublicKey == person.PublicKey &&x.Username == person.Username && x.Password == person.Password);
				}
 				 
				if (match.LoginSession != null)
					return Content ( JsonConvert.SerializeObject(RenewSession(context,match.LoginSession,match))); 
 
				match.LoginSession = RenewSession(context,null,match); 
				context.SaveChanges();

				return Content ( JsonConvert.SerializeObject(match.LoginSession)); 
			}

		}
		public static LoginSession RenewSession(DataContext context,LoginSession session,Person person)
		{  
			session = new LoginSession (Guid.NewGuid ().ToString (), DateTime.Now); 
			if (person.LoginSession != null)
				context.LoginSessions.Remove (person.LoginSession);
			person.LoginSession = session;
			context.SaveChanges();   
			return session;
		} 

		public static void UpdateLoginSession(DataContext context,LoginSession session)
		{ 
				session.LastUpdatedDateTime = DateTime.Now;
				context.SaveChanges();   
		}

		public static bool ValidateLoginSession(DataContext context,string token)
		{ 
			Person person = context.Persons.FirstOrDefault (x => x.LoginSessionToken != null && x.LoginSessionToken == token);

			int loginLifetimeMs = Convert.ToInt32(WebConfigurationManager.AppSettings["LoginLifetimeMs"]);
			if (person == null)
				return false;
			if (DateTime.Now - person.LoginSession.LastUpdatedDateTime > new TimeSpan (0, 0, 0, 0, loginLifetimeMs)) {
				context.LoginSessions.Remove (person.LoginSession);
				person.LoginSession = null;
				context.SaveChanges ();
				return false;
			}
			return true;
		}

    }
}
