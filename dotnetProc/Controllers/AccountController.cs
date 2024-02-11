using Microsoft.AspNetCore.Mvc;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using Microsoft.EntityFrameworkCore;
using HalloDoc_BAL.Interface;

namespace dotnetProc.Controllers
{
    public class AccountController : Controller
    {


        
        private readonly IAccount _account;
       

        public AccountController(IAccount account)
        {
             
           
            _account = account;
           
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public  IActionResult Login(UserCred um)
        {


            if(ModelState.IsValid)
            {
                //var user  =  await _context.Aspnetusers.FirstOrDefaultAsync(u=>um.Email == u.Email && um.Password == u.Passwordhash);

                var user =  _account.ValidateLogin(um);



                if(user == null)
                {

                    TempData["Error"] = "Invalid Attributes";

                    return View();
                     
                }


                return RedirectToAction("DashBoard","Account");

            }



            return View("Login", um);

              
             

        }



        public IActionResult DashBoard()
        {
            return View();
        }

    }
}
