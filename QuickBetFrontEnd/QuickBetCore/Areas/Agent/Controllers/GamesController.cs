using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Quickbet.Areas.Agent.Controllers
{
    [TypeFilter(typeof(CheckAgentSessionExpire))]
    public class GamesController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GamesController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Agent/Games
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetGamelist()
        {
            List<GameViewModel> model = new List<GameViewModel>();
            try
            {
                model = DbOperation.Getgamelist(0);
                return PartialView("_gamelist", model);
            }
            catch (Exception ex)
            {
                model = new List<GameViewModel>();
            }
            return PartialView("_gamelist", model);
        }
        public ActionResult ChooseCustomer(int gameid)
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                if (gameid > 0)
                {
                    model = (from r in db.AgentCustomers.Include("User")
                             where r.AgentId == userSession.Id && r.User != null
                             && r.User.UserStatus==(int)UserStatus.active
                             select new UserListViewModel
                             {
                                 Id = r.User.Id,
                                 name = r.User.Name,
                                 email = r.User.Email,
                                 countrycode = r.User.CountryCode,
                                 phone = r.User.ContactNo,
                             }).OrderBy(a=>a.name).ToList();
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}