using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;



namespace dotnetProc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HalloDocContext _context;


        public HomeController(HalloDocContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User um)
        {
            //if (ModelState.IsValid)
            //{
           

            //    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == um.UserName && u.Password == um.Password);
                   

            //        if(user == null)
            //    {
            //        TempData["Error"] = "Invalid Attributes";

            //        return View();
            //    }

            //    return RedirectToAction("FormByPatient");
            //}

            return View("Login", um);

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