using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


namespace Quickbet.Areas.Agent.Controllers
{
    [TypeFilter(typeof(CheckAgentSessionExpire))]
    public class AgentFundsController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AgentFundsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Agent/AgentFunds
        public ActionResult Index()
        {
            return View();
        }
        //Agent/AgentFunds/AddBankDetail
        public ActionResult ManageBankDetail()
        {
            return View();
        }
        //Agent/AgentFunds/GetBankList
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
        //Agent/AgentFunds/AddBankDetail
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
        public ActionResult Depositfunds()
        {
            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            return View();
        }

        #region TranserFundPlayerAccount

        public ActionResult Fundtransfer(int CustomerId)
        {
            ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            try
            {
                FundTransferModelUser fundTransferModelUser = new FundTransferModelUser();
                var user = db.Users.Where(a => a.Id == CustomerId).FirstOrDefault();
                fundTransferModelUser.countryCode = user.CountryCode;
                fundTransferModelUser.phone = user.ContactNo;
                fundTransferModelUser.email = user.Email;
                fundTransferModelUser.amount = 100;
                return View(fundTransferModelUser);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Depositfunds");
            }

        }

        [HttpPost]
        public ActionResult FundtransferToPlayer(FundTransferModelUser transferModel)
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
                    
                    var resp = AgentCustomerDbOperation.ValidateFundTransferToPlayer(transferModel,userSession.Id);
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
            return View("Fundtransfer", transferModel);
        }

        [HttpPost]
        public ActionResult FundtransferToPlayerConfirmation(FundTransferModelUser modelConfirmationModel)
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
                    var resp = AgentCustomerDbOperation.ValidateFundTransferToPlayer(modelConfirmationModel,userSession.Id);
                    if (resp != null && resp.ErrorMessage != null && resp.ErrorMessage.ErrorCode == (int)ReturnCode.Success)
                    {
                        //sucesss
                        var userobj = db.Users.Where(a => a.Id == resp.UserId).FirstOrDefault();
                        if (PaymentDb.DoTransactionWalletToPlayer(userobj, resp.amount, resp.message, userSession.Id))
                        {
                            userSession.MyWallet = PaymentDb.GetBalance(userSession.Id);
                            HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                            resp.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, ErrorMessage = Applicationstring.Amounttransferredsuccessfully };
                            TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                            return RedirectToAction("FundtransferReciept");
                        }
                        else
                        {
                            modelConfirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, ErrorMessage = Applicationstring.Fundnottransferingerror };
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

        public ActionResult FundtransferReciept()
        {
            FundTransferModelUser fundTransferModel = new FundTransferModelUser();
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    fundTransferModel = JsonConvert.DeserializeObject<FundTransferModelUser>((string)TempData["ActionResponse"]);// (FundTransferModelUser)TempData["ActionResponse"];
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

        #region  withdrawalBalance 
        public ActionResult withdrawalBalance()
        {
            try
            {

                var User = db.Users.Where(x => x.Id == userSession.Id).FirstOrDefault();
                if (User != null)
                {
                    ViewBag.BalanceAmount = User.MyWalletbalance;
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public JsonResult withdrawalBalanceRequest([FromBody] WithdrawModel withdraw)
        {
            MyBal resp = new MyBal();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (withdraw.amount >= 100)
                {
                    if (PaymentDb.RequestWalletAmountRedeemAgentToAdmin(withdraw.amount, userSession.Id))
                    {
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Requests have been made and Please contact to Admin to approve this request.";
                        decimal Bal = PaymentDb.GetBalance(userSession.Id);
                        userSession.MyWallet = Bal;
                        HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                        resp.MyWallet = Bal;
                    }
                    else
                    {
                        resp.Msg = "Sorry we are not able take your request right now ,please try again later!";
                    }

                }
                else
                {
                    resp.Msg = "Please enter valid amount!";
                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }
        #endregion
    }
}