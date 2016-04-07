using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrustlessClientWeb2.Models;
using TrustLessModelLib;

namespace TrustlessClientWeb2.Controllers
{
    public class HomeController : Controller
    {
        static Repository Repo = new Repository();

        public async Task<ActionResult> Index()
        {
            ViewBag.S = Repo.GetS();
            ViewBag.F = Repo.GetF();

            return View("Index", await Repo.GetDebatableStatements());
            //return View("Index", new List<Statement>().AsEnumerable());
        }

        public ActionResult RecommendationList()
        {
            return View("RecommendationList", new Tuple<IEnumerable<TrustLessModelLib.Statement>, Dictionary<int, List<TrustLessModelLib.Recommendation>>>(Repo._Statements.AsEnumerable<Statement>(), Repo._StatementsRecommendations));
        }

        public async Task<ActionResult> GetRecommendation(string medicinOne, string medicinTwo)
        {
            await Repo.GetRecommendation(medicinOne, medicinTwo);
            return RecommendationList();
        }

        public async Task<ActionResult> NewStatementAction(string medicinOne, string medicinTwo, string description)
        {
            await Repo.SendNewStatment(medicinOne, medicinTwo, description);

            return await Index();
        }

        public async Task<ActionResult> Recommend(int id, bool recommend, string description)
        {
            if (description == null)
                description = "";
            await Repo.ReplyDebatableStatement(id, recommend, description);

            return await Index();
        }
    }
}