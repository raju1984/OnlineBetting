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

namespace Quickbet.Areas.SuperAgent.Controllers
{
    [TypeFilter(typeof(CheckSuperAgentSessionExpire))]

    public class SuperDashboardController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SuperDashboardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: SuperAgent/SuperDashboard
        public ActionResult Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            try
            {

                model.Agents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
          && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.Pending_for_approval).Count();

                model.TotalAgents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
                && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.active).Count();

                //model.BlockedAgent = db.Users.Where(a => a.UserType == (int)UserType.Agent
                // && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.block).Count();

                var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                if (userobj.UserStatus == (int)UserStatus.Pending_for_approval)
                {
                    model.isAprove = false;
                    model.SuperAgentBalance = userobj.MyWalletbalance;
                }
                else
                {
                    model.isAprove = true; model.SuperAgentBalance = userobj.MyWalletbalance;
                }
                userSession.UserStatus = userobj.UserStatus;
                HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                DateTime currentDate = DateTime.UtcNow;
                var AgentPromBoj = db.AgentPromotionEntries.Where(x => x.AgentId == userSession.Id && x.IsApproved == true
            && x.InsertedDate.Date == currentDate.Date).FirstOrDefault();
                if (AgentPromBoj != null)
                {
                    model.IsEntryAvailable = true;
                }
                else
                {
                    model.IsEntryAvailable = false;
                }

                var TotalEarningAgents = db.Users.Where(x => x.ParentAgentId == userSession.Id).ToList();

                if (TotalEarningAgents != null && TotalEarningAgents.Count > 0)
                {
                    decimal TotalAmounts = 0;
                    decimal? TotalAmount = 0;
                    foreach (var item in TotalEarningAgents)
                    {
                        TotalAmount = db.WalletTransactions.Where(x => x.UserId == item.Id
                        && (x.TransferType == (int)WalletTransactionType.AgentCommission_onBet || x.TransferType == (int)WalletTransactionType.Cashback_On_WinPrize) && x.Status == (int)WalletTransactionStatusType.TransactionSuccess).Sum(x => (decimal?)x.Amount).GetValueOrDefault();
                        TotalAmounts = (decimal)(TotalAmounts + TotalAmount);

                    }


                    model.TotalAgentsEarning = TotalAmounts;
                }


            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        public ActionResult GetwinWithdrawRequets()
        {
            
            List<BettingViewModel> model = new List<BettingViewModel>();
            try
            {

                ViewBag.models = (from r in db.WalletTransactions
                                  join e in db.Users
                                     on r.UserId equals e.Id
                                  where r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountSuperAgent && r.Status == (int)WalletTransactionStatusType.TransactionInitiated && r.UserId == userSession.Id
                                  select new WalletAccountWithralRequestModel
                                  {
                                      Id = r.Id,
                                      Amount = r.Amount,
                                      RequestedBy = e.Name,
                                      RequestedDate = r.InsertDate,
                                      TransactionId = r.TransactionId,
                                      TransactionType = r.TransType,
                                      ApprovalStatus = r.Status
                                  }).OrderByDescending(a => a.RequestedDate).ToList();


            }
            catch (Exception ex)
            {

            }
            return PartialView("_winninglist", model);
        }


        [HttpPost]
        public JsonResult AproveDenyPayment([FromBody] AproveDenyPayment aproveDenyPayment)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var winobj = dbConn.WalletTransactions.Where(x => x.Id == aproveDenyPayment.WalletId).FirstOrDefault();
                    //var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (winobj != null)
                    {
                        if (aproveDenyPayment.Action == 1)
                        {
                            winobj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                            winobj.LastUpdated = DateTime.UtcNow;
                            if (dbConn.SaveChanges() > 0)
                            {
                                resp.Code = (int)ApiResponseCode.ok;
                                resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
                            }
                            return Json(resp);
                        }
                        else
                        {
                            var response = PaymentDb.ReverseMoneyToWalletAmountRejectedcaseSuperAgentToAdmin(winobj.Amount, userSession.Id, winobj.TransactionId);

                            if (response)
                            {
                                resp.Code = (int)ApiResponseCode.ok;
                                resp.Msg = Applicationstring.Requestcanceledsuccessfully;
                            }
                            return Json(resp);
                        }
                    }
                    else
                    {
                        resp.Msg = "Invalid Lottery!";
                        return Json(resp);
                    }

                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        [HttpPost]
        public JsonResult AproveDenyPaymentSelf(int WalletId, int Action)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var winobj = dbConn.WalletTransactions.Where(x => x.Id == WalletId).FirstOrDefault();
                    //var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (winobj != null)
                    {
                        if (Action == 0)
                        {
                            var response = PaymentDb.ReverseMoneyToWalletAmountRejectedSelfcaseSuperAgentToAdmin(winobj.Amount, userSession.Id, winobj.TransactionId);

                            if (response)
                            {
                                resp.Code = (int)ApiResponseCode.ok;
                                resp.Msg = Applicationstring.Requestcanceledsuccessfully;

                            }

                        }
                    }
                    else
                    {
                        resp.Msg = "Invalid Lottery!";

                    }

                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;

            }
            return Json(resp);
        }

    }
}