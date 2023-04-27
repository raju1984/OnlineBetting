using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickBetCore.DatabaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models.APIAuth
{
    public class CustomError
    {
        public string Error { get; }
        public CustomError(string message)
        {
            Error = message;
        }
    }
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message)
            : base(new CustomError(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
    public class CustomAuthorizeFilter : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                //var bodyStr = "";
                var req = context.HttpContext.Request;
                // Allows using several time the stream in ASP.Net Core
                req.EnableBuffering();
                // Arguments: Stream, Encoding, detect encoding, buffer size 
                // AND, the most important: keep stream opened
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                //Extract credentials
                if (authHeader != null)
                {
                    string Access_Token = authHeader.Substring("Bearer ".Length).Trim();
                    if (!string.IsNullOrEmpty(Access_Token))
                    {
                        QuickbetDbEntities dbConn = new QuickbetDbEntities();
                        var IsTokenExist =dbConn.Users.Any(a => a.Token == Access_Token && a.UserStatus == (int)UserStatus.active);
                        if (IsTokenExist)
                        {
                            // setting current principle
                            return;
                        }
                        else
                        {
                            // Return custom 401 result
                            context.Result = new CustomUnauthorizedResult("Invalid Token");
                        }
                    }
                    else
                    {
                        // Return custom 401 result
                        context.Result = new CustomUnauthorizedResult("Token not found in request parameters");
                    }
                }
                else
                {
                    context.Result = new CustomUnauthorizedResult("Token not found in request parameters");

                }
            }
            catch (Exception ex)
            {
                context.Result = new CustomUnauthorizedResult(ex.Message);
            }

        }
    }
}
