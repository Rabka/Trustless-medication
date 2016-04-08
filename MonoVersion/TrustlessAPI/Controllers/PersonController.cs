using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustLessAPI.Models;
using System.Net;
using Newtonsoft.Json;
using TrustLessModelLib;

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

		[HttpPost]
		public ActionResult CreateNewPerson(Person person)
		{
			using (DataContext context = new DataContext())
			{
				var match =
					context.Persons.FirstOrDefault(x => x.Username == person.Username);

				if (match != null)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				if (String.IsNullOrEmpty(person.PublicKey))
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				context.Persons.Add(person);
				context.SaveChanges();

				return new HttpStatusCodeResult((int)HttpStatusCode.Created);
			}

		}
    }
}
