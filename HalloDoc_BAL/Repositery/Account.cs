using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repositery
{
    public class Account : IAccount
    {
        private readonly HalloDocContext _context;

      public  Account(HalloDocContext context)
        {
             _context = context;
        }
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

                Aspnetuser user  =   _context.Aspnetusers.FirstOrDefault(u=>um.Email == u.Email && GetHashedPassword(um.Password) == u.Passwordhash);

            return user;
        }


        public User GetUserByAspNetId(string id)
        {
             
            User user  = _context.Users.FirstOrDefault(u=>u.Aspnetuserid == id);

            return user;

        }


        public Object GetUserRequests(int userid)
        {
             
            var requests = _context.Requests.Where(u=>u.Userid == userid).ToList();

            return requests;
        }




    }
}
