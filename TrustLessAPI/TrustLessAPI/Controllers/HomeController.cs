using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrustLessAPI.Models;

namespace TrustLessAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            using (DataContext context = new DataContext())
            {
                var a = context.Persons.FirstOrDefault();
                
            } 

            return View();
        }
    }
}
