using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using QuickBetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Filters
{

    public class CheckSessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (currentUser == null || currentUser.Id == 0)
            {
                context.Result = new RedirectToRouteResult
                 (
                 new RouteValueDictionary(new
                 {
                     action = "Login",
                     controller = "Account",
                     area = ""
                 }));
            }
        }
    }
    public class CheckUserSessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (currentUser == null || currentUser.Id == 0 || currentUser.UserType!=(int)UserType.Users)
            {
                context.Result = new RedirectToRouteResult
                 (
                 new RouteValueDictionary(new
                 {
                     action = "Login",
                     controller = "Account",
                     area = ""
                 }));
            }
        }
    }

    public class CheckAgentSessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (currentUser != null && currentUser.Id > 0 && 
                (currentUser.UserType == (int)UserType.MobileAgent || currentUser.UserType == (int)UserType.Agent))
            {
               
            }
            else
            {
                context.Result = new RedirectToRouteResult
                (
                new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Account",
                    area = ""
                }));
            }
        }
    }
    public class CheckNationallotterySessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (currentUser == null || currentUser.Id == 0 || currentUser.UserType != (int)UserType.Nationallottery)
            {
                context.Result = new RedirectToRouteResult
                 (
                 new RouteValueDictionary(new
                 {
                     action = "Login",
                     controller = "Account",
                     area = ""
                 }));
            }
        }
    }
    public class CheckSuperAgentSessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (currentUser == null || currentUser.Id == 0 || currentUser.UserType != (int)UserType.SuperAgent)
            {
                context.Result = new RedirectToRouteResult
                 (
                 new RouteValueDictionary(new
                 {
                     action = "Login",
                     controller = "Account",
                     area = ""
                 }));
            }
        }
    }

    public class CheckAdminSessionExpire : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            UserSession currentUser = context.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);

            if (currentUser != null && (currentUser.UserType == (int)UserType.Admin || currentUser.UserType == (int)UserType.Staff))
            {
                //valid user
            }
            else
            {
                context.Result = new RedirectToRouteResult
                (
                new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Account",
                    area = ""
                }));
            }

        }
    }

}
