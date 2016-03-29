using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustlessClientWeb2.Models;

namespace TrustlessClientWeb2.Controllers
{
    public class HomeController : Controller
    {
        static Repository Repo = new Repository();

        public ActionResult Index()
        {
            ViewBag.S = Repo.GetS();
            ViewBag.F = Repo.GetF();

            return View("Index");
        }

        public ActionResult NewStatementAction(string medicinOne, string medicinTwo, string description)
        {
            Repo.SendNewStatment(medicinOne, medicinTwo, description);

            return Index();
        }
    }
}