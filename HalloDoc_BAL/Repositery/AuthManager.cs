using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace HalloDoc_BAL.Repositery
{
    public class AuthManager : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
      
        public AuthManager(string role = "")
        {
            _role = role;
           
        }
        public  void OnAuthorization(AuthorizationFilterContext context)
        {
            //LoggedInUser user = SessionUtils.GetLoggedInUser(context.HttpContext.Session);

            var _jwtServices = context.HttpContext.RequestServices.GetRequiredService<IJwtServices>();


            if (_jwtServices == null) {

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
            }


            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];


           

             if (token == null || !_jwtServices.ValidateToken(token, out JwtSecurityToken jwtSecurityToken)){
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return;
            }


             var roleClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
            }

            if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
            {
                
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
                
            }
        }
    }
}