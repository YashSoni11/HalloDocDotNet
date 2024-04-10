using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using HalloDoc_BAL.Interface;
using HalloDoc_DAL.ViewModels;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repositery
{
    public class JwtServices : IJwtServices
    {

        private readonly IConfiguration configuration;
     

        public JwtServices(IConfiguration configuration)
        {
             
            this.configuration = configuration;
        }

        public  string GenerateJWTAuthetication(LoggedInUser user)
        {
            var claims = new List<Claim>
            {
            
                new Claim("Role", user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Firstname", user.Firstname),
                
               

            };


         


            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires =  DateTime.UtcNow.AddMinutes(100);
                   

            var token = new JwtSecurityToken(
              configuration["JWT:Issuer"],
              configuration["JWT:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }


        public  bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(configuration["JWT:Key"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);


                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if(jwtSecurityToken != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
