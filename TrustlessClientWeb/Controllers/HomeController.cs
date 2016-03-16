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
            return View();
        }
    }
}