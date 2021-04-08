﻿using MechanicChecker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
<<<<<<< Updated upstream
=======
            SellerAddressContext context = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.SellerAddressContext)) as SellerAddressContext;
            ExternalAPIsContext eContext = HttpContext.RequestServices.GetService(typeof(MechanicChecker.Models.ExternalAPIsContext)) as ExternalAPIsContext;

            //var data = eContext.activateAPI("DeveloperAPI Ebay");
            //Debug.WriteLine("API " + data);

>>>>>>> Stashed changes
            return View();
        }

        public IActionResult Results()
        {
            return View("SearchResultsList");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description pages.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
