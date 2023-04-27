using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.User.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Controllers
{
    [TypeFilter(typeof(CheckUserSessionExpire))]
    public class FundsController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FundsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/Funds
        public IActionResult Index()
        {

            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            return View();
        }
        public IActionResult Depositfunds()
        {
            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            return View();
        }

        public IActionResult Withdrawfund()
        {
            WithdrawModel withdraw = new WithdrawModel();
            try
            {
                ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
                withdraw.amount= PaymentDb.GetMinimumAmountToWithdraw();
                ViewData["minumumAmount"]= withdraw.amount;
                if (TempData["actionresponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["actionresponse"]);
                }
            }
            catch (Exception ex)
            {

            }
            return View(withdraw);
        }

        public IActionResult WithdrawfundToAccount(WithdrawModel withdraw)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                var minumumAmount = PaymentDb.GetMinimumAmountToWithdraw();
                var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == userSession.Id).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                decimal ClosingBalance = 0;
                if (objWalletTransactions != null)
                {
                    ClosingBalance = objWalletTransactions.ClosingBalance;
                }
                if(withdraw.amount>= minumumAmount)
                {
                    if (withdraw.amount >= 100)
                    {
                        if (withdraw.amount <= ClosingBalance)
                        {
                            var bankdetailobj = db.BankDetails.Where(a => a.UserId == userSession.Id).ToList();
                            if (bankdetailobj != null && bankdetailobj.Count() > 0)
                            {
                                var defaultaccount = bankdetailobj.Where(a => a.IsDefault == true && a.Isdeleted == false).FirstOrDefault();
                                if (defaultaccount != null)
                                {
                                    if (PaymentDb.DoTransactionWalletToBank(defaultaccount, withdraw.amount, userSession.Id))
                                    {
                                        resp.Code = (int)ApiResponseCode.ok;
                                        resp.Msg = "Requests have been made and will be completed within 24 hours.";
                                    }
                                    else
                                    {
                                        resp.Msg = "Sorry we are not able take your request right now ,please try again later!";
                                    }
                                }
                                else
                                {
                                    resp.Msg = "Please set default Bank for transfering money!";
                                }
                            }
                            else
                            {
                                resp.Msg = "Please configure Bank detail for transfering money!";
                            }
                        }
                        else
                        {
                            resp.Msg = "Insufficient balance to perform this transaction";
                        }
                    }
                    else
                    {
                        resp.Msg = "Please enter valid amount!";
                    }
                }
                else
                {
                    resp.Msg = "Please enter valid amount!,enter amount greater or equal to ₦" + minumumAmount;
                }
                
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            TempData["actionresponse"] = JsonConvert.SerializeObject(resp);
            return RedirectToAction("Withdrawfund");
        }


        public IActionResult GetTransactionRequest(string Type = "pending")
        {
            List<FundTransferVModel> fundTransfers = new List<FundTransferVModel>();
            try
            {
                //result.pending
                if (Type == "pending")
                {
                    fundTransfers = (from r in db.WalletTransactions
                                     where r.Status == (int)WalletTransactionStatusType.TransactionPending &&
                                     r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                     && r.UserId == userSession.Id
                                     select new FundTransferVModel
                                     {
                                         Id = r.Id,
                                         transactionid = r.TransactionId,
                                         amount = r.Amount,
                                         remark = r.Note,
                                         description = r.TransactionRemark,
                                         Transferdate = r.InsertDate
                                     }).OrderByDescending(a => a.Transferdate).ToList();
                    ViewData["Requests"] = "Pending Requests";
                }
                else if (Type == "aproved")
                {
                    fundTransfers = (from r in db.WalletTransactions
                                     where r.Status == (int)WalletTransactionStatusType.TransactionSuccess &&
                                     r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                     && r.UserId == userSession.Id
                                     select new FundTransferVModel
                                     {
                                         Id = r.Id,
                                         transactionid = r.TransactionId,
                                         amount = r.Amount,
                                         remark = r.Note,
                                         description = r.TransactionRemark,
                                         Transferdate = r.InsertDate
                                     }).OrderByDescending(a => a.Transferdate).ToList();
                    ViewData["Requests"] = "Approved Requests";
                }
                else if (Type == "cancelled")
                {
                    fundTransfers = (from r in db.WalletTransactions
                                     where r.Status == (int)WalletTransactionStatusType.TransactionCancel &&
                                     r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                     && r.UserId == userSession.Id
                                     select new FundTransferVModel
                                     {
                                         Id = r.Id,
                                         transactionid = r.TransactionId,
                                         amount = r.Amount,
                                         remark = r.Note,
                                         description = r.TransactionRemark,
                                         Transferdate = r.InsertDate
                                     }).OrderByDescending(a => a.Transferdate).ToList();
                    ViewData["Requests"] = "Cancelled Requests";
                }
                else
                {
                    ViewData["Requests"] = "";
                }
                return PartialView("_PaymentRequest", fundTransfers);
            }
            catch (Exception ex)
            {
                ViewData["Requests"] = "";
                return PartialView("_PaymentRequest", fundTransfers);
            }
        }

        public IActionResult GetAddBankDetail()
        {

            try
            {
                return PartialView("_AddBankDetail");
            }
            catch (Exception ex)
            {

            }
            return PartialView("_AddBankDetail");
        }
        public IActionResult LoadWithdrawAmount()
        {

            try
            {
                ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
                ViewData["minumumAmount"] = PaymentDb.GetMinimumAmountToWithdraw();
                return PartialView("_WithdrawAmount");
            }
            catch (Exception ex)
            {

            }
            return PartialView("_WithdrawAmount");
        }

        #region TranserFundPlayerAccount

        public IActionResult Fundtransfer()
        {
            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            return View(new FundTransferModelUser());
        }
        [HttpPost]
        public IActionResult FundtransferToPlayer(FundTransferModelUser transferModel)
        {
            try
            {
                if (transferModel == null)
                {
                    transferModel = new FundTransferModelUser();

                }
                if (transferModel.ErrorMessage == null)
                {
                    transferModel.ErrorMessage = new Error();
                }


                ModelState.Remove("UserId");
                if (ModelState.IsValid)
                {

                    if (transferModel.email == userSession.email || transferModel.phone == userSession.phone)
                    {
                        transferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.CannotTransferFunds };
                        return View("Fundtransfer", transferModel);
                    }

                    var resp = AgentCustomerDbOperation.ValidateFundTransferToPlayer(transferModel, userSession.Id);
                    if (resp != null && resp.ErrorMessage != null && resp.ErrorMessage.ErrorCode == (int)ReturnCode.Success)
                    {
                        return View("FundtransferToPlayerConfirmation", resp);
                    }
                    else
                    {
                        transferModel.ErrorMessage = resp != null && resp.ErrorMessage != null ? resp.ErrorMessage : new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidModel };
                    }
                }
                else
                {
                    transferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidModel };
                }
            }
            catch (Exception ex)
            {
                transferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
            }
            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            return View("Fundtransfer", transferModel);
        }

        [HttpPost]
        public IActionResult FundtransferToPlayerConfirmation(FundTransferModelUser modelConfirmationModel)
        {
            try
            {
                if (modelConfirmationModel == null)
                {
                    modelConfirmationModel = new FundTransferModelUser();

                }
                if (modelConfirmationModel.ErrorMessage == null)
                {
                    modelConfirmationModel.ErrorMessage = new Error();
                }
                if (ModelState.IsValid)
                {
                    var resp = AgentCustomerDbOperation.ValidateFundTransferToPlayer(modelConfirmationModel, userSession.Id);
                    if (resp != null && resp.ErrorMessage != null && resp.ErrorMessage.ErrorCode == (int)ReturnCode.Success)
                    {
                        //sucesss

                        var userobj = db.Users.Where(a => a.Id == resp.UserId).FirstOrDefault();
                        if (PaymentDb.DoTransactionWalletToPlayer(userobj, resp.amount, resp.message, userSession.Id))
                        {
                            resp.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, ErrorMessage = Applicationstring.Amounttransferredsuccessfully };
                            TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                            return RedirectToAction("FundtransferReciept");
                        }
                        else
                        {
                            modelConfirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.Fundnottransferingerror };
                        }
                    }
                    else
                    {
                        modelConfirmationModel.ErrorMessage = resp != null && resp.ErrorMessage != null ? resp.ErrorMessage : new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidModel };
                    }
                }
                else
                {
                    modelConfirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidModel };
                }
            }
            catch (Exception ex)
            {
                modelConfirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
            }
            return View(modelConfirmationModel);
        }

        public IActionResult FundtransferReciept()
        {
            FundTransferModelUser fundTransferModel = new FundTransferModelUser();
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    //fundTransferModel = (FundTransferModelUser)TempData["ActionResponse"];
                    ViewBag.response = JsonConvert.DeserializeObject<FundTransferModelUser>((string)TempData["ActionResponse"]);
                    return View(fundTransferModel);
                }
                else
                {
                    fundTransferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.somethingwentwrong };
                    return View(fundTransferModel);
                }
            }
            catch (Exception ex)
            {
                fundTransferModel = new FundTransferModelUser();
                fundTransferModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
                return View(fundTransferModel);
            }
        }
        #endregion



        public IActionResult GetBankList()
        {

            List<BankdetailViewModel> model = new List<BankdetailViewModel>();
            try
            {
                model = (from r in db.BankDetails
                         where r.Isdeleted == false && r.UserId == userSession.Id
                         select new BankdetailViewModel
                         {
                             Id = r.Id,
                             bankname = r.BankName,
                             accountname = r.AccountName,
                             accountnumer = r.AccountNumber,
                             isdefault = r.IsDefault
                         }).ToList();
                return PartialView("_BankList", model);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_BankList");
        }
        public JsonResult AddBankDetail([FromBody] AddBankDetail addBank)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(addBank.accoutname) && !string.IsNullOrEmpty(addBank.accountnumber)
                    && !string.IsNullOrEmpty(addBank.bankname))
                {
                    BankDetail objdetail = new BankDetail();
                    objdetail.AccountName = addBank.accoutname;
                    objdetail.BankName = addBank.bankname;
                    objdetail.AccountNumber = addBank.accountnumber;
                    objdetail.UserId = userSession.Id;
                    int conunt = db.BankDetails.Where(a => a.UserId == userSession.Id).Count();
                    if (conunt == 0)
                    {
                        objdetail.IsDefault = true;
                    }
                    db.BankDetails.Add(objdetail);
                    db.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                }
                else
                {
                    resp.Msg = "Invalid Model!";
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });

        }


        public JsonResult UpdateBankDetail([FromBody] UpdateBankDetail updateBank)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (updateBank.type == 1 && updateBank.bnkId > 0)
                {
                    var banklist = db.BankDetails.Where(a => a.UserId == userSession.Id).ToList();
                    if (banklist != null && banklist.Where(a => a.Id == updateBank.bnkId).Count() > 0)
                    {
                        foreach (var item in banklist)
                        {
                            if (item.Id == updateBank.bnkId)
                            {
                                item.IsDefault = true;
                            }
                            else
                            {
                                item.IsDefault = false;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                else if (updateBank.type == 0 && updateBank.bnkId > 0)
                {
                    var bankobj = db.BankDetails.Where(a => a.Id == updateBank.bnkId).FirstOrDefault();
                    if (bankobj != null)
                    {
                        bankobj.Isdeleted = true;
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });

        }


        public IActionResult TransactionHistory(string viewType = "mm")
        {
            List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
            try
            {
                DateTime filterdate = new DateTime();
                if (viewType == ViewFilters.Today)
                {
                    filterdate = DateTime.UtcNow;
                }
                if (viewType == ViewFilters.Week)
                {
                    filterdate = DateTime.UtcNow.AddDays(-7);

                }
                else if (viewType == ViewFilters.Month)
                {
                    filterdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                }

                if (viewType == ViewFilters.LifeTime)
                {
                    result = (from r in db.WalletTransactions
                              where r.UserId == userSession.Id
                              select new TransactionHistoryViewModel
                              {
                                  Id = r.Id,
                                  TransactionId = r.TransactionId,
                                  Date = r.InsertDate,
                                  Amount = r.Amount,
                                  FinalBalanceAmount = r.ClosingBalance,
                                  TransactionType = r.TransType,
                                  TransactionStatus = r.Status,
                                  Remark = r.TransactionRemark,
                                  Note = r.Note
                              }).OrderByDescending(a => a.Date).ToList();
                }
                else
                {
                    result = (from r in db.WalletTransactions
                              where r.UserId == userSession.Id && r.InsertDate.Date >= filterdate.Date
                              select new TransactionHistoryViewModel
                              {
                                  Id = r.Id,
                                  TransactionId = r.TransactionId,
                                  Date = r.InsertDate,
                                  Amount = r.Amount,
                                  FinalBalanceAmount = r.ClosingBalance,
                                  TransactionType = r.TransType,
                                  TransactionStatus = r.Status,
                                  Remark = r.TransactionRemark,
                                  Note = r.Note
                              }).OrderByDescending(a => a.Date).ToList();
                }

                ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }

        //private string GetRemark(WalletTransaction walletTransactions)
        //{
        //    string remark = string.Empty;
        //    try
        //    {
        //        if(walletTransactions!=null && !string.IsNullOrEmpty(walletTransactions.PlayerEmail))
        //        {
        //            remark= remark+ "PlayerEmail :"+ walletTransactions.PlayerEmail+""+ walletTransactions.PlayerEmail
        //        }
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

        #region Withdrawal money from agent
        public IActionResult WithdrawalFromAgent()
        {
            List<SelectListItem> UserList = new List<SelectListItem>();
            // List<UserManageModel> models = new List<UserManageModel>();
            try
            {

                if (TempData["actionresponse"] != null)
                {
                    //ApiResponse response = new ApiResponse();
                    //response = (ApiResponse)TempData["actionresponse"];
                    //ViewBag.response = response;
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["actionresponse"]);
                }

                ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);

                var Users = (from r in db.Users
                             where r.UserType == (int)UserType.Agent && r.UserStatus == (int)UserStatus.active && r.MyWalletbalance >= 1000
                             select new
                             {
                                 Id = r.Id,
                                 Name = r.Name + "~" + r.Email
                             }).ToList();


                UserList.Add(new SelectListItem { Value = "", Text = "---Select Agents---" });

                if (Users != null && Users.Count() > 0)
                {
                    foreach (var item in Users)
                    {

                        UserList.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });

                    }
                }

                if (UserList != null)
                {
                    ViewBag.UserList = UserList;
                }

                //Getting minimum Amount to withrwal
                var ActualBalance = PaymentDb.GetBalance(userSession.Id);
                decimal WithrawalthroughAgent = 0;
                decimal Playwins = 0;

                var ListFirst = db.WalletTransactions.Where(r => r.UserId == userSession.Id &&
                 r.TransType == (int)TransType.Credit && r.TransferType == (int)WalletTransactionType.Credit_Bet_Win).ToList();

                if (ListFirst.Count > 0)
                {
                    Playwins = ListFirst.Sum(q => q.Amount);
                }

                var list = db.WalletTransactions.Where(r => r.UserId == userSession.Id &&
                r.TransType == (int)TransType.Debit && (r.TransferType == (int)WalletTransactionType.Withdraw_Jackbot || r.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer) && (r.Status != (int)WalletTransactionStatusType.TransactionCancel && r.Status != (int)WalletTransactionStatusType.TransactionFailed)).ToList();
                if (list.Count > 0)
                {
                    WithrawalthroughAgent = list.Sum(a => a.Amount);
                }

                var TotalAmount = Playwins - WithrawalthroughAgent;
                if (TotalAmount > 0)
                {
                    if (TotalAmount >= ActualBalance)
                    {
                        ViewBag.PaymentToBeWithrawal = ActualBalance;
                    }
                    else
                    {

                        ViewBag.PaymentToBeWithrawal = TotalAmount;
                    }
                }
                else
                {
                    ViewBag.PaymentToBeWithrawal = 0;
                }

                var WinList = db.WalletTransactions.Where(x => x.UserId == userSession.Id && x.TransferType == (int)WalletTransactionType.Credit_Bet_Win).ToList();
                if (WinList != null)
                {
                    ViewBag.WinList = WinList.Sum(a => a.Amount);
                }
                ViewData["minumumAmount"] = PaymentDb.GetMinimumAmountToWithdraw();
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        [HttpPost]
        public IActionResult FundWidhdawalrequest(withdawalrequestAdminModel model)
        {
            ApiResponse response = new ApiResponse(); decimal DailyAmount = 0;
            try
            {
                DateTime currentDate = DateTime.UtcNow;

                DateTime SeveneDaysBefore = DateTime.Today.AddDays(-7);
                if (ModelState.IsValid)
                {
                    var minumumAmount = PaymentDb.GetMinimumAmountToWithdraw();

                    if(model.Amount>=minumumAmount)
                    {
                        decimal? weekelyAmount = 0;
                        var NineDaysCondition = db.WalletTransactions.Where(x => x.UserId == userSession.Id /*&& x.Agentid == model.AgentId*/
                        && x.InsertDate.Date > SeveneDaysBefore
                        && (x.Status != (int)WalletTransactionStatusType.TransactionFailed && x.Status != (int)WalletTransactionStatusType.TransactionCancel) && x.TransferType == (int)WalletTransactionType.withrawalWalletAmountCustomer && x.TransType == (int)TransType.Debit).ToList();

                        if (NineDaysCondition != null && NineDaysCondition.Count > 0)
                        {
                            weekelyAmount = NineDaysCondition.Sum(a => a.Amount);
                            weekelyAmount = weekelyAmount + model.Amount;
                        }
                        if (weekelyAmount <= 50000)
                        {
                            if (model.AgentAmount >= model.Amount)
                            {
                                if (model.Amount <= 50000)
                                {
                                    var checkDailyLimit = db.WalletTransactions.Where(x => x.UserId == userSession.Id &&
                                   x.InsertDate.Date == currentDate.Date && x.Agentid == model.AgentId && (x.Status != (int)WalletTransactionStatusType.TransactionFailed && x.Status != (int)WalletTransactionStatusType.TransactionCancel)
                                     && x.TransType == (int)TransType.Debit).ToList();
                                    if (checkDailyLimit.Count > 0)
                                    {
                                        DailyAmount = checkDailyLimit.Sum(a => a.Amount);
                                    }

                                    if (DailyAmount < 50000)
                                    {
                                        if (model.MinimumAmount >= model.Amount)
                                        {
                                            if (model.Amount > 0)
                                            {
                                                var balance = PaymentDb.GetClosingBalance(userSession.Id);
                                                if (balance >= model.Amount)
                                                {

                                                    var Check = PaymentDb.RequestWalletAmountRedeemCustomerToAgent(model.Amount, userSession.Id, model.AgentId, model.Note);
                                                    if (Check)
                                                    {
                                                        response.Code = (int)ApiResponseCode.ok;
                                                        response.Msg = "Requests have been made and Please contact to selected Agent to approve the request and pay you the requested funds in cash.";
                                                    }
                                                    else
                                                    {
                                                        response.Code = (int)ApiResponseCode.fail;
                                                        response.Msg = "Sorry we are not able take your request right now ,please try again later!";
                                                    }
                                                }
                                                else
                                                {
                                                    response.Code = (int)ApiResponseCode.fail;
                                                    response.Msg = "Amount should be equal or less then winning amount!";
                                                }
                                            }
                                            else
                                            {
                                                response.Code = (int)ApiResponseCode.fail;
                                                response.Msg = "Amount Could not be 0!";

                                            }
                                        }
                                        else
                                        {
                                            response.Code = (int)ApiResponseCode.fail;
                                            response.Msg = "Amount should be less than or equal to the minumum wallet amount!";
                                        }

                                    }

                                    else
                                    {
                                        response.Code = (int)ApiResponseCode.fail;
                                        response.Msg = "Cannot withrawal amount greater than ₦ 50000! in a day with agent!";
                                    }
                                }
                                else
                                {
                                    response.Code = (int)ApiResponseCode.fail;
                                    response.Msg = "Cannot withrawal amount greater than ₦ 50000";
                                }
                            }
                            else
                            {
                                response.Code = (int)ApiResponseCode.fail;
                                response.Msg = "Sorry agent daily limit exceeded!";
                            }
                        }
                        else
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = "Your weekly limit of ₦ 50000 has been exceeded!";
                            // response.Msg = "Your weekly limit of ₦ 50000 with same agent has been exceeded! Please select another agent!";

                        }
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "Please enter valid amount!,enter amount greater or equal to ₦" + minumumAmount;
                    }
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    response.Msg = "Invalid Model.Please fill all the details!";
                }


            }
            catch (Exception ex)
            {
            }
            TempData["actionresponse"] = JsonConvert.SerializeObject(response);
            return RedirectToAction("WithdrawalFromAgent", "Funds");
        }

        public JsonResult GetAgentpendingAmount(int Id = 0)
        {
            dashboard resp = new dashboard();
            try
            {
                DateTime currentDate = DateTime.UtcNow;
                decimal totalAmount = 0;
                decimal pendingReq = 0;
                decimal ApproveReq = 0;
                if (Id > 0)
                {
                    var agentBal = db.WalletTransactions.Where(x => x.Agentid == Id && (x.Status == (int)WalletTransactionStatusType.TransactionInitiated ||
                    x.Status == (int)WalletTransactionStatusType.TransactionPending
                    ) && x.TransType == (int)TransType.Debit).ToList();


                    if (agentBal != null && agentBal.Count > 0)
                    {
                        pendingReq = agentBal.Sum(x => x.Amount);
                    }

                    var Approverequest = db.WalletTransactions.Where(x => x.Agentid == Id && x.Status == (int)WalletTransactionStatusType.TransactionSuccess && x.TransType == (int)TransType.Debit
                 //&& DbFunctions.TruncateTime(x.LastUpdated) == currentDate
                 && x.LastUpdated.Date == currentDate.Date
                    ).ToList();

                    if (Approverequest != null && Approverequest.Count > 0)
                    {
                        ApproveReq = Approverequest.Sum(x => x.Amount);
                    }
                    totalAmount = pendingReq + ApproveReq;
                    var FinalAmount = 50000 - totalAmount;
                    if (FinalAmount > 0)
                    {
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.AgentBalance = FinalAmount;

                    }
                    else
                    {
                        resp.Code = (int)ApiResponseCode.fail;
                        resp.AgentBalance = FinalAmount = 0;

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return Json(resp);
        }
        #endregion
    }
}
