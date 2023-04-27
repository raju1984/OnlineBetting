using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.SuperAgent.Data;
using QuickBetCore.Areas.User.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Controllers
{
    [TypeFilter(typeof(CheckUserSessionExpire))]
    public class UserDashboardController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public UserDashboardController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        QuickbetDbEntities db = new QuickbetDbEntities();
        // GET: User/UserDashboard
        public IActionResult Index(string viewType = "mm")
        {
            UserDashboardViewModel userDashboardViewModel = new UserDashboardViewModel();

            try
            {
                if (TempData["ModelError"] != null)
                {
                    ViewData["error"] = TempData["ModelError"].ToString();
                }
                List<FundTransferVModel> result = new List<FundTransferVModel>();
                userDashboardViewModel = UserDbLogic.GetUserDashboard(userSession.Id);
            }
            catch (Exception ex)
            {

            }
            return View(userDashboardViewModel);
        }

        public IActionResult CashBackOffers()
        {
            List<CashBackOffersTransaction> cashBackOffers = new List<CashBackOffersTransaction>();
            try
            {
                cashBackOffers = db.CashBackOffersTransactions.Where(a => a.UserId == userSession.Id
                && a.IsCreditToWallet==false).OrderBy(a=>a.CreatedAt).ToList();
            }
            catch(Exception ex)
            {

            }
            return View(cashBackOffers);
        }
        public IActionResult CashBackHistory()
        {
            List<CashBackOffersTransaction> cashBackOffers = new List<CashBackOffersTransaction>();
            try
            {
                cashBackOffers = db.CashBackOffersTransactions.Where(a => a.UserId == userSession.Id
                && a.IsCreditToWallet == true).OrderByDescending(a=>a.CreatedAt).ToList();
            }
            catch (Exception ex)
            {

            }
            ViewData["history"] = "h";
            return View("CashBackOffers",cashBackOffers);
        }
        public IActionResult WiningBetForPayment()
        {
            List<WalletAccountWithralRequestModel> betViews = new List<WalletAccountWithralRequestModel>();
            try
            {
                //only get non
                //betViews = (from _bet in db.Playwins
                //            where _bet.externalPlayerId_UserId == userSession.Id &&
                //            _bet.AgentId != null &&
                //            (_bet.PaidMoneyStatus == (int)WinPaidMoneyStatus.NotPaid
                //            || _bet.PaidMoneyStatus == (int)WinPaidMoneyStatus.Paid_Request_Initiated
                //            || _bet.PaidMoneyStatus == (int)WinPaidMoneyStatus.Paid_By_Agent_Waiting_User_Aproval)
                //            select new WinViewModel
                //            {
                //                Id = _bet.Id,
                //                Player_UserId = _bet.externalPlayerId_UserId,
                //                betamount = _bet.betAmount == null ? 0 : _bet.betAmount.Value,
                //                winamount = _bet.jackpotAmount,
                //                withdrawamount = _bet.WithDrawAmount,
                //                currency = _bet.currency,
                //                gameId = _bet.gameId,
                //                GameName = _bet.gameName,
                //                transactionId = _bet.transactionId,
                //                type = _bet.type,
                //                freeRoundsRemaining = _bet.freeRoundsRemaining,
                //                Insertdate = _bet.InsertAt,
                //                PaidMoneyStatus = _bet.PaidMoneyStatus
                //            }).OrderByDescending(x => x.Insertdate).ToList();

                ViewBag.models = (from b in db.WalletTransactions
                                  join e in db.Users
                                  on b.UserId equals e.Id
                                  join ee in db.Users
                                  on b.Agentid equals ee.Id
                                  where b.UserId == userSession.Id && (b.Status == (int)WalletTransactionStatusType.TransactionPending || b.Status == (int)WalletTransactionStatusType.TransactionInitiated) &&
                                  b.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer && b.TransType == (int)TransType.Debit
                                  select new WalletAccountWithralRequestModel
                                  {
                                      Id = b.Id,
                                      Amount = b.Amount,
                                      RequestedBy = ee.Name,
                                      RequestedDate = b.InsertDate,
                                      TransactionId = b.TransactionId,
                                      TransactionType = b.TransType,
                                      ApprovalStatus = b.Status
                                  }).OrderByDescending(a => a.RequestedDate).ToList();

                return PartialView("_WinningBet", betViews);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_WinningBet", betViews);
        }

        public String GetAgentName(string Agentid)
        {
            String DefaultName = "Agent";
            try
            {
                //if (Agentid > 0)
                //{
                //    var Name = db.Users.Where(x => x.Id == Agentid).FirstOrDefault();

                //    if (Name != null)
                //    {
                //        DefaultName = Name.Name;
                //    }
                //}
            }
            catch (Exception ex)
            {
            }
            return DefaultName;
        }

        public IActionResult WiningBetRedeem(int wining)
        {
            RedeemLotteryAmountModel redeemLottery = new RedeemLotteryAmountModel();
            redeemLottery.ErrorMessage = new Error();
            redeemLottery.ErrorMessage.ErrorCode = (int)ApiResponseCode.fail;
            try
            {
                var playwin = db.Playwins.Include("Agent").Where(a => a.Id == wining && a.ExternalPlayerIdUserId == userSession.Id).FirstOrDefault();
                if (playwin != null && playwin.Agent != null)
                {
                    if (playwin.Agent.UserStatus == (int)UserStatus.active)
                    {
                        redeemLottery.winingId = playwin.Id;
                        redeemLottery.AgentName = playwin.Agent.Name;
                        if (!string.IsNullOrEmpty(playwin.Agent.CountryCode))
                        {
                            redeemLottery.AgentNumber = playwin.Agent.CountryCode + "-" + playwin.Agent.ContactNo;
                        }
                        else
                        {
                            redeemLottery.AgentNumber = playwin.Agent.ContactNo;
                        }

                        redeemLottery.AgentId = playwin.Agent.Id;
                        var balance = PaymentDb.GetClosingBalance(userSession.Id);
                        if (balance > playwin.JackpotAmount)
                        {
                            redeemLottery.Amount = playwin.JackpotAmount;
                        }
                        else
                        {
                            redeemLottery.Amount = balance;
                        }
                        redeemLottery.ErrorMessage.ErrorCode = (int)ApiResponseCode.ok;
                        return View(redeemLottery);
                    }
                    else
                    {
                        redeemLottery.ErrorMessage = new Error()
                        {
                            ErrorCode = (int)ReturnCode.Failed,
                            ErrorMessage = "Agent account is blocked by " +
                            "Quickbet,please try withdraw amount to your bank account!"
                        };
                        return View(redeemLottery);
                    }
                }
                else
                {
                    redeemLottery.ErrorMessage = new Error()
                    {
                        ErrorCode = (int)ReturnCode.Failed,
                        ErrorMessage = "Invalid action!"
                    };
                    return View(redeemLottery);

                }
            }
            catch (Exception ex)
            {
                redeemLottery.ErrorMessage = new Error()
                {
                    ErrorCode = (int)ReturnCode.Failed,
                    ErrorMessage = ex.Message
                };
                return View(redeemLottery);
            }
        }

        [HttpPost]
        public IActionResult WiningBetRedeem(RedeemLotteryAmountModel redeemLotteryAmountModel)
        {
            try
            {
                if (redeemLotteryAmountModel == null)
                {
                    redeemLotteryAmountModel = new RedeemLotteryAmountModel();

                }
                if (redeemLotteryAmountModel.ErrorMessage == null)
                {
                    redeemLotteryAmountModel.ErrorMessage = new Error();
                }
                var playwin = db.Playwins.Include("Agent").Where(a => a.Id == redeemLotteryAmountModel.winingId && a.ExternalPlayerIdUserId == userSession.Id).FirstOrDefault();
                if (playwin != null && playwin.Agent != null)
                {
                    if (playwin.Agent.UserStatus == (int)UserStatus.active)
                    {
                        redeemLotteryAmountModel.winingId = playwin.Id;
                        redeemLotteryAmountModel.AgentName = playwin.Agent.Name;
                        redeemLotteryAmountModel.AgentNumber = playwin.Agent.CountryCode + "-" + playwin.Agent.ContactNo;
                        redeemLotteryAmountModel.AgentId = playwin.Agent.Id;
                        var balance = PaymentDb.GetClosingBalance(userSession.Id);
                        if (redeemLotteryAmountModel.Amount > playwin.JackpotAmount)
                        {
                            redeemLotteryAmountModel.ErrorMessage = new Error()
                            {
                                ErrorCode = (int)ReturnCode.Failed,
                                ErrorMessage = "Amount should be equal or less then winning amount!"
                            };
                        }
                        else
                        {
                            if (balance > redeemLotteryAmountModel.Amount)
                            {
                                if (PaymentDb.DeductMoneyFromPlayerWalletWrtRedeem(redeemLotteryAmountModel.Amount, userSession.Id,
                                    playwin.Id))
                                {
                                    playwin.PaidMoneyStatus = (int)WinPaidMoneyStatus.Paid_Request_Initiated;
                                    playwin.WithDrawAmount = redeemLotteryAmountModel.Amount;
                                    db.SaveChanges();
                                    redeemLotteryAmountModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, 
                                        ErrorMessage = Applicationstring.Amounttransferredsuccessfully };

                                }
                                else
                                {
                                    redeemLotteryAmountModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.Fundnottransferingerror };
                                }
                            }
                            else
                            {
                                redeemLotteryAmountModel.ErrorMessage = new Error()
                                {
                                    ErrorCode = (int)ReturnCode.Failed,
                                    ErrorMessage = Applicationstring.InsufficientbalanceInWallet
                                };
                            }
                        }

                    }
                    else
                    {
                        redeemLotteryAmountModel.ErrorMessage = new Error()
                        {
                            ErrorCode = (int)ReturnCode.Failed,
                            ErrorMessage = "Agent account is blocked by " +
                            "Quickbet,please try withdraw amount to your bank account!"
                        };
                    }
                }
                else
                {
                    redeemLotteryAmountModel.ErrorMessage = new Error()
                    {
                        ErrorCode = (int)ReturnCode.Failed,
                        ErrorMessage = "Invalid action!"
                    };
                }

            }
            catch (Exception ex)
            {
                redeemLotteryAmountModel.ErrorMessage = new Error()
                {
                    ErrorCode = (int)ReturnCode.Failed,
                    ErrorMessage = ex.Message
                };
            }
            TempData["ActionResponse"] = redeemLotteryAmountModel;
            return RedirectToAction("FundtransferReciept");
        }
        public IActionResult FundtransferReciept()
        {
            RedeemLotteryAmountModel fundTransferModel = new RedeemLotteryAmountModel();
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    fundTransferModel = (RedeemLotteryAmountModel)TempData["ActionResponse"];
                    return View(fundTransferModel);
                }
                else
                {
                    fundTransferModel.ErrorMessage = new Error()
                    {
                        ErrorCode = (int)ReturnCode.Failed,
                        ErrorMessage = Applicationstring.somethingwentwrong
                    };
                    return View(fundTransferModel);
                }
            }
            catch (Exception ex)
            {
                fundTransferModel = new RedeemLotteryAmountModel();
                fundTransferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
                return View(fundTransferModel);
            }
        }

        //[HttpPost]
        //public JsonResult AproveDenyPayment(int WinId, int Action)
        //{
        //    ApiResponse resp = new ApiResponse();
        //    resp.Code = (int)ApiResponseCode.fail;
        //    try
        //    {
        //        var winobj = db.Playwins.Where(a => a.Id == WinId && a.PaidMoneyStatus == (int)WinPaidMoneyStatus.Paid_By_Agent_Waiting_User_Aproval).FirstOrDefault();
        //        var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
        //        if (winobj != null)
        //        {
        //            if (Action == 1)
        //            {
        //                //when user aproved paymen then credit commison to agent wallet
        //                winobj.PaidMoneyStatus = (int)WinPaidMoneyStatus.Paid_By_Agent_Aproved_By_User;
        //                db.SaveChanges();
        //                int AgentId = Convert.ToInt32(winobj.AgentId);
        //                var agentobj = db.Users.Where(a => a.Id == AgentId).FirstOrDefault();
        //                var cashbackamount = Convert.ToDecimal(winobj.WithDrawAmount * agentobj.AgentCashBackOnPayment * Convert.ToDecimal(.01));
        //                AgentCustomerDbOperation.CreditMoneyToAgentWalletWinGamePayment(cashbackamount, AgentId, winobj.Id, winobj.transactionId);
        //                resp.Code = (int)ApiResponseCode.ok;
        //                resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
        //                return Json(resp, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                PaymentDb.ReveresMoneyFromPlayerWalletWrtRedeem(winobj.WithDrawAmount, userSession.Id, winobj.Id);
        //                winobj.PaidMoneyStatus = (int)WinPaidMoneyStatus.NotPaid;
        //                db.SaveChanges();
        //                resp.Code = (int)ApiResponseCode.ok;
        //                resp.Msg = Applicationstring.Requestcanceledsuccessfully;
        //                return Json(resp, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else
        //        {
        //            resp.Msg = "Invalid Lottery!";
        //            return Json(resp, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        resp.Msg = ex.Message;
        //        return Json(resp, JsonRequestBehavior.AllowGet);
        //    }
        //}


        [HttpPost]
        public JsonResult AproveDenyPayment([FromBody] AproveDenyPayment aproveDenyPayment)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                var winobj = db.WalletTransactions.Where(x => x.Id == aproveDenyPayment.WalletId).FirstOrDefault();
                //var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                if (winobj != null)
                {
                    if (aproveDenyPayment.Action == 1)
                    {
                        winobj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                        winobj.LastUpdated = DateTime.UtcNow;
                        int AgentId = Convert.ToInt32(winobj.Agentid);
                        var agentobj = db.Users.Where(a => a.Id == AgentId).FirstOrDefault();
                        var TransferAmount = winobj.Amount;
                        if (agentobj.AgentCashBackOnPayment > 0 && agentobj != null)
                        {
                            // decimal total = agentobj.AgentCashBackOnPayment != 0 ? agentobj.AgentCashBackOnPayment : Convert.ToDecimal( ApplicationVariable.defaultagentcashback);
                            var cashbackamount = Convert.ToDecimal(winobj.Amount * agentobj.AgentCashBackOnPayment * Convert.ToDecimal(.01));

                            var cashback = AgentCustomerDbOperation.CreditMoneyToAgentWalletForCashWidhrwal(cashbackamount, AgentId, winobj.TransactionId, TransferAmount);
                            if (cashback)
                            {
                                var ActualyMOney = AgentCustomerDbOperation.CreditCustomerMoneyToAgentWalletWhenCashWidhrawal(AgentId, winobj.TransactionId, TransferAmount);
                                if (ActualyMOney)
                                {

                                    if (agentobj.ParentAgentId != null)
                                    {
                                        var SuperAgentId = db.Users.Where(a => a.Id == agentobj.ParentAgentId).FirstOrDefault();
                                        if (SuperAgentId != null)
                                        {
                                            if (SuperAgentId.SuperAgentCashBack > 0)
                                            {
                                                //decimal TotalMoney = SuperAgentId.AgentCommison != 0 ? SuperAgentId.AgentCommison : ApplicationVariable.defaultSuperAgentCommission;
                                                var SuperagerntCashbackAmount = Convert.ToDecimal(cashbackamount * SuperAgentId.SuperAgentCashBack * Convert.ToDecimal(.01));
                                                SuperAgentCommission.CreditCommisionMoneyToSuperAgentWalletOnCashwidrwal(SuperagerntCashbackAmount, SuperAgentId.Id, winobj.TransactionId);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (db.SaveChanges() > 0)
                        {
                            resp.Code = (int)ApiResponseCode.ok;
                            resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
                        }
                        return Json(resp);
                    }
                    else if (aproveDenyPayment.Action == 2)
                    {
                        var response = PaymentDb.ReverseMoneyToWalletAmountRejectedBySelf(winobj.Amount, userSession.Id, winobj.TransactionId);

                        if (response)
                        {
                            resp.Code = (int)ApiResponseCode.ok;
                            resp.Msg = Applicationstring.Requestcanceledsuccessfully;
                        }
                        return Json(resp);
                    }
                    else
                    {
                        var response = PaymentDb.ReverseMoneyToWalletAmountRejectedcaseCustomerToAgent(winobj.Amount, userSession.Id, winobj.TransactionId);

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

        // Post:UpdateUserProfile 
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(IFormCollection collection)
        {
            //getting form data
            string fullname = collection["fullname"];
            string displayname = collection["displayname"];
            string emailaddress = collection["emailaddress"];
            string phonenumber = collection["PhoneNumber"];
            string CountryCode = collection["CountryCode"];
            phonenumber = phonenumber.Replace(" ", String.Empty);
            phonenumber = CountryCode + phonenumber;
            try
            {
                QuickbetDbEntities db1 = new QuickbetDbEntities();
                var countuser = db1.Users.Where(a => a.Id != userSession.Id && a.Email == emailaddress.Trim()).Count();
                if (countuser == 0)
                {
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                        if (userobj != null)
                        {
                            if (Request.Form.Files.Count > 0)
                            {
                                for (int i = 0; i < Request.Form.Files.Count; i++)
                                {
                                   
                                    string uploadsFolder = webHostEnvironment.WebRootPath+"/Content/Images";
                                    //getting file name and combine with path and save it
                                    var file = Request.Form.Files[i];
                                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                    }
                                    //save folder path 
                                    userobj.ProfilePicture = "/Content/Images/" + filename;
                                }
                            }
                            userobj.Name = fullname;
                            userobj.DisplayName = displayname;
                            userobj.Email = emailaddress;
                            userobj.ContactNo = phonenumber;
                            db.SaveChanges();
                            userSession.email = userobj.Email;
                            userSession.name = userobj.Name;
                            userSession.phone = userobj.ContactNo;
                            userSession.displayname = userobj.DisplayName;
                            userSession.profilepicture = userobj.ProfilePicture;
                            TempData["ModelError"] = "profile updated successfully";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ModelError"] = "user not found!";
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ViewData["error"] = "email already exist please try with diffrent one!";
                    return View("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["ModelError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult UpdatePassword(IFormCollection collection)
        {
            //getting form data

            try
            {
                string currentpassword = collection["cuurentpassword"];
                string newpassword = collection["newpassword"];
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (userobj != null && userobj.Password == currentpassword.Trim())
                    {
                        userobj.Password = newpassword;
                        db.SaveChanges();
                        TempData["ModelError"] = "Password changed successfully!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ModelError"] = "Your current password is wrong";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ModelError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult ReferParticalView()
        {
            UserSignUpViewModel model = new UserSignUpViewModel();
            try
            {
                model.Id = userSession.Id;
            }
            catch (Exception ex)
            {


            }
            return PartialView("_ReferParticalView", model);
        }

        [HttpPost]

        public IActionResult UpdateReferCode(UserSignUpViewModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (model.ReferCode != null)
                {
                    bool IsAnyRefercodeFound = db.Users.Any(x => x.ReferCode.ToLower().Trim() == model.ReferCode.ToLower().Trim());

                    if (IsAnyRefercodeFound == true)
                    {
                        var Found = db.Users.Where(x => x.ReferCode.ToLower().Trim() == model.ReferCode.ToLower().Trim()).FirstOrDefault();
                        var Percentage = db.UserReferDetails.Where(x => x.Id == 1).FirstOrDefault();
                        var ReferTable = db.ReferUserMappings.Where(x => x.ReferedUserId == userSession.Id).FirstOrDefault();
                        if (ReferTable == null)
                        {
                            ReferUserMapping refer = new ReferUserMapping()
                            {
                                ReferCode = model.ReferCode,
                                Percentage = Percentage.ReferPercentage,
                                ReferedDate = DateTime.UtcNow,
                                TimePeriod = Percentage.ReferPeriods.ToString(),
                                ReferedFromId = Found.Id,
                                ReferedUserId = userSession.Id,

                            };
                            db.ReferUserMappings.Add(refer);
                            var updaterefer = db.Users.Where(x => x.Id == userSession.Id).FirstOrDefault();

                            if (db.SaveChanges() > 0)
                            {
                                response.Code = (int)ApiResponseCode.ok;
                                TempData["ModelError"] = "Successfully Updated";


                            }
                        }
                        else
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            TempData["ModelError"] = "Something Went wrong!";

                        }

                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        TempData["ModelError"] = "Entered Refercode doesnot found!";

                    }
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    TempData["ModelError"] = "Invalid entries, Please try again later!";

                }
            }
            catch (Exception ex)
            {
                TempData["ModelError"] = ex;


            }

            return RedirectToAction("Index", "UserDashboard");
        }

        //public JsonResult CancelReferCode()
        //{
        //    ApiResponse response = new ApiResponse();
        //    try
        //    {
        //        var user = db.Users.Where(x => x.Id == userSession.Id).FirstOrDefault();

        //        if (user != null)
        //        {
        //            user.IsReferShowed = 1;
        //            db.SaveChanges();
        //            response.Code = (int)ApiResponseCode.ok;
        //        }
        //        else
        //        {
        //            response.Code = (int)ApiResponseCode.fail;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return Json(response,JsonRequestBehavior.AllowGet);
        //}

    }
}
