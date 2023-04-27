using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.Areas.User.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.APIAuth;
using QuickBetCore.Models.Data;
using QuickBetCore.Models.MobileAppModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [CustomAuthorizeFilter]
    public class AppCashbackController : ControllerBase
    {
        QuickbetDbEntities dbConn = new QuickbetDbEntities();
        //api/AppCashback/GetCheckForCashBack?UserId=
        [HttpGet]
        public ActionResult GetCheckForCashBack(int UserId)
        {
            CashBackViewModel cashBack = new CashBackViewModel();
            try
            {
                var cashBackAmount = DbOperation.GetRandomBonus();
                if (cashBackAmount > 0)
                {
                    ////UserOffer userOffer = new UserOffer();
                    ////userOffer.UserId = UserId;
                    ////userOffer.Value = cashBackAmount;
                    ////userOffer.IsRedeem = false;
                    ////userOffer.OfferType = (int)OfferType.RandomBonus;
                    ////userOffer.CreatedAt = DateTime.UtcNow;
                    ////userOffer.UpdatedAt = DateTime.UtcNow;
                    ////dbConn.UserOffers.Add(userOffer);
                    //dbConn.SaveChanges();
                    cashBack.isCashback = true;
                    cashBack.Text = "<p>Congratulations! you have won cashback</p><br><h4>Get <b>" + cashBackAmount + "</b> % cashback, When you deposit money into your Wallet.</h4>";
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(cashBack);
        }

        // GET: api/AppCashback/GetDashboardData?UserId=
        [HttpGet]
        public IActionResult GetDashboardData(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                UserDashboardViewModel userDashboardViewModel = new UserDashboardViewModel();
                userDashboardViewModel = UserDbLogic.GetUserDashboard(UserId);
                if (userDashboardViewModel.CashBack > 0)
                {
                    if (userDashboardViewModel.isInitail)
                    {
                        userDashboardViewModel.CashbackText = "You will receive<b> " + userDashboardViewModel.CashBack + " %</b>cashback<b> BONUS</b>on this initial deposit.";
                    }
                    else
                    {
                        userDashboardViewModel.CashbackText = "Make a<b>DEPOSIT</b> now and get a<b> " + userDashboardViewModel.CashBack + " %</b>  cashback<b> BONUS</b>";
                    }
                }
                resp.data = userDashboardViewModel;
            }
            catch (Exception ex)
            {
                resp.data = new UserDashboardViewModel();
            }
            return Ok(resp);
        }

        // GET: api/AppCashback/GetCashBackOffersHistory?UserId=
        [HttpGet]
        public IActionResult GetCashBackOffersHistory(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                CashBackOfferHistory cashBackOfferHistory = new CashBackOfferHistory();
                cashBackOfferHistory.Incoming = dbConn.CashBackOffersTransactions.Where(a => a.UserId == UserId
                 && a.IsCreditToWallet == false).OrderBy(a => a.CreatedAt).ToList();
                cashBackOfferHistory.History = dbConn.CashBackOffersTransactions.Where(a => a.UserId == UserId
                && a.IsCreditToWallet == true).OrderByDescending(a => a.CreatedAt).ToList();
                resp.data = cashBackOfferHistory;
            }
            catch (Exception ex)
            {
                resp.data = new CashBackOfferHistory();
            }
            return Ok(resp);
        }

        // GET: api/AppCashback/GetJackpotHistory?UserId=&viewType=
        public IActionResult GetJackpotHistory(int UserId,string viewType = "mm")
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            List<WinViewModel> betViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    betViews = CardsHistoryLogic.GetWinViewsJackpot(viewType, (int)UserType.Users, UserId);
                    resp.data = betViews;
                }
            }
            catch { }
            return Ok(resp);
        }
    }
    public class CashBackOfferHistory
    {
        public List<CashBackOffersTransaction> History { get; set; }
        public List<CashBackOffersTransaction> Incoming { get; set; }
    }

    public class CashBackViewModel
    {
        public bool isCashback { get; set; }
        public string Text { get; set; }
    }
}
