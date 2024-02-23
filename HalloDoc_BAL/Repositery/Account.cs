﻿using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.Repositery
{
    public class Account : IAccount
    {
        private readonly HalloDocContext _context;

      public  Account(HalloDocContext context)
        {
             _context = context;
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

        // public var GiveResetPasswordLinkObject(string resetid)
        //{
            

        //    string subject = "Password Reset";

        //    string resetLink = "https://localhost:7008/ResetPassword/" + resetid;

        //    string body = "Please click on <a asp-route-id='" + resetid + "' href='" + resetLink + "'+>ResetPassword</a> to reset your password";

        //    var resetLinkObj = new
        //    {
        //        subject,
        //        resetLink,
        //        body
        //    };

        //    return resetLinkObj;


        //}

        public string GetHashedPassword(string password)
        {
             
            SHA256 hash = SHA256.Create();

            byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();

            for(int i = 0;i< bytes.Length; i++)
            {
                builder.Append(bytes[i]);
            }
            return builder.ToString();

        }


        public Aspnetuser ValidateLogin(UserCred um)
        {

            string pass = GetHashedPassword(um.Password);

                Aspnetuser user  =   _context.Aspnetusers.FirstOrDefault(u=>um.Email == u.Email && pass == u.Passwordhash);

            return user;
        }

        public Aspnetuser GetAspnetuserByEmail(string email)
        {

            Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u=>u.Email == email);

            return aspnetuser;

        }
        public User GetUserByAspNetId(string id)
        {
             
            User user  = _context.Users.FirstOrDefault(u=>u.Aspnetuserid == id);

            return user;

        }

        public User GetUserByUserId(int  userId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Userid == userId);
 

      
            return user;
        }

        public Aspnetuser UpdateAspnetuserPassByEmail(string Email,string Newpassword)
        {
             Aspnetuser aspnetuser= _context.Aspnetusers.FirstOrDefault(u=>u.Email == Email);

            if(aspnetuser!=null)
            {
                 aspnetuser.Passwordhash = GetHashedPassword(Newpassword);
                 aspnetuser.Modifieddate = DateTime.Now;
                  
                 _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();

            }

            return aspnetuser;
        }

        public List<DashBoardRequests> GetUserRequests(int userid)
        {

            List<DashBoardRequests> requests = _context.Requests.Where(u => u.Userid == userid).Select(r => new DashBoardRequests
            {
                requestid = r.Requestid,
                createDate = r.Createddate,
                Status = r.Status,
                totalDocuments = _context.Requestwisefiles.Where(u => u.Requestid == r.Requestid).Count(),
                providerName = _context.Physicians.FirstOrDefault(u => u.Physicianid == r.Physicianid).Firstname,
            }).ToList();

            return requests;
        }


        public  UserProfile UpdateUserByUserId(UserInformation um,int UserId)
        {

        

            User curUser = _context.Users.FirstOrDefault(u => u.Userid == UserId);

            if(curUser != null)
            {


                curUser.Firstname = um.User.Firstname;
                curUser.Lastname = um.User.Lastname;
                curUser.Email = um.User.Email;
                curUser.Mobile = um.User.Phonnumber;
                curUser.Intdate = um.User.Birthdate.Day;
                curUser.Intyear = um.User.Birthdate.Year;
                curUser.Strmonth = ((Months)um.User.Birthdate.Month).ToString();
                curUser.Street = um.User.Address.Street;
                curUser.City = um.User.Address.City;
                curUser.State = um.User.Address.State;
                curUser.Zipcode = um.User.Address.ZipCode;

                 _context.Users.Update(curUser);
                _context.SaveChanges();

                AddressModel newAddress = new AddressModel
                {
                    State = um.User.Address.State,
                    City = um.User.Address.City,
                    Street = um.User.Address.Street,
                    ZipCode = um.User.Address.ZipCode,
                };

                UserProfile updatedUser = new UserProfile
                {
                    Firstname = um.User.Firstname,
                    Lastname = um.User.Lastname,
                    Email = um.User.Email,
                    Birthdate = um.User.Birthdate,
                    Phonnumber = um.User.Phonnumber,
                    Address = newAddress,

                };
            return updatedUser;

            }
            return new UserProfile();
        }


        public List<ViewDocument> GetDocumentsByRequestId(int requestId)
        {

            List<ViewDocument> documents = _context.Requestwisefiles.Where(u => u.Requestid == requestId).Select(r => new ViewDocument
            {
                filename = r.Filename,
                uploader = _context.Requests.FirstOrDefault(u => u.Requestid == requestId).Firstname,
                uploadDate = r.Createddate
            }).ToList();

             return documents;
        }

        public bool UploadFile(IFormFile file, int requestid)
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
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory())) + "\\wwwroot\\Upload";
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
