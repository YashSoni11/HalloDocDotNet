using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class PatientRequest : IPatientReq
    {

        private readonly HalloDocContext _context;

        public PatientRequest(HalloDocContext context)
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



        public void AddPatientReq(PatientReq pr)
        {


            Aspnetuser aspNetUser =  _context.Aspnetusers.FirstOrDefault(u => u.Email == pr.Email);

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


        }



    }
}
