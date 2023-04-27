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
    public class TransactionHistoryController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        // GET: Admin/TransactionHistory
        public ActionResult History(string trantype)
        {
            List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(trantype) && trantype == "tfsfund")
                {
                    result = (from r in db.WalletTransactions
                              where r.TransferType == (int)WalletTransactionType.Transfer_To_Account
                              //&& (r.TransType == (int)TransType.Credit
                              select new TransactionHistoryViewModel
                              {
                                  TransactionId = r.TransactionId,
                                  Date = r.InsertDate,
                                  Amount = r.Amount,
                                  FinalBalanceAmount = r.ClosingBalance,
                                  TransactionType = r.TransType,
                                  TransactionStatus = r.Status,
                                  Remark = r.TransactionRemark
                              }).OrderByDescending(a => a.Date).ToList();
                }
                else if (!string.IsNullOrEmpty(trantype) && trantype == "disfund")
                {
                    var ids = db.DisputeTickeds.Where(a => a.DisputeType == (int)DisputeType.dispute_on_wallaet_transaction
                && a.Status == (int)supportTicketStatus.Open).Select(a => a.DisputeReferenceId).ToList();
                    if (ids != null && ids.Count() > 0)
                    {
                        ids = ids.Distinct().ToList();
                        result = (from r in db.WalletTransactions
                                  where ids.Contains(r.Id)
                                  select new TransactionHistoryViewModel
                                  {
                                      TransactionId = r.TransactionId,
                                      Date = r.InsertDate,
                                      Amount = r.Amount,
                                      FinalBalanceAmount = r.ClosingBalance,
                                      TransactionType = r.TransType,
                                      TransactionStatus = r.Status,
                                      Remark = r.TransactionRemark
                                  }).OrderByDescending(a => a.Date).ToList();

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return View(trantype);
        }

        public ActionResult GetTransactionHistory(string viewType = "mm")
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
                }
                else
                {
                    result = (from r in db.WalletTransactions
                              where r.InsertDate.Date >= filterdate.Date
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
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_AllTransactionInSysem", result);
        }

        public ActionResult TranHistoryByUser(int customerid)
        {
            List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
            try
            {
                result = (from r in db.WalletTransactions
                          where r.UserId == customerid
                          select new TransactionHistoryViewModel
                          {
                              TransactionId = r.TransactionId,
                              Date = r.InsertDate,
                              Amount = r.Amount,
                              FinalBalanceAmount = r.ClosingBalance,
                              TransactionType = r.TransType,
                              TransactionStatus = r.Status,
                              Remark = r.TransactionRemark
                          }).OrderByDescending(a => a.Date).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }
        public ActionResult PaystackHistory()
        {
            try
            {
               
            }
            catch (Exception ex)
            {

            }
            return View();
        }
        public ActionResult GetPaystackHistory(string viewType = "mm")
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
                    result = (from r in db.Payments
                              where r.GatewayName == PaymentGateWayType.Paystack.ToString()
                              select new TransactionHistoryViewModel
                              {
                                  TransactionId = r.TransactionId,
                                  Date = r.PaymentDate,
                                  Name = r.User.Name + "(" + r.User.Email + ")",
                                  Amount = r.Amount,
                                  TransactionStatus = r.Status,
                              }).OrderByDescending(a => a.Date).ToList();
                }
                else
                {
                    result = (from r in db.Payments
                              where r.GatewayName == PaymentGateWayType.Paystack.ToString()
                              &&   r.PaymentDate.Date >= filterdate.Date
                              select new TransactionHistoryViewModel
                              {
                                  TransactionId = r.TransactionId,
                                  Date = r.PaymentDate,
                                  Name = r.User.Name + "(" + r.User.Email + ")",
                                  Amount = r.Amount,
                                  TransactionStatus = r.Status,
                              }).OrderByDescending(a => a.Date).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_PaystackHistory", result);
        }
        public ActionResult WithDrawRequest()
        {
            try
            {
                
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public ActionResult GetWithDrawRequestHistory(string viewType = "mm")
        {
            List<FundTransferVModel> result = new List<FundTransferVModel>();
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
                              where
                               (r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountAgent 
                               || r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountSuperAgent
                               || r.TransferType == (int)WalletTransactionType.Transfer_To_Account)
                               &&
                              (r.Status == (int)WalletTransactionStatusType.TransactionPending 
                              || r.Status == (int)WalletTransactionStatusType.TransactionInitiated)
                             
                              select new FundTransferVModel
                              {
                                  Id = r.Id,
                                  transactionid = r.TransactionId,
                                  name = r.User.Name + "(" + r.User.Email + ")",
                                  amount = r.Amount,
                                  remark = r.TransactionRemark,
                                  Transferdate = r.InsertDate,
                                  accountname = r.NameOnBank,
                                  accountnumber = r.BankAccountNumber,
                                  bankname = r.BankName
                              }).OrderByDescending(a => a.Transferdate).ToList();
                }
                else
                {

                    result = (from r in db.WalletTransactions
                              where
                              (r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountAgent
                               || r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountSuperAgent
                               || r.TransferType == (int)WalletTransactionType.Transfer_To_Account)
                               &&
                              (r.Status == (int)WalletTransactionStatusType.TransactionPending 
                              || r.Status == (int)WalletTransactionStatusType.TransactionInitiated)
                              && r.InsertDate.Date >= filterdate.Date
                              select new FundTransferVModel
                              {
                                  Id = r.Id,
                                  transactionid = r.TransactionId,
                                  name = r.User.Name + "(" + r.User.Email + ")",
                                  amount = r.Amount,
                                  remark = r.TransactionRemark,
                                  Transferdate = r.InsertDate,
                                  accountname = r.NameOnBank,
                                  accountnumber = r.BankAccountNumber,
                                  bankname = r.BankName
                              }).OrderByDescending(a => a.Transferdate).ToList();

                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_WithDrawRequest", result);
        }

        public JsonResult RejectPaymentRequest(int walletId, string remark)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                var objWalletTransactions = db.WalletTransactions.Where(a => a.Id == walletId).Count();
                if (objWalletTransactions > 0)
                {
                    if (PaymentDb.RejectTransactionWalletToBank(walletId, remark))
                    {
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Transaction cancelled successfully";
                    }
                    else
                    {
                        resp.Msg = "Sorry we are not able take your request right now ,please try again later!";
                    }
                }
                else
                {
                    resp.Msg = "Please configure Bank detail for transfering money!";
                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }
        public JsonResult AcceptPaymentRequest(int walletId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {

                var objWalletTransactions = db.WalletTransactions.Where(a => a.Id == walletId).FirstOrDefault();
                if (objWalletTransactions != null)
                {
                    objWalletTransactions.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    db.SaveChanges();
                }
                else
                {
                    resp.Msg = "Something went wrong please try again later!";
                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        
    }
}