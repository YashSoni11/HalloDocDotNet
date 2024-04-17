using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IJwtServices
    {


        public  string GenerateJWTAuthetication(LoggedInUser loggedInUser);

        public bool ValidateToken(string token,out JwtSecurityToken jwtSecurityToken);


        public LoggedInUser GetLoggedInUserFromJwt(string token);



    }
}
