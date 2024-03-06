using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace HalloDoc_BAL.Repositery
{
    public class SessionUtils
    {



        public static LoggedInUser GetLoggedInUser(ISession session)
        {
            LoggedInUser userInformation = null;

            if (!string.IsNullOrEmpty(session.GetString("UserId")))
            {
                userInformation = new LoggedInUser();
                userInformation.UserId = Convert.ToInt32(session.GetString("UserId"));
                userInformation.Firstname = session.GetString("Firstname");
                userInformation.Lastname = session.GetString("Lastname");
                userInformation.Role = session.GetString("Role");
            }

            return userInformation;
        }


        public static void SetLoggedInUser(ISession session,User user) {
        
          
           if(user != null)
            {
                session.SetString("UserId", user.Userid.ToString());
                session.SetString("Firstname", user.Firstname);
                session.SetString("Lastname",user.Lastname);
                session.SetString("Role","Patient");
              

            }
        
        }


    }
}
