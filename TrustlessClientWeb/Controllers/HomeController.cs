using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustlessClientWeb.Models;

namespace TrustlessClientWeb.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        static Repository Repo = new Repository();

        public ActionResult Index()
        {
            ViewBag.S = Repo.GetS();
            ViewBag.F = Repo.GetF();

            return View("Index");
        }

        public ActionResult NewStatementAction(string drug1, string drug2, string description)
        {
            Repo.SendNewStatment(drug1, drug2, description);

            return Index();
        }
    }
}