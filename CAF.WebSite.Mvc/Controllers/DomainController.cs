using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAF.WebSite.Mvc.Controllers
{
    public class DomainController : Controller
    {
        // GET: Domain
        public ActionResult Index()
        {
            var cityName = RouteData.Values["CityNameUrl"];
            return View(cityName);
        }
    }
}