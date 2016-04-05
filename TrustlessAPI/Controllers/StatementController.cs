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

namespace TrustlessAPI.Controllers
{
    public class StatementController : Controller
    {
        public ActionResult Index()
        {
            return View ();
        }

		public ActionResult GetStatements()
		{
			using (DataContext context = new DataContext())
			{
				int maxVotings = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVotings"]);
				return Content ( JsonConvert.SerializeObject(context.Statements.Where(x=> context.Recommendations.Count(y => y.StatementId== x.Id) < maxVotings ).ToList())); 
			} 
		}

		public ActionResult SearchStatement(string medicinOne, string medicinTwo)
		{
			using (DataContext context = new DataContext())
			{
				int maxVotings = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVotings"]);
				return Content ( JsonConvert.SerializeObject(context.Statements.Where (x => context.Recommendations.Count (y => y.StatementId == x.Id) >= maxVotings && context.Recommendations.Count (y => y.StatementId == x.Id && y.IsRecommended) >= maxVotings / 2))); 
			} 
		}

		public ActionResult GetRecommendations(int statement)
		{
			using (DataContext context = new DataContext())
			{
				return Content ( JsonConvert.SerializeObject(context.Recommendations.Where (x => x.StatementId == statement).ToList())); 
			} 
		}

		[HttpPost]
		public ActionResult CreateNewStatement(TrustLessModelLib.Statement statement)
		{ 
			using (DataContext context = new DataContext())
			{
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
				context.SaveChanges();
				
				return new HttpStatusCodeResult((int)HttpStatusCode.Created);
			}
		}

		[HttpPost]
		public ActionResult Recommend(int statement, string username, bool trust)
		{
			using (DataContext context = new DataContext())
			{
				var statementObject =
					context.Statements.FirstOrDefault(x => x.Id == statement);

				if (statementObject == null)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
				
				var recommendation =
					context.Recommendations.FirstOrDefault(x => x.StatementId== statement && x.PersonUsername == username);

				if (recommendation != null)
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				if (IsStatementRecommendationsComplete (context, statementObject))
					return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);

				var person =
					context.Persons.FirstOrDefault(x => x.Username == username);
				
				recommendation = new TrustLessModelLib.Recommendation(){ Person = person, Statement = statementObject, IsRecommended = trust, Transaction = null};

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

			int maxVotings = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxVotings"]);
			return recommendationAmount >= maxVotings;
		}

		private void IssueTrustForStatement(DataContext context,TrustLessModelLib.Statement statement)
		{
			var recommendations =
				context.Recommendations.Where(x => x.StatementId== statement.Id && x.Transaction != null).ToList();

			int sAmount = recommendations.Count (x => x.IsRecommended);
			int fAmount = recommendations.Count (x => !x.IsRecommended);

			foreach (Recommendation recommendation in recommendations) {
				if (sAmount > fAmount) {
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
			double initialTrust = 0.2;
			double trustProblabilityOfGoodPerson = 0.8;
			double nonTrustProblabilityOfBadPerson = 1.5;

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

			var sBalance = balances.FirstOrDefault (x => x.name == "S");
			var fBalance = balances.FirstOrDefault (x => x.name == "F");
			var sAmount = sBalance != null ? sBalance.qty : 0;
			var fAmount = fBalance != null ? sBalance.qty : 0;

			return new double[] {sAmount, fAmount};
		}



    }
}
