using Microsoft.AspNetCore.Mvc;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {


        private readonly HalloDocContext _context;

        public AccountController(HalloDocContext context)
        {
             
             _context = context;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(UserCred um)
        {


            if(ModelState.IsValid)
            {
                var user  =  await _context.Aspnetusers.FirstOrDefaultAsync(u=>um.Email == u.Email && um.Password == u.Passwordhash);


                if(user == null)
                {

                    TempData["Error"] = "Invalid Attributes";

                    return View();
                     
                }


                return RedirectToAction("FormByPatient");

            }



            return View("Login", um);

              
             

        }


    }
}
