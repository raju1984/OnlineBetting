using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    //[Route("Agent/[controller]/[action]")]
    public class DashboardController : Controller
    {
        QuickbetDbEntities dbConn = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Agent/Dashboard
        public ActionResult Index()
        {
            AgentDahsboardViewModel model = new AgentDahsboardViewModel();
            try
            {
                if (TempData["actionresponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //response;
                }
                
                var SuperAgentRequest = dbConn.AgentPromotionEntries.Where(x => x.AgentId == userSession.Id && x.ApprovedAdminId == 0).FirstOrDefault();
                if (SuperAgentRequest != null)
                {
                    ViewBag.SuperAgentRequest = true;
                }
                else
                {
                    ViewBag.SuperAgentRequest = false;
                }
                var userobj = dbConn.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                userSession.UserStatus = userobj.UserStatus;
                userSession.MyWallet = userobj.MyWalletbalance;
                HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                model.AgentCommison = userobj.AgentCommison;
                model.CashBackonPayment = userobj.AgentCashBackOnPayment;
                model.CustomerRetention = userobj.CustomerRetentionPeriod;
                model.UserType = userobj.UserType;
                if (userobj.UserStatus == (int)UserStatus.Pending_for_approval)
                {
                    model.isAprove = false;
                }
                else
                {
                    model.isAprove = true;
                }

                if (userobj.MyWalletbalance >= ApplicationVariable.MininumBalance)
                {
                    model.IsBalance = true;
                }
                else
                {
                    model.IsBalance = false;
                }

                model.Balance = PaymentDb.GetBalance(userSession.Id);
                model.NoofCustomer = dbConn.AgentCustomers.Where(a => a.AgentId == userSession.Id).Count();

                if (dbConn.PlayerBets.Any(a => a.AgentId == userSession.Id))
                {
                    model.TotalBetAmount = dbConn.PlayerBets.Where(a => a.AgentId == userSession.Id).Sum(a => a.Amount);
                }
                if (dbConn.Playwins.Any(a => a.AgentId == userSession.Id))
                {
                    model.TotalWinAmount = dbConn.Playwins.Where(a => a.AgentId == userSession.Id).Sum(a => a.JackpotAmount);
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
                string username = userSession.name;
                //model = (from r in dbConn.Playwins.Include("User")
                //         where
                //         r.AgentId == userSession.Id &&
                //         (r.PaidMoneyStatus == (int)WinPaidMoneyStatus.Paid_Request_Initiated
                //         || r.PaidMoneyStatus == (int)WinPaidMoneyStatus.Paid_By_Agent_Waiting_User_Aproval)
                //         select new BettingViewModel
                //         {
                //             Id = r.Id,
                //             customername = r.User != null ? r.User.Name + "(" + r.User.Email + ")" : username,
                //             gamename = r.gameName,
                //             datetime = r.InsertAt,
                //             PaidMoneyStatus = r.PaidMoneyStatus,
                //             jackpotamount = r.jackpotAmount,
                //             WithdrawrequestAmount = r.WithDrawAmount,
                //         }).OrderByDescending(a => a.datetime).ToList();

                model = (from r in dbConn.WalletTransactions
                         join u in dbConn.Users
                         on r.UserId equals u.Id
                         where r.Agentid == userSession.Id && r.TransType == (int)TransType.Debit && r.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer && (r.Status == (int)WalletTransactionStatusType.TransactionInitiated || r.Status == (int)WalletTransactionStatusType.TransactionPending)
                         select new BettingViewModel
                         {
                             Id = r.Id,
                             customername = u.Name != null ? u.Name + "(" + r.User.Email + ")" : username,
                             datetime = r.InsertDate,
                             PaidMoneyStatus = r.Status,
                             WithdrawrequestAmount = r.Amount,
                             TransactionId = r.TransactionId

                         }).OrderByDescending(a => a.datetime).ToList();

                ViewBag.models = (from r in dbConn.WalletTransactions
                                  join e in dbConn.Users
                                     on r.UserId equals e.Id
                                  where r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountAgent 
                                  && r.Status == (int)WalletTransactionStatusType.TransactionPending && 
                                  r.UserId == userSession.Id
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

                var winobj = dbConn.WalletTransactions.Where(x => x.Id == aproveDenyPayment.WalletId).FirstOrDefault();
                //var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                if (winobj != null)
                {
                    if (aproveDenyPayment.Action == 1)
                    {
                        winobj.Status = (int)WalletTransactionStatusType.TransactionSuccess;

                        var objuser = dbConn.Users.Where(x => x.Id == winobj.UserId).FirstOrDefault();
                        string SincerelyName = "QuickBet Team";
                        String Subject = "money withdrawal";

                        String Body = "Hi " + objuser.Name + ",<br><br>Congratulations requested " +
                        "Amount of " + winobj.Amount + "has been approved by the admin" + objuser.Name + "Please approve from your side." +
                        "<br><br>Sincerely,<br>" + SincerelyName ;
                        CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);

                        if (dbConn.SaveChanges() > 0)
                        {
                            resp.Code = (int)ApiResponseCode.ok;
                            resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
                        }
                        return Json(resp);
                    }
                    else
                    {
                        var response = PaymentDb.ReverseMoneyToWalletAmountRejectedcaseAgentToAdmin(winobj.Amount, 
                            userSession.Id, winobj.TransactionId);

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
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        #region Promotion as Super agent
        public ActionResult PromotionAsSuperAgent()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                var UserId = userSession.Id;
                var check = dbConn.Users.Where(x => x.Id == UserId && x.UserStatus == (int)UserStatus.active
                && (x.UserType == (int)UserType.MobileAgent || x.UserType == (int)UserType.Agent)).FirstOrDefault();

                if (check != null)
                {

                    var agent = dbConn.AgentPromotionEntries.Where(x => x.AgentId == userSession.Id

                    && x.ApprovedAdminId == 0).FirstOrDefault();
                    if (agent == null)
                    {
                        agent = new AgentPromotionEntry();

                        agent.IsApproved = false;
                        agent.AgentId = userSession.Id;
                        agent.AgentType = check.UserType;
                        agent.ApprovedAdminId = 0;
                        agent.InsertedDate = DateTime.UtcNow;
                        agent.UpdatedDate = DateTime.UtcNow;

                        dbConn.AgentPromotionEntries.Add(agent);
                        if (dbConn.SaveChanges() > 0)
                        {
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = Applicationstring.Success;
                        }
                        else
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = Applicationstring.somethingwentwrong;

                        }
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "Already Requested,Request in progress";

                    }
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    response.Msg = "UserId does not exists";

                }
                TempData["actionresponse"] = JsonConvert.SerializeObject(response);
            }
            catch (Exception ex)
            {
                response.Code = (int)ApiResponseCode.fail;
                response.Msg = Applicationstring.somethingwentwrong;
            }
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion
    }
}