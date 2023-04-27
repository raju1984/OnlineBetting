using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Admin.Controllers
{
    [TypeFilter(typeof(CheckAdminSessionExpire))]
    public class CardsPurchasedController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CardsPurchasedController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/CardsPurchased
        public ActionResult BetHistory(string viewType = "mm")
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
            return View(betViews);
        }

        public ActionResult WinningHistory(string viewType = "mm")
        {
            return View();
        }

        public ActionResult WinningHistoryList(string viewType = "mm")
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
            return PartialView("_WinningHistory",winViews);
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
        public ActionResult JackpotHistory()
        {
            try
            {

            }
            catch { }
            return View();
        }
        public ActionResult JackpotHistoryList(string viewType = "mm")
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
            return PartialView("_JackpotHistoryList", betViews);
        }
        [HttpPost]
        public JsonResult ApproveJackpot(int id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                QuickbetDbEntities dbEntities = new QuickbetDbEntities();
                var playerWinObj = dbEntities.Playwins.Where(a => a.Id == id).FirstOrDefault();
                playerWinObj.IsJackAprovebyAdmin = true;
                dbEntities.SaveChanges();
                resp.Code = (int)ApiResponseCode.ok;
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }
    }
}