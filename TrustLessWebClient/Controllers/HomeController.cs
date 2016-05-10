using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrustlessClientWeb.Models;
using TrustLessModelLib;

namespace TrustlessClientWeb.Controllers
{
    public class HomeController : Controller
    {
        static Repository Repo = new Repository();

		public ActionResult Index()
        {
            ViewBag.S = Repo.GetS();
            ViewBag.F = Repo.GetF();

			var listOfDebatebelStatements = Repo.GetDebatableStatements ();

			return View("Index",listOfDebatebelStatements);
            //return View("Index", new List<Statement>().AsEnumerable());
        }

		public ActionResult Login(string username, string password)
		{ 
			Repo.LoginAttempt (username,password);
			return Index();
			//return View("Index", new List<Statement>().AsEnumerable());
		}

        public ActionResult RecommendationList()
        {
            return View("RecommendationList", new Tuple<IEnumerable<TrustLessModelLib.Statement>, Dictionary<int, List<TrustLessModelLib.Recommendation>>>(Repo._Statements.AsEnumerable<Statement>(), Repo._StatementsRecommendations));
        }

		public ActionResult GetRecommendation(string medicinOne, string medicinTwo)
        {
            Repo.GetRecommendation(medicinOne, medicinTwo);
            return RecommendationList();
        }

		public ActionResult NewStatementAction(string medicinOne, string medicinTwo, string description)
        {
			if ( Session ["token"]== null)
				return View ("Login");
			
            Repo.SendNewStatment(medicinOne, medicinTwo, description);

            return Index();
        }

		public ActionResult Recommend(int id, bool recommend, string description)
		{ 
			if ( Session ["token"]== null)
				return View ("Login");
			
            if (description == null)
                description = "";
            Repo.ReplyDebatableStatement(id, recommend, description);

            return Index();
        }
    }
}