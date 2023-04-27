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
    public class BettingController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BettingController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Agent/Betting
        public ActionResult Index(int CustomerId = 0)
        {
            return View();
        }
        public ActionResult Getwinlist(int CustomerId = 0)
        {
            
            List<BettingViewModel> model = new List<BettingViewModel>();
            try
            {
                string username = userSession.name;
                if (CustomerId > 0)
                {
                    model = (from r in db.Playwins.Include("ExternalPlayerIdUser")
                             where r.AgentId == userSession.Id
                             && r.ExternalPlayerIdUserId == CustomerId
                             select new BettingViewModel
                             {
                                 Id = r.Id,
                                 customername = r.ExternalPlayerIdUser != null ? 
                                 r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : username,
                                 gamename = r.GameId,
                                 datetime = r.InsertAt,
                                 PaidMoneyStatus = r.PaidMoneyStatus,
                                 jackpotamount = r.JackpotAmount,
                             }).OrderByDescending(a => a.datetime).ToList();
                }
                else
                {
                    model = (from r in db.Playwins.Include("ExternalPlayerIdUser")
                             where r.ExternalPlayerIdUserId == userSession.Id
                             select new BettingViewModel
                             {
                                 Id = r.Id,
                                 customername = r.ExternalPlayerIdUser != null ?
                                 r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : username,
                                 gamename = r.GameId,
                                 datetime = r.InsertAt,
                                 PaidMoneyStatus = r.PaidMoneyStatus,
                                 jackpotamount = r.JackpotAmount,
                             }).OrderByDescending(a => a.datetime).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_winninglist", model);
        }
        public ActionResult Getbetlist(int CustomerId = 0)
        {
            
            List<BettingViewModel> model = new List<BettingViewModel>();
            try
            {
                string username = userSession.name;
                if (CustomerId > 0)
                {
                    model = (from r in db.PlayerBets.Include("ExternalPlayerIdUser")
                             where r.AgentId == userSession.Id
                             && r.ExternalPlayerIdUserId == CustomerId
                             select new BettingViewModel
                             {
                                 Id = r.Id,
                                 customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + 
                                 "(" + r.ExternalPlayerIdUser.Email + ")" : username,
                                 gamename = r.GameName,
                                 datetime = r.Insertdate,
                                 betamount = r.Amount,
                                 quikbetcommision = r.AgentCommison,
                             }).OrderByDescending(a => a.datetime).ToList();
                }
                else
                {
                    model = (from r in db.PlayerBets.Include("ExternalPlayerIdUser")
                             where r.ExternalPlayerIdUserId == userSession.Id
                             select new BettingViewModel
                             {
                                 Id = r.Id,
                                 customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name +
                                 "(" + r.ExternalPlayerIdUser.Email + ")" : username,
                                 gamename = r.GameName,
                                 datetime = r.Insertdate,
                                 betamount = r.Amount,
                                 quikbetcommision = r.AgentCommison,
                             }).OrderByDescending(a => a.datetime).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_bettinglist", model);
        }

        public ActionResult GetBarcode(int WinId)
        {
            BarCodeViewModel model = new BarCodeViewModel();
            
            try
            {
                var winobj = db.WalletTransactions.Where(a => a.Id == WinId && a.Status == (int)WalletTransactionStatusType.TransactionInitiated).FirstOrDefault();
                if (string.IsNullOrEmpty(winobj.Barcode))
                {
                    winobj.Barcode = winobj.Id + ApplicatiopnCommonFunction.AlphanumbericNumber();
                    db.SaveChanges();
                }
                model.Id = winobj.Id;
                //model.wincode = winobj.winnerCode;
                model.amount = winobj.Amount;
               // model.barcode = ApplicatiopnCommonFunction.GenerateBarcode(winobj.Barcode);
            }
            catch (Exception ex)
            {
                model.msg = ex.Message;
            }
            return PartialView("_Barcode", model);
        }
        [HttpPost]
        public JsonResult PayLottery([FromBody]PayLotteryModel payLottery)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DateTime currentDate = DateTime.UtcNow;
     
                decimal ApproveTransactions = 0;
                var GetAmount = db.WalletTransactions.Where(x => x.Id == payLottery.WinId).FirstOrDefault();
                if (GetAmount != null)
                {
                    var ApproveEntries = db.WalletTransactions.Where(x => x.Agentid == userSession.Id
                && (x.Status == (int)WalletTransactionStatusType.TransactionPending ||x.Status == (int)WalletTransactionStatusType.TransactionSuccess) && x.TransType == (int)TransType.Debit
                && x.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer && 
                x.LastUpdated.Date == currentDate.Date).ToList();


                    if (ApproveEntries != null && ApproveEntries.Count > 0)
                    {

                        ApproveTransactions = ApproveEntries.Sum(a => a.Amount);
                    }

                    var Total = ApproveTransactions;
                    if (GetAmount.Amount <= 50000)
                    {
                        if (Total <= 50000)
                        {
                            var winobj = db.WalletTransactions.Where(a => a.Id == payLottery.WinId && a.TransType == (int)TransType.Debit && a.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer && a.Status == (int)WalletTransactionStatusType.TransactionInitiated).FirstOrDefault();
                            if (winobj != null)
                            {
                                winobj.Status = (int)WalletTransactionStatusType.TransactionPending;
                                winobj.LastUpdated = DateTime.UtcNow;
                                db.SaveChanges();

                                var objuser = db.Users.Where(x => x.Id == winobj.UserId).FirstOrDefault();
                                string SincerelyName = "QuickBet Team";
                                String Subject = "Approval mail";

                                String Body = "Hi " + objuser.Name + ",<br><br>Amount:₦ " +
                                winobj.Amount + "has been approved by the Agent, Please login your account and approve/reject the request to complete the process" +
                                "<br><br>Sincerely,<br>" + SincerelyName ;
                                CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                                resp.Code = (int)ApiResponseCode.ok;
                                resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
                                return Json(resp);
                            }
                            else
                            {
                                resp.Msg = "Invalid Transactions!";
                                return Json(resp);
                            }
                        }
                        else
                        {
                            resp.Msg = "Your daily Limit of ₦ 50000 is over!";
                            return Json(resp);
                        }
                    }
                    else
                    {
                        resp.Msg = "Requested amount should not be greater than 50000!";
                        return Json(resp);
                    }
                }
                else
                {
                    resp.Msg = "Something went wrong!";
                    return Json(resp);
                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }
    }
}