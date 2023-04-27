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
    public class NationallotteryDashboardController : Controller
    {
        QuickbetDbEntities dbConn = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NationallotteryDashboardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: NationallotteryDashboard/Dashboard
        public ActionResult Index()
        {
            NationallotteryDahsboardViewModel model = new NationallotteryDahsboardViewModel();
            try
            {
                
                var userobj = dbConn.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                userSession.UserStatus = userobj.UserStatus;
                //
                DateTime startdayofMothe = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var bets= dbConn.PlayerBets.Where(a => a.Insertdate.Date >= startdayofMothe.Date).ToList();
                if(bets!=null && bets.Count()>0)
                {
                    model.CurrentMontheBet = bets.Sum(a => a.Amount);
                    model.CurrentMontheCommison = model.CurrentMontheBet * Convert.ToDecimal(.03);
                }
                var startoflastmonthe = startdayofMothe.AddMonths(-1);

                DateTime firstdayOfMonthe = new DateTime(startoflastmonthe.Year, startoflastmonthe.Month, 1);

                var days= DateTime.DaysInMonth(startoflastmonthe.Year, startoflastmonthe.Month);
                DateTime lastdayOfMonthe = new DateTime(startoflastmonthe.Year, startoflastmonthe.Month, days);

                var lastmonthe = dbConn.PlayerBets.Where(a => a.Insertdate.Date >= firstdayOfMonthe.Date
                && a.Insertdate.Date <= lastdayOfMonthe.Date).ToList();

                if(lastmonthe!=null && lastmonthe.Count()>0)
                {
                    model.LastMontheBet = lastmonthe.Sum(a => a.Amount);
                    model.LastMontheCommison = model.LastMontheBet * Convert.ToDecimal(.03);
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
    }
}