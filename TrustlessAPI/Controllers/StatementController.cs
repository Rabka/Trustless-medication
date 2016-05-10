using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustLessAPI.Models;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Net;
using TrustLessAPI.Storage; 
using TrustLessModelLib;
using System.IO;
using System.Web.Configuration;

namespace TrustlessAPI.Controllers
{
    public class StatementController : Controller
    {

		public ActionResult GetStatements(string token)
		{ 
			using (DataContext context = new DataContext())
			{ 

				if (!PersonController.ValidateLoginSession (context, token)) {

					return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);  
				}
				Person person = context.Persons.FirstOrDefault (x => x.LoginSessionToken != null && x.LoginSessionToken == token);

				int maxVotings = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxVotings"]);
				double initialTrust = Convert.ToDouble(WebConfigurationManager.AppSettings["InitialTrust"]);
				int maxStatementsForVoting = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxStatementsForVoting"]);
				var statements = context.Statements.Include ("Person").Where (x => x.Person.Username != person.Username && context.Recommendations.Count (y => y.StatementId == x.Id) < maxVotings &&
					!context.Recommendations.Any (y => y.PersonUsername == person.Username && y.StatementId == x.Id));
				if (CalculateBayesianModelTrust (person) <= initialTrust) {
					//					List<Statement> finalStatement = new List<Statement> ();
					//					foreach (Statement statement in statements) {
					//						var people = 
					//							context.Persons.Where(x => 
					//								//Haven't voted on the statement
					//								x.Username != person.Username && !context.Recommendations.Any (y => y.PersonUsername == person.Username && y.StatementId == statement.Id)
					//								//Has made a statement that is correct.
					//							&& context.Statements.Any (y=> context.Recommendations.Count(z => z.StatementId == y.Id) == maxVotings && ((y.Person.Username == x.Username &&  context.Recommendations.Count(z => z.StatementId == y.Id && z.IsRecommended) >= maxVotings / 2) ||
					//								//Contains a recommendation that is correct with the outcome
					//								(context.Recommendations.Any (z => z.PersonUsername == person.Username && z.StatementId == y.Id
					//										&& context.Recommendations.Count(a => a.StatementId == z.StatementId && z.IsRecommended == a.IsRecommended) >= maxVotings / 2))))).ToList();
					//						
					//						people.RemoveAll (x => CalculateBayesianModelTrust (x) <= initialTrust);
					//						if (!people.Any ())
					//							finalStatement.Add (statement);
					//					}
					//					return Content ( JsonConvert.SerializeObject(finalStatement));
					var dt = DateTime.Now - new TimeSpan (8, 0, 0, 0);
					if (context.Recommendations.Any (x => x.CreationDate < dt))
						return Content ("[]");

					string newId = Guid.NewGuid ().ToString();
					return Content ( JsonConvert.SerializeObject( statements.OrderBy(r => newId).Take(maxStatementsForVoting).ToList ()));

				}
				return Content ( JsonConvert.SerializeObject( statements.ToList ()));
			 
			}
		}

		public ActionResult SearchStatement(string medicinOne, string medicinTwo)
		{
			using (DataContext context = new DataContext())
			{
				int maxVotings = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxVotings"]);

				return Content ( JsonConvert.SerializeObject(
					context.Statements.Include("Person").Where (x => x.MedicinOne == medicinOne && x.MedicinTwo == medicinTwo &&
						context.Recommendations.Count (y => y.StatementId == x.Id) >= maxVotings &&
						context.Recommendations.Count (y => y.StatementId == x.Id && y.IsRecommended) >= maxVotings / 2).ToList())); 
			} 
		}

		public ActionResult GetRecommendations(int statement)
		{
			using (DataContext context = new DataContext())
			{
				var recommendations = context.Recommendations.Include("Person").Where (x => x.StatementId == statement).ToList ();
				recommendations.ForEach(x => x.Person.TrustValue = CalculateBayesianModelTrust(x.Person));
				return Content ( JsonConvert.SerializeObject(recommendations)); 
			} 
		}

		[HttpPost]
		public ActionResult CreateNewStatement(TrustLessModelLib.Statement statement)
		{ 
			using (DataContext context = new DataContext())
			{
				if (!PersonController.ValidateLoginSession(context,statement.Person.LoginSession.Token))
					return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);  
				
				var match =
					context.Statements.FirstOrDefault(x => x.MedicinOne == statement.MedicinOne && x.MedicinTwo == statement.MedicinTwo);

				//Does a statement already exists with the same two medicins.
				if (match != null) // && IsStatementValid(context,match)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);  

				//Security: Don't allow user to specify the person who created the statement. Set it to the current user logged in.
				statement.Person =
					context.Persons.FirstOrDefault(x => x.Username == statement.Person.Username);

				//If the person does not exist, abort.
				if (statement.Person == null)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
				
				//Rule: refuse a user with zero trust from creating statements.
				if (CalculateBayesianModelTrust(statement.Person) == 0)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				context.Statements.Add(statement);

				//Add recommendation from user
				var recommendation = new TrustLessModelLib.Recommendation(){ Person = statement.Person, Statement = statement, IsRecommended = true, Transaction = null, Description = statement.Description, CreationDate = DateTime.Now};

				//This will fail if a racecondition was to add two recommendations (given that our key assignment of the table ensures only one of the same recommendation exist).
				context.Recommendations.Add (recommendation);
				context.SaveChanges ();

				BlockChain.MakeRecommendation (context, recommendation);
				
				return new HttpStatusCodeResult((int)HttpStatusCode.Created);
			}
		}

		[HttpPost]
		public ActionResult Recommend(int statement, string token, bool trust)
		{
			var possibleStatements = GetStatements (token);
			if(possibleStatements.GetType() != typeof(ContentResult))
				return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
			
			if (!JsonConvert.DeserializeObject<List<Statement>>(((ContentResult)possibleStatements).Content, new JsonSerializerSettings{
					MissingMemberHandling = MissingMemberHandling.Ignore
			}).Any(x => x.Id == statement))
				return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);  
				

		    Stream req = Request.InputStream;
			req.Seek (0, System.IO.SeekOrigin.Begin);
			string description = new StreamReader(req).ReadToEnd();
			using (DataContext context = new DataContext())
			{
				if (!PersonController.ValidateLoginSession(context,token))
					return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);  

				Person person = context.Persons.FirstOrDefault (x => x.LoginSessionToken != null && x.LoginSessionToken == token);

				var statementObject =
					context.Statements.FirstOrDefault(x => x.Id == statement);

				if (statementObject == null)
					return new HttpStatusCodeResult((int)HttpStatusCode.NotFound);
				
				var recommendation =
					context.Recommendations.FirstOrDefault(x => x.StatementId== statement && x.PersonUsername == person.Username);

				if (recommendation != null)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				if (IsStatementRecommendationsComplete (context, statementObject))
					return new HttpStatusCodeResult((int)HttpStatusCode.Forbidden);

				
				recommendation = new TrustLessModelLib.Recommendation(){ Person = person, Statement = statementObject, IsRecommended = trust, Transaction = null, Description = description, CreationDate = DateTime.Now};

				//This will fail if a racecondition was to add two recommendations (given that our key assignment of the table ensures only one of the same recommendation exist).
				context.Recommendations.Add (recommendation);
				context.SaveChanges ();

				BlockChain.MakeRecommendation (context, recommendation);

				if (IsStatementRecommendationsComplete (context, statementObject))
					IssueTrustForStatement (context, statementObject);

				return new HttpStatusCodeResult((int)HttpStatusCode.Created);
			}
		}

		private bool IsStatementRecommendationsComplete(DataContext context,TrustLessModelLib.Statement statement)
		{
			var recommendationAmount =
				context.Recommendations.Count(x => x.StatementId== statement.Id && x.Transaction != null);

			int maxVotings = Convert.ToInt32(WebConfigurationManager.AppSettings["MaxVotings"]);
			return recommendationAmount >= maxVotings;
		}

		private void IssueTrustForStatement(DataContext context,TrustLessModelLib.Statement statement)
		{
			var recommendations =
				context.Recommendations.Include("Person").Include("Transaction").Where(x => x.StatementId== statement.Id && x.Transaction != null).ToList();

			int sAmount = recommendations.Count (x => x.IsRecommended) +1;
			int fAmount = recommendations.Count (x => !x.IsRecommended);

			foreach (Recommendation recommendation in recommendations) {
				if (sAmount >= fAmount) {
					if (recommendation.IsRecommended) {
						BlockChain.IssueS (recommendation);
					} else {
						BlockChain.IssueF (recommendation);
					}
				} else {
					if (recommendation.IsRecommended) {
						BlockChain.IssueF (recommendation);
					} else {
						BlockChain.IssueS (recommendation);
					}
				}
			}
		}

		private double CalculateBayesianModelTrust(Person person)
		{
			double initialTrust = Convert.ToDouble(WebConfigurationManager.AppSettings["InitialTrust"]);
			double trustProblabilityOfGoodPerson = Convert.ToDouble(WebConfigurationManager.AppSettings["TrustProblabilityOfGoodPerson"]);
			double nonTrustProblabilityOfBadPerson = Convert.ToDouble(WebConfigurationManager.AppSettings["ŃonTrustProblabilityOfBadPerson"]);

			double[] sfFloats = GetSVFromBlockChain(person);

			double trustValue =  (initialTrust*Math.Pow(trustProblabilityOfGoodPerson, sfFloats[0])*
			        Math.Pow(1 - trustProblabilityOfGoodPerson, sfFloats[1]))/
				(initialTrust*Math.Pow(trustProblabilityOfGoodPerson, sfFloats[0])*
				 Math.Pow(1 - trustProblabilityOfGoodPerson, sfFloats[1]) +
				 (1 - initialTrust)*Math.Pow(nonTrustProblabilityOfBadPerson, sfFloats[0])*
				 Math.Pow(1 - nonTrustProblabilityOfBadPerson, sfFloats[1]));
			return trustValue;
		}

		private double[] GetSVFromBlockChain(Person person)
		{
			var balances = BlockChain.GetPersonBalance (person);

			var sBalance = balances.Where (x => x.name.StartsWith("S"));
			var fBalance = balances.Where (x => x.name.StartsWith("F"));
			var sAmount = sBalance != null ? sBalance.Sum(x=>x.qty) : 0;
			var fAmount = fBalance != null ? fBalance.Sum(x=>x.qty) : 0;

			return new double[] {sAmount, fAmount};
		}



    }
}
