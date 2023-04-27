using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    public class PlaygameController : Controller
    {
        // GET: Playgame
        [TypeFilter(typeof(CheckUserSessionExpire))]
        public IActionResult Index(string gameid)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (!string.IsNullOrEmpty(gameid) && userSession != null && userSession.Id > 0)
            {
                GameMiddleWareModel model = new GameMiddleWareModel();
                model.GameId = gameid;
                return View("GameMiddleware", model);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { param = gameid });
            }
        }
        [TypeFilter(typeof(CheckAgentSessionExpire))]
        public IActionResult PlayAgentGame(string customerId, string gameid)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
         
        }


        [TypeFilter(typeof(CheckUserSessionExpire))]
        public IActionResult Game(string gameid)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
       
        }
        [TypeFilter(typeof(CheckAgentSessionExpire))]
        public IActionResult AgentGame(string customerId, string gameid)
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
           
        }
        //Playgame/GameMiddleware
        public IActionResult GameMiddleware()
        {
            return View();
        }
        public IActionResult CloseGame()
        {
            var userSession = HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
            if (userSession != null && userSession.Id > 0)
            {
                if (userSession.UserType == (int)UserType.Agent)
                {
                    return RedirectToAction("Index", "Games", new { area = "Agent" });
                }
                else
                {
                    return RedirectToAction("Lottery", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult InsufficentBalance()
        {
            return View();
        }
    }
}
