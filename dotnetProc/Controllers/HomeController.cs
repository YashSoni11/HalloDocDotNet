using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
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

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }


        public enum Months
        {
            January = 1,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public IActionResult Index()
        {
            return View();
        }

     
        public  IActionResult ckeckEmailAvailibility(string email)
        {

            if(email != null)
            {
                Aspnetuser exist =  _context.Aspnetusers.FirstOrDefault(u => u.Email == email);

                if(exist != null)
                {
                    
                    return Json(new { Error = "Account Already Exist!" ,code = 401});
                }

            }


            return Json(new { code = 402 });

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






        [HttpPost]
        [ValidateAntiForgeryToken]



        public async Task<IActionResult> FormByPatient(PatientReq pr)
        {


            Aspnetuser aspNetUser = await _context.Aspnetusers.FirstOrDefaultAsync(u => u.Email == pr.Email);

            if (aspNetUser == null)
            {

                Aspnetuser aspNetUser1 = new Aspnetuser
                {
                    Id = RandomString(5),
                    Username = pr.FirstName + "_" + pr.LastName,
                    Email = pr.Email,
                    Passwordhash = pr.FirstName,
                    Phonenumber = pr.Phonenumber,
                    Createddate = DateTime.Now
                };

                _context.Aspnetusers.Add(aspNetUser1);
                aspNetUser = aspNetUser1;



            }


            User user = new User
            {
                Firstname = pr.FirstName,
                Lastname = pr.LastName,
                Email = pr.Email,
                Mobile = pr.Phonenumber,
                Zipcode = pr.Location.ZipCode,
                State = pr.Location.State,
                City = pr.Location.City,
                Street = pr.Location.Street,
                Intdate = pr.BirthDate.Day,
                Intyear = pr.BirthDate.Year,
                Strmonth = ((Months)pr.BirthDate.Month).ToString(),
                Createddate = DateTime.Now,
                CreatedbyNavigation = aspNetUser,
                Aspnetuser = aspNetUser
               
            };

            _context.Users.Add(user);

            Request request = new Request
            {
                Requesttypeid = 3,
                Firstname = pr.FirstName,
                Lastname = pr.LastName,
                Phonenumber = pr.Phonenumber,
                Email = pr.Email,
                Createddate = DateTime.Now,
                Status = 1,
                User = user
            };


            _context.Requests.Add(request);
            _context.SaveChanges();


            return RedirectToAction("Login");

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