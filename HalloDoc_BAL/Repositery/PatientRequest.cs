﻿using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;

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



        public string GetHashedPassword(string password)
        {

            SHA256 hash = SHA256.Create();

            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i]);
            }
            return builder.ToString();

        }

        public object GetInfoValidation(string email, string phone, int region)
        {
            bool isemailexist =  IsEmailExistance(email);
            bool isemailblocked =  IsEmailBlocked(email);
            bool IsPhoneBlock =  IsPhoneBlocked(phone);
            bool IsregionAvailable = IsRegionAvailable(region);

            var result = new
            {
                isemailexist,
                isemailblocked,
                IsPhoneBlock,
                IsregionAvailable

            };

            return result;
        }

        public Requestclient GetRequestByRequestId(int requestId)
        {
            return _context.Requestclients.FirstOrDefault(q => q.Requestid == requestId);
        }



        public int GetCurrentRequestsCount()
        {
            int currentDate = DateTime.Now.Day;

            int count = _context.Requests.Where(r=>r.Createddate.Day == currentDate).Count();

            return count;
        }

        public string GetConfirmationNumber(string Firstname,string Lastname,string State)
        {

            string Region = State.Substring(0, 2).ToUpperInvariant();

            string NameAbbr = Lastname.Substring(0, 2).ToUpperInvariant() + Firstname.Substring(0, 2).ToUpperInvariant();

            string date = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

            int requestsCount = GetCurrentRequestsCount();

            string newRequestCount = requestsCount.ToString();

            string ConfirmationNumber = Region+date+NameAbbr+newRequestCount;

            return ConfirmationNumber;
        }

        public bool IsEmailExistance(string email)
        {
             bool response  = _context.Aspnetusers.Any(x => x.Email == email);

            return response;
        }

        public bool IsEmailBlocked(string email)
        {
            return _context.Blockrequests.Any(x => x.Email == email && x.Isactive == new BitArray(1,false));   
        }

        public bool IsPhoneBlocked(string phone)
        {
            return _context.Blockrequests.Any(x=>x.Phonenumber == phone);
        }


        public void AddPatientReq(PatientReq pr)
        {


            Aspnetuser? aspNetUser = _context.Aspnetusers.FirstOrDefault(u => u.Email == pr.Email);

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
                State = _context.Regions.Where(q=>q.Regionid == pr.Location.State).Select(q=>q.Name).FirstOrDefault(),
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



        public Aspnetuser AddAspNetUser(PatientReq pr, string password)
        {

            try
            {
                Aspnetuser aspNetUser1 = new Aspnetuser
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = pr.FirstName + "_" + pr.LastName,
                    Email = pr.Email,
                    Passwordhash = GetHashedPassword(password),
                    Phonenumber = pr.Phonenumber,
                    Createddate = DateTime.Now
                };


                _context.Aspnetusers.Add(aspNetUser1);



                Aspnetuserrole aspnetuserrole = new Aspnetuserrole();

                aspnetuserrole.Userid = aspNetUser1.Id;
                aspnetuserrole.Roleid = "Patient";

                _context.Aspnetuserroles.Add(aspnetuserrole);
                _context.SaveChanges();

                return aspNetUser1;
            }
            catch(Exception ex) 
            {
                return new Aspnetuser();
            }
        }


        public int AddUser(string AspNetUserId,PatientReq patientReq,AddressModel patinetLocation)
        {
            try
            {
            User user = new User
            {
                Firstname = patientReq.FirstName,
                Lastname = patientReq.LastName,
                Email = patientReq.Email,
                Mobile = patientReq.Phonenumber,
                Zipcode = patinetLocation.ZipCode,
                State = _context.Regions.Where(q => q.Regionid == patinetLocation.State).Select(q => q.Name).FirstOrDefault(),
                City = patinetLocation.City,
                Street = patinetLocation.Street,
                Intdate = patientReq.BirthDate.Day,
                Intyear = patientReq.BirthDate.Year,
                Strmonth = ((Months)patientReq.BirthDate.Month).ToString(),
                Createddate = DateTime.Now,
                Aspnetuserid = AspNetUserId,
                Createdby = AspNetUserId,
                Isdeleted = new BitArray(1, false),
                Regionid = patinetLocation.State,
                Status=1,

            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Userid;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public Request AddRequest(CmnInformation cm, int userId, string requestType,int state,string Role)
        {

             string? statName = _context.Regions.Where(q=>q.Regionid == state).Select(q=>q.Name).FirstOrDefault();

            string confirmatinumber = GetConfirmationNumber(cm.FirstName, cm.LastName, statName);


            Request request = new Request
            {

                Requesttypeid = _context.Requesttypes.FirstOrDefault(u => u.Name == requestType).Requesttypeid,
                Firstname = cm.FirstName,
                Lastname = cm.LastName,
                Phonenumber = cm.PhoneNumber,
                Createddate = DateTime.Now,
                Confirmationnumber = confirmatinumber,
                Email = cm.Email,
                Createduserid = userId,
                Userid = userId == 0?null:userId,
                Isdeleted = new BitArray(1, false),
            };
            
            if(Role == "Physician" || Role == "Admin")
            {
                request.Userid = null;
            }

            if(Role == "Physician")
            {
                request.Status = 9;
                request.Physicianid = userId;
            }
            else
            {
                request.Status = 1;
            }

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
                State = _context.Regions.Where(q=>q.Regionid == patientAddress.State).Select(q=>q.Name).FirstOrDefault(),
                Zipcode = patientAddress.ZipCode,
                Notes = pr.Symptoms,
                Regionid = patientAddress.State,
                Address = patientAddress.Street+","+patientAddress.City+","+patientAddress.State+","+patientAddress.ZipCode,
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
            try
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
            }catch(Exception ex)
            {
                return new Business();
            }


        }

        public bool AddConciergeRequest(int concieargeId,int requestId) 
        {
            try
            {

            Requestconcierge requestconcierge = new Requestconcierge
            {
                Conciergeid = concieargeId,
                Requestid = requestId,
            };

            _context.Requestconcierges.Add(requestconcierge);
            _context.SaveChanges();
            return true;

            }catch(Exception ex)
            {
                return false;
            }
        
        }

        public Concierge Addconciearge(AddressModel partnerLocation,string name) 
        {
            try
            {
            Concierge concierge = new Concierge
            {
                Conciergename = name,
                Street = partnerLocation.Street,
                City = partnerLocation.City,
                State = _context.Regions.Where(q=>q.Regionid ==  partnerLocation.State).Select(q=>q.Name).FirstOrDefault(),
                Zipcode = partnerLocation.ZipCode,
                Createddate = DateTime.Now,

            };

            _context.Concierges.Add(concierge);
            _context.SaveChanges();

            return concierge;

            }catch(Exception ex)
            {
                return new Concierge();
            }

        }

        public void AddConcieargeData(ConcieargeModel concieargeModel,int user)
        {
            try
            {

            Request patientRequest = AddRequest(concieargeModel.concieargeInformation, user, "Concierge",concieargeModel.PatinentInfo.Location.State,"Patient");

            bool response = AddRequestClient(concieargeModel.PatinentInfo, patientRequest.Requestid, concieargeModel.concieargeLocation);

            Concierge concierge = Addconciearge(concieargeModel.concieargeLocation, concieargeModel.concieargeInformation.FirstName);

            bool response2 = AddConciergeRequest(concierge.Conciergeid, patientRequest.Requestid);

            return;

            }catch(Exception ex)
            {
                return;
            }

        }


        public User GetUserDataById(int userId)
        {
              User user = _context.Users.FirstOrDefault(u=>u.Userid == userId);

            return user;
        }

        public Requestclient GetRequestClientByEmail(string email)
        {
            Requestclient requestclient = _context.Requestclients.FirstOrDefault(q=>q.Email == email);

            return requestclient;
        }

        public Request UpdateRequestByRequestId(int requestId,int userId)
        {
            try
            {

            Request request = _context.Requests.FirstOrDefault(q=>q.Requestid == requestId);

            request.Userid = userId;
            request.Createduserid = userId;

            _context.Requests.Update(request);

            _context.SaveChanges();

            return request; 
            }
            catch (Exception ex)
            {
                return new Request();
            }

        }

        public bool  IsRegionAvailable(int region)
        {
            return _context.Regions.Any(q=>q.Regionid == region);
        }


        public bool UploadFile(IFormFile file,int requestid)
        {

            if(file == null) return false;
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
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload\\";
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
