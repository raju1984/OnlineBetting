using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.APIAuth;
using QuickBetCore.Models.Data;
using QuickBetCore.Models.MobileAppModel;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [CustomAuthorizeFilter]
    public class ApptransactionController : ControllerBase
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        // Post: api/Apptransaction/WithdrawRequest
        [HttpPost]
        public IActionResult WithdrawRequest(WithdrawrequestModel withdrawrequestModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == withdrawrequestModel.UserId)
                    .OrderByDescending(a => a.InsertDate).FirstOrDefault();
                decimal ClosingBalance = 0;
                if (objWalletTransactions != null)
                {
                    ClosingBalance = objWalletTransactions.ClosingBalance;
                }
                var minumumAmount = PaymentDb.GetMinimumAmountToWithdraw();
                if(withdrawrequestModel.amount >= minumumAmount)
                {
                    if (withdrawrequestModel.amount > 99 && withdrawrequestModel.UserId > 0)
                    {
                        if (withdrawrequestModel.amount <= ClosingBalance)
                        {
                            var bankdetailobj = db.BankDetails.Where(a => a.UserId == withdrawrequestModel.UserId).ToList();
                            if (bankdetailobj != null && bankdetailobj.Count() > 0)
                            {
                                var defaultaccount = bankdetailobj.Where(a => a.IsDefault == true && a.Isdeleted == false).FirstOrDefault();
                                if (defaultaccount != null)
                                {
                                    if (PaymentDb.DoTransactionWalletToBank(defaultaccount, withdrawrequestModel.amount,
                                        withdrawrequestModel.UserId))
                                    {
                                        resp.code = (int)ApiResponseCode.ok;
                                        resp.message = "Your request submitted successfully";
                                    }
                                    else
                                    {
                                        resp.message = "Sorry we are not able take your request right now ,please try again later!";
                                    }
                                }
                                else
                                {
                                    resp.message = "Please set default Bank for transfering money!";
                                }
                            }
                            else
                            {
                                resp.message = "Please configure Bank detail for transfering money!";
                            }
                        }
                        else
                        {
                            resp.message = "insufficient balance to transfer bank account";
                        }

                    }
                    else
                    {
                        resp.message = "Please enter valid amount!";
                    }
                }
                else
                {
                    resp.message = "Please enter valid amount!,enter amount greater or equal to ₦" + minumumAmount;
                }
                
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }


        // Post: api/Apptransaction/WithdrawRequest
        [HttpPost]
        public IActionResult FundTransferToPlayerAccount(PlayFundTransferAppModel playFundTransferAppModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (playFundTransferAppModel.amount > 99)
                {
                    var userobj = db.Users.Where(a => a.Email == playFundTransferAppModel.playermail.Trim()
                    && a.UserStatus == (int)UserStatus.active).FirstOrDefault();
                    if (userobj == null)
                    {
                        resp.message = "Player does not exist ,please check again";
                        return Ok(resp);
                    }
                    if (playFundTransferAppModel.UserId == userobj.Id)
                    {
                        resp.message = "Invalid Email";
                        return Ok(resp);
                    }
                    var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == playFundTransferAppModel.UserId)
                        .OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }

                    if (playFundTransferAppModel.amount >= ClosingBalance)
                    {
                        resp.message = "insufficient balance to transfer bank account";
                        return Ok(resp);
                    }
                    if (userobj != null)
                    {
                        if (PaymentDb.DoTransactionWalletToPlayer(userobj, playFundTransferAppModel.amount,
                            playFundTransferAppModel.message, playFundTransferAppModel.UserId))
                        {
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = "Amount transferred successfully";
                        }
                        else
                        {
                            resp.message = "Sorry we are not able take your request right now ,please try again later!";
                        }
                    }
                    else
                    {
                        resp.message = "Please configure Bank detail for transfering money!";
                    }
                }
                else
                {
                    resp.message = "Please enter valid amount!";
                }
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }



        // GET: api/Apptransaction/GetAlltransactionrequest?UserId=0
        public IActionResult GetAlltransactionrequest(int UserId, string viewType = "mm")
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            FundTransferiew result = new FundTransferiew();
            try
            {
                result.balance = PaymentDb.GetBalance(UserId);
                result.pending = (from r in db.WalletTransactions
                                  where r.Status == (int)WalletTransactionStatusType.TransactionPending &&
                                  r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                  && r.UserId == UserId
                                  select new FundTransferVModel
                                  {
                                      Id = r.Id,
                                      transactionid = r.TransactionId,
                                      amount = r.Amount,
                                      remark = r.Note,
                                      description = r.TransactionRemark,
                                      Transferdate = r.InsertDate
                                  }).OrderByDescending(a => a.Transferdate).ToList();
                result.approve = (from r in db.WalletTransactions
                                  where r.Status == (int)WalletTransactionStatusType.TransactionSuccess &&
                                  r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                  && r.UserId == UserId
                                  select new FundTransferVModel
                                  {
                                      Id = r.Id,
                                      transactionid = r.TransactionId,
                                      amount = r.Amount,
                                      remark = r.Note,
                                      description = r.TransactionRemark,
                                      Transferdate = r.InsertDate
                                  }).OrderByDescending(a => a.Transferdate).ToList();

                result.cancelled = (from r in db.WalletTransactions
                                    where r.Status == (int)WalletTransactionStatusType.TransactionCancel &&
                                    r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                                    && r.UserId == UserId
                                    select new FundTransferVModel
                                    {
                                        Id = r.Id,
                                        transactionid = r.TransactionId,
                                        amount = r.Amount,
                                        remark = r.Note,
                                        description = r.TransactionRemark,
                                        Transferdate = r.InsertDate
                                    }).OrderByDescending(a => a.Transferdate).ToList();
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
                resp.code = (int)ApiResponseCode.fail;
            }
            resp.data = result;
            return Ok(resp);
        }

        // GET: api/Apptransaction/GetWiningtransaction?UserId=0&&viewType=
        public IActionResult GetWiningtransaction(int UserId, string viewType = "monthe")
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            List<FundTransferVModel> transaction = new List<FundTransferVModel>();
            UserDashboardViewModel result = new UserDashboardViewModel();
            try
            {
                var Playwins = db.Playwins.Where(a => a.ExternalPlayerIdUserId == UserId).Select(a => a.Id).ToList();
                List<int?> ids = new List<int?>();
                if (Playwins != null && Playwins.Count() > 0)
                {
                    foreach (var item in Playwins)
                    {
                        ids.Add(item);
                    }
                }
                result.winning = db.WalletTransactions.Where(a => ids.Contains(a.PlayerWinId) && a.UserId == UserId
                 && a.Status == (int)WalletTransactionStatusType.TransactionSuccess).Sum(x => (decimal?)(x.Amount)) ?? 0;

                result.transactions = DbOperation.Getwintransaction(UserId, viewType);
                result.balance = PaymentDb.GetBalance(UserId);
                transaction = DbOperation.Getwintransaction(UserId, viewType);
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
                resp.code = (int)ApiResponseCode.fail;
            }
            resp.data = result;
            return Ok(result);
        }

        // GET: api/Apptransaction/Getallbankaccounts?UserId=0
        public IActionResult Getallbankaccounts(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            List<BankdetailViewModel> model = new List<BankdetailViewModel>();
            try
            {
                model = (from r in db.BankDetails
                         where r.Isdeleted == false && r.UserId == UserId
                         select new BankdetailViewModel
                         {
                             Id = r.Id,
                             bankname = r.BankName,
                             accountname = r.AccountName,
                             accountnumer = r.AccountNumber,
                             isdefault = r.IsDefault
                         }).ToList();
            }
            catch (Exception ex)
            {

            }
            resp.data = model;
            return Ok(resp);
        }


        // Post: api/Apptransaction/AddBankDetail
        [HttpPost]
        public IActionResult AddBankDetail(AddBankDetailApi addBankDetailApi)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(addBankDetailApi.accountname) && !string.IsNullOrEmpty(addBankDetailApi.accountnumber)
                    && !string.IsNullOrEmpty(addBankDetailApi.bankname))
                {
                    BankDetail objdetail = new BankDetail();
                    objdetail.AccountName = addBankDetailApi.accountname;
                    objdetail.BankName = addBankDetailApi.bankname;
                    objdetail.AccountNumber = addBankDetailApi.accountnumber;
                    objdetail.UserId = addBankDetailApi.UserId;
                    int conunt = db.BankDetails.Where(a => a.UserId == addBankDetailApi.UserId).Count();
                    if (conunt == 0)
                    {
                        objdetail.IsDefault = true;
                    }
                    db.BankDetails.Add(objdetail);
                    db.SaveChanges();
                    resp.code = (int)ApiResponseCode.ok;
                    resp.message = Applicationstring.Success;
                }
                else
                {
                    resp.message = "Invalid Model!";
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }


        // Post: api/Apptransaction/UpdateBankDetail
        [HttpPost]
        public IActionResult UpdateBankDetail(UpdateBankDetailApi updateBankDetailApi)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            resp.metadescription = "type:{1=for setting bank default,0=for deleting bank }";
            try
            {
                if (updateBankDetailApi.type == 1 && updateBankDetailApi.bnkId > 0)
                {
                    var banklist = db.BankDetails.Where(a => a.UserId == updateBankDetailApi.UserId).ToList();
                    if (banklist != null && banklist.Where(a => a.Id == updateBankDetailApi.bnkId).Count() > 0)
                    {
                        foreach (var item in banklist)
                        {
                            if (item.Id == updateBankDetailApi.bnkId)
                            {
                                item.IsDefault = true;
                            }
                            else
                            {
                                item.IsDefault = false;
                            }
                        }
                    }
                    resp.code = (int)ApiResponseCode.ok;
                    resp.message = Applicationstring.Success;
                    db.SaveChanges();
                }
                else if (updateBankDetailApi.type == 0 && updateBankDetailApi.bnkId > 0)
                {
                    var bankobj = db.BankDetails.Where(a => a.Id == updateBankDetailApi.bnkId).FirstOrDefault();
                    if (bankobj != null)
                    {
                        bankobj.Isdeleted = true;
                        db.SaveChanges();
                        resp.code = (int)ApiResponseCode.ok;
                        resp.message = Applicationstring.Success;
                    }
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }


        // GET: api/Apptransaction/GetTransactionHistory?UserId=0
        public IActionResult GetTransactionHistory(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            resp.metadescription = "TransactionStatus:{TransactionInitiated = 200,//pending"
                + "TransactionSuccess = 201,//ok success" + "TransactionPending = 202,//pending"
                + "TransactionFailed = 203,//failed "
                + "TransactionCancel = 204,//failed,"
                + "TransactionSuccess_butfailedDuring_OurInsertInLocaldb = 204,//failed, }"
                + "TransactionType:{Debit=0,Credit = 1}";
            AppTransactionHistory model = new AppTransactionHistory();
            try
            {
                model.transactions = (from r in db.WalletTransactions
                                      where r.UserId == UserId
                                      select new TransactionHistoryViewModel
                                      {
                                          Id = r.Id,
                                          TransactionId = r.TransactionId,
                                          Date = r.InsertDate,
                                          Amount = r.Amount,
                                          FinalBalanceAmount = r.ClosingBalance,
                                          TransactionType = r.TransType,
                                          TransactionStatus = r.Status,
                                          Remark = r.TransactionRemark
                                      }).OrderByDescending(a => a.Date).ToList();
                model.balance = PaymentDb.GetBalance(UserId);
            }
            catch (Exception ex)
            {

            }
            resp.data = model;
            return Ok(resp);
        }
    }
}
