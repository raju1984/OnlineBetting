using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Controllers
{
    [TypeFilter(typeof(CheckUserSessionExpire))]
    public class CardsPlayedController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CardsPlayedController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/CardsPlayed
        public IActionResult BetHistory(string viewType = "mm")
        {
            return View();
        }

        public IActionResult GetBetHistorylist(string viewType = "mm")
        {
            List<BetViewModel> betViews = new List<BetViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = CardsHistoryLogic.GetBetViews(viewType, userSession.UserType, userSession.Id);
                }
            }
            catch { }
            return PartialView("_BetHistory",betViews);
        }

        public IActionResult WinningHistory(string viewType = "mm")
        {
           
            return View();
        }
        public IActionResult GetWinningHistorylist(string viewType = "mm")
        {
            List<WinViewModel> winViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    winViews = CardsHistoryLogic.GetWinViews(viewType, userSession.UserType, userSession.Id);
                }
            }
            catch { }
            return PartialView("_WinningHistory", winViews);
        }
        public IActionResult RollbackHistory(string viewType = "mm")
        {
            List<BetViewModel> betViews = new List<BetViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = CardsHistoryLogic.GetRollbackViews(viewType, userSession.UserType, userSession.Id);
                }
            }
            catch { }
            return View(betViews);
        }

        public IActionResult JackpotHistory(string viewType = "mm")
        {
            List<WinViewModel> betViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = CardsHistoryLogic.GetWinViewsJackpot(viewType, userSession.UserType, userSession.Id);
                }
            }
            catch { }
            return View(betViews);
        }
        public IActionResult GetJackpotHistoryList(string viewType = "mm")
        {
            List<WinViewModel> winViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    winViews = CardsHistoryLogic.GetWinViewsJackpot(viewType, userSession.UserType, userSession.Id);
                }
            }
            catch { }
            return PartialView("_JackpotHistory", winViews);
        }
    }
}
