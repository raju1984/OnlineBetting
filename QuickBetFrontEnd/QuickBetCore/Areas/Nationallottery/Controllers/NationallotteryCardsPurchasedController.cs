using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.Nationallottery.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickbet.Areas.Nationallottery.Controllers
{
    [TypeFilter(typeof(CheckNationallotterySessionExpire))]
    public class NationallotteryCardsPurchasedController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NationallotteryCardsPurchasedController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: NationallotteryCardsPurchased/CardsPurchased
        public ActionResult BetHistory(string viewType = "w")
        {
            List<BetViewModel> betViews = new List<BetViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = NationallotteryCardsHistoryLogic.GetBetViews(viewType);
                }
            }
            catch { }
            return View(betViews);
        }

        public ActionResult WinningHistory(string viewType = "mm")
        {
            return View();
        }

        public ActionResult WinningHistorylist(string viewType = "mm")
        {
            List<WinViewModel> betViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = NationallotteryCardsHistoryLogic.GetWinViews(viewType);
                }
            }
            catch { }
            return PartialView("_WinningHistory", betViews);
        }

        public ActionResult RollbackHistory(string viewType = "mm")
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
    }
}