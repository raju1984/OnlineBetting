using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Agent.Data
{
    public class PayLotteryModel
    {
        public int WinId { get; set; }
    }
    public class SupportTicketModel
    {
        public string title { get; set; } 
        public string message { get; set; }
    }
    public class AproveDenyPayment
    {
        public int WalletId { get; set; }
        public int Action { get; set; }
    }
    public class WithdrawModel
    {
        [Required]
        public decimal amount { get; set; }
    }
    public class AgentCustomerDbOperation
    {
        public static FundTransferModelUser ValidateFundTransferToPlayer(FundTransferModelUser fundTransferModel,int UserId)
        {
            FundTransferModelUser confirmationModel = new FundTransferModelUser();
            confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.somethingwentwrong };
            try
            {
                if (fundTransferModel.amount >= 100)
                {
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        QuickBetCore.DatabaseEntity.User userobj = new QuickBetCore.DatabaseEntity.User();
                        if (!string.IsNullOrEmpty(fundTransferModel.email))
                        {
                            string email = fundTransferModel.email.Trim().ToLower();
                            userobj = db.Users.Where(a => a.Email == email
                           && (a.UserType == (int)UserType.Users || a.UserType == (int)UserType.Agent)
                           && a.UserStatus == (int)UserStatus.active).FirstOrDefault();
                        }
                        if (!string.IsNullOrEmpty(fundTransferModel.phone))
                        {
                            string ContactNo = fundTransferModel.phone.Trim().Replace(" ", String.Empty).ToLower();
                            userobj = db.Users.Where(a => a.ContactNo == ContactNo
                           && (a.UserType == (int)UserType.Users || a.UserType == (int)UserType.Agent)
                           && a.UserStatus == (int)UserStatus.active).FirstOrDefault();
                        }
                        if (userobj != null)
                        {
                            var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == UserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                            decimal ClosingBalance = 0;
                            if (objWalletTransactions != null)
                            {
                                ClosingBalance = objWalletTransactions.ClosingBalance;
                            }
                            decimal finalbalance = ClosingBalance - fundTransferModel.amount;
                            if (fundTransferModel.amount >= ClosingBalance)
                            {
                                confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.Insufficientbalance };
                            }
                            else
                            {
                                //return confirmation page
                                confirmationModel.UserId = userobj.Id;
                                confirmationModel.name = userobj.Name;
                                confirmationModel.email = userobj.Email;
                                confirmationModel.phone = userobj.ContactNo;
                                confirmationModel.countryCode = userobj.CountryCode;
                                confirmationModel.amount = fundTransferModel.amount;
                                confirmationModel.message = fundTransferModel.message;
                                confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, ErrorMessage = "" };
                            }
                        }
                        else
                        {
                            confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.PlayerNotFound };
                        }
                    }
                }
                else
                {
                    confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.Pleaseentervalidamount };
                }
            }
            catch (Exception ex)
            {
                confirmationModel.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
            }
            return confirmationModel;
        }
        public static bool CreditCommisionMoneyToAgentWallet(decimal amount, int UserId, int BetId, string TransactionId)
        {

            bool flag = false;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var objWalletTransactions = dbConn.WalletTransactions.Where(a => a.UserId == UserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.TransactionId = Guid.NewGuid().ToString();
                    obj.UserId = UserId;
                    obj.TransferType = (int)WalletTransactionType.AgentCommission_onBet;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Debit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    obj.PlayerBetId = BetId;
                    obj.TransactionRemark = "Amount:₦" + amount + " credit to your wallet w.r.t Commission earned , Bet Id :" + BetId + " Credit Amount Transaction Id:" + TransactionId;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    dbConn.WalletTransactions.Add(obj);
                    dbConn.SaveChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }

        public static bool CreditMoneyToAgentWalletWinGamePayment(decimal amount, int UserId, int PlayWinId, string TransactionId)
        {

            bool flag = false;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var objWalletTransactions = dbConn.WalletTransactions.Where(a => a.UserId == UserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.TransactionId = Guid.NewGuid().ToString();
                    obj.UserId = UserId;
                    obj.TransferType = (int)WalletTransactionType.Cashback_On_WinPrize;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Credit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    obj.PlayerWinId = PlayWinId;
                    obj.TransactionRemark = "CashbackAmount:₦" + amount + " credit to your wallet w.r.t to payment made to customer, for scratch card win :" + PlayWinId + " Credit Amount Transaction Id:" + TransactionId;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    dbConn.WalletTransactions.Add(obj);
                    dbConn.SaveChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }

        public static bool CreditMoneyToAgentWalletForCashWidhrwal(decimal amount, int UserId, string TransactionId, decimal TransferAmount)
        {

            bool flag = false;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var objWalletTransactions = dbConn.WalletTransactions.Where(a => a.UserId == UserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.TransactionId = Guid.NewGuid().ToString();
                    obj.UserId = UserId;
                    obj.TransferType = (int)WalletTransactionType.Cashback_On_WinPrize;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Credit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    //obj.PlayerWinId = PlayWinId;
                    obj.TransactionRemark = "CashbackAmount:₦" + amount + "credit to your wallet w.r.t to payment made to customer,Credit Amount Transaction Id:" + TransactionId;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    dbConn.WalletTransactions.Add(obj);
                    if (dbConn.SaveChanges() > 0)
                    {

                        string SincerelyName = "QuickBet Team";
                        String Subject = "Credit amount";

                        String Body = "Hi " + objuser.Name + ",<br><brCashbackAmount:₦" + amount + " & Transfer Amount" + TransferAmount + "credit to your wallet w.r.t to payment made to customer,Credit Amount Transaction Id:" + TransactionId +
                        "<br><br>Sincerely,<br>" + SincerelyName;
                        CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                        return true;
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }

        public static bool CreditCustomerMoneyToAgentWalletWhenCashWidhrawal(int UserId, string TransactionId, decimal TransferAmount)
        {

            bool flag = false;
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var objWalletTransactions = dbConn.WalletTransactions.Where(a => a.UserId == UserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.TransactionId = Guid.NewGuid().ToString();
                    obj.UserId = UserId;
                    obj.TransferType = (int)WalletTransactionType.CustomerWithrawalCreditToAgentWallet;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Credit
                    obj.ClosingBalance = ClosingBalance + TransferAmount;
                    obj.Amount = TransferAmount;
                    //obj.PlayerWinId = PlayWinId;
                    obj.TransactionRemark = "Customer Amount:₦" + TransferAmount + "credit to your wallet w.r.t to payment made to customer,Credit Amount Transaction Id:" + TransactionId;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    dbConn.WalletTransactions.Add(obj);
                    if (dbConn.SaveChanges() > 0)
                    {
                        return true;
                    }


                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }
    }
}
