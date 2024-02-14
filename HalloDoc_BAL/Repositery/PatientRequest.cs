using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
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


            Aspnetuser? aspNetUser =  _context.Aspnetusers.FirstOrDefault(u => u.Email == pr.Email);

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

        public Object CheckEmail(string email)
        {

            var exist = _context.Aspnetusers.FirstOrDefault(u => u.Email == email);

            return exist;
        }


        public Aspnetuser GetAspNetUserByEmail(string email)
        {
             
            Aspnetuser user = _context.Aspnetusers.FirstOrDefault(a => a.Email == email);

            return user;
        }


        public int GetUserIdByEmail(string email)
        {
              if(_context.Users.Any(a => a.Email == email))
            {
                var hh = _context.Users.FirstOrDefault(u => u.Email == email).Userid;
                return hh;
            }

              return 0;
        }



        public Aspnetuser AddAspNetUser(PatientReq pr,string password)
        {

            Aspnetuser aspNetUser1 = new Aspnetuser
            {
                Id = Guid.NewGuid().ToString(),
                Username = pr.FirstName + "_" + pr.LastName,
                Email = pr.Email,
                Passwordhash = password.GetHashCode().ToString(),
                Phonenumber = pr.Phonenumber,
                Createddate = DateTime.Now
            };


            _context.Aspnetusers.Add(aspNetUser1);

            _context.SaveChanges();

            return aspNetUser1;
        }


        public int AddUser(string AspNetUserId,PatientReq patientReq,AddressModel patinetLocation)
        {
            User user = new User
            {
                Firstname = patientReq.FirstName,
                Lastname = patientReq.LastName,
                Email = patientReq.Email,
                Mobile = patientReq.Phonenumber,
                Zipcode = patinetLocation.ZipCode,
                State = patinetLocation.State,
                City = patinetLocation.City,
                Street = patinetLocation.Street,
                Intdate = patientReq.BirthDate.Day,
                Intyear = patientReq.BirthDate.Year,
                Strmonth = ((Months)patientReq.BirthDate.Month).ToString(),
                Createddate = DateTime.Now,
                Aspnetuserid = AspNetUserId,
                Createdby = AspNetUserId,
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Userid;
        }


        public Request AddRequest(CmnInformation cm, int userId, string requestType)
        {


            Request request = new Request
            {

                Requesttypeid = _context.Requesttypes.FirstOrDefault(u => u.Name == requestType).Requesttypeid,
                Firstname = cm.FirstName,
                Lastname = cm.LastName,
                Phonenumber = cm.PhoneNumber,
                Createddate = DateTime.Now,
                Status = 1,
                Userid = userId,
            };

            _context.Requests.Add(request);

            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return new Request();
            }



            return request;


        }


        public bool AddRequestClient(PatientReq pr,int requestId,AddressModel patientAddress )
        {

            Requestclient requestclient = new Requestclient
            {
                Requestid = requestId,
                Firstname = pr.FirstName,
                Lastname = pr.LastName,
                Phonenumber = pr.Phonenumber,
                Email = pr.Email,
                Intdate = pr.BirthDate.Day,
                Intyear = pr.BirthDate.Year,
                Strmonth = ((Months)pr.BirthDate.Month).ToString(),
                Street = patientAddress.Street,
                City = patientAddress?.City,
                State = patientAddress?.State,
                Zipcode = patientAddress.ZipCode,
            };

            _context.Requestclients.Add(requestclient);

            try
            {
                _context.SaveChanges();

            }
            catch(Exception ex) { return false; }


            return true;
        }


        public bool AddBusinessRequest(int businessId,int requestId)
        {
            Requestbusiness requestbusiness = new Requestbusiness
            {
                Businessid = businessId,
                Requestid = requestId,
            };

            _context.Requestbusinesses.Add(requestbusiness);
            _context.SaveChanges();
            return true;
        }

        public Business AddBusiness(CmnInformation cm,AddressModel partnerLocation)
        {
            Business business = new Business
            {
                Name = cm.FirstName,
                City = partnerLocation.City,
                Zipcode = partnerLocation.ZipCode,
                Phonenumber = cm.PhoneNumber,
                Createddate = DateTime.Now,
            };

            _context.Businesses.Add(business);
            _context.SaveChanges();
            return business;


        }

        public bool AddConciergeRequest(int concieargeId,int requestId) 
        {

            Requestconcierge requestconcierge = new Requestconcierge
            {
                Conciergeid = concieargeId,
                Requestid = requestId,
            };

            _context.Requestconcierges.Add(requestconcierge);
            _context.SaveChanges();
            return true;
        
        }

        public Concierge Addconciearge(AddressModel partnerLocation,string name) 
        {

            Concierge concierge = new Concierge
            {
                Conciergename = name,
                Street = partnerLocation.Street,
                City = partnerLocation.City,
                State = partnerLocation.State,
                Zipcode = partnerLocation.ZipCode,
                Createddate = DateTime.Now,

            };

            _context.Concierges.Add(concierge);
            _context.SaveChanges();

            return concierge;
        }


        public User GetUserDataById(int userId)
        {
              User user = _context.Users.FirstOrDefault(u=>u.Userid == userId);

            return user;
        }


        public bool UploadFile(IFormFile file,int requestid)
        {

            string path = "";
            bool iscopied = false;

            Requestwisefile requestwisefile = new Requestwisefile
            {
                Requestid = requestid,
                Filename = file.FileName,
                Doctype = 1,
                Createddate = DateTime.Now,
            };


            try
            {
                if (file.Length > 0)
                {
                    string filename = file.FileName;
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\Upload\\";
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    _context.Requestwisefiles.Add(requestwisefile);
                    _context.SaveChanges();
                    iscopied = true;
                }
                else
                {
                    iscopied = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return iscopied;



        }
    }
}
