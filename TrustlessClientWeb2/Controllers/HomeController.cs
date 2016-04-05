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

        public async Task<ActionResult> NewStatementAction(string medicinOne, string medicinTwo, string description)
        {
            await Repo.SendNewStatment(medicinOne, medicinTwo, description);

            return await Index();
        }

        public async Task<ActionResult> Recommend(Statement item, bool recommend)
        {
            Person testP = new Person { Id = 80, PublicKey = "1", ReservedServerWalletKey = "1", Username = "testPerson" };
            await Repo.ReplyDebatableStatement(item, testP, recommend);

            return await Index();
        }
    }
}