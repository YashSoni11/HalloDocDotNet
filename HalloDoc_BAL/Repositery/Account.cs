using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.ViewModels;
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


        public Object ValidateLogin(UserCred um)
        {
             
                var user  =   _context.Aspnetusers.FirstOrDefault(u=>um.Email == u.Email && um.Password == u.Passwordhash);

            return user;
        }

    }
}
