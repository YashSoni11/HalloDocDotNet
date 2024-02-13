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

                Aspnetuser aspuser =  _account.ValidateLogin(um);



                if(aspuser == null)
                {

                    TempData["Error"] = "Invalid Attributes";

                    return View();
                     
                }
                else
                {

                    User user = _account.GetUserByAspNetId(aspuser.Id);

                    HttpContext.Session.SetInt32("LoginId", user.Userid);

                    HttpContext.Session.SetString("UserName", user.Firstname);

                   return RedirectToAction("DashBoard","Account");
                }



            }



            return View("Login", um);

              
             

        }


        [HttpGet]
        public IActionResult DashBoard()
        {


            int LoginId = (int)HttpContext.Session.GetInt32("LoginId");

            var userRequests = _account.GetUserRequests(LoginId);

            TempData["UserName"] = HttpContext.Session.GetString("UserName");

            return View(userRequests);
        }

    }
}
