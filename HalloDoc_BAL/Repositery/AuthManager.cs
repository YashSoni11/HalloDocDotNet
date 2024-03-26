using HalloDoc_BAL.Interface;
using HalloDoc_DAL.Context;
using HalloDoc_DAL.Models;
using HalloDoc_DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace HalloDoc_BAL.Repositery
{
    public class AuthManager :   IAuthManager     /* Attribute , IAuthorizationFilter*/
    {
        private readonly string _role;
        private readonly HalloDocContext _context;

        public AuthManager(HalloDocContext context,string role = "")
        {
            _context = context;
            _role = role;
           
        }

        public bool Authorize(HttpContext context,int RoleMenuId)
        {
            

            var _jwtServices = context.RequestServices.GetRequiredService<IJwtServices>();


            if (_jwtServices == null)
            {

                return false;
            }


          
            var token = context.Request.Cookies["jwt"];




            if (token == null || !_jwtServices.ValidateToken(token, out JwtSecurityToken jwtSecurityToken))
            {


                //if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                //{
                //    context.Result = new JsonResult(new { code = 401 });
                //    return;
                //}
                //else
                //{

                //context.HttpContext.Response.Cookies.Append("LoginMsg", "true");
                //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
                return false;
                //}
                //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "Index" }));

            }


            int roleId = int.Parse(jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "Role").Value);

            bool isRistricted = IsRistricted(RoleMenuId, roleId);

            if (!isRistricted)
            {
                return false;
                //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
            }






            //if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
            //{



            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));


            //    return;

            //}

            return true;    
        }


        public bool IsRistricted(int id,int roleId)
        {
             return  _context.Rolemenus.Where(q=>q.Roleid == roleId).Any(r=>r.Rolemenuid == id);
        }


        //public  void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    //LoggedInUser user = SessionUtils.GetLoggedInUser(context.HttpContext.Session);

        //    var _jwtServices = context.HttpContext.RequestServices.GetRequiredService<IJwtServices>();


        //    if (_jwtServices == null) {

        //        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
        //    }


        //    var request = context.HttpContext.Request;
        //    var token = request.Cookies["jwt"];




        //     if (token == null || !_jwtServices.ValidateToken(token, out JwtSecurityToken jwtSecurityToken)){


        //        //if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //        //{
        //        //    context.Result = new JsonResult(new { code = 401 });
        //        //    return;
        //        //}
        //        //else
        //        //{

        //        context.HttpContext.Response.Cookies.Append("LoginMsg", "true");     
        //            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login"}));
        //            return;
        //        //}
        //        //context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "Index" }));

        //    }


        //     var roleClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "Role");

        //    if (roleClaim == null)
        //    {
        //        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Error" }));
        //    }

        //    if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
        //    {



        //        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "AccessDenied" }));


        //        return;

        //    } 
        //}
    }
}