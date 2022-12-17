using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FASTFOOD.Controllers
{
    public class HomeController : Controller
    {
        fastfoodEntities db = new fastfoodEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return View();

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}