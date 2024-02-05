using dotnetProc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace dotnetProc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ForgotPass()
        {
            return View();
        }


        public IActionResult PatientRequest()
        {

            return View();
        }


        public IActionResult FormByPatient()
        {

            return View();
        }

        public IActionResult FormByFamily()
        {

            return View();
        }

        public IActionResult FormByConciearge()
        {

            return View();
        }

        public IActionResult FormByBusinessPartner()
        {

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