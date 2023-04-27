using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.SuperAgent.Data
{
    public class SuperAgentCommission
    {
        public static bool CreditCommisionMoneyToSuperAgentWallet(decimal amount, int UserId, int BetId, string TransactionId)
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
                    obj.TransferType = (int)WalletTransactionType.SuperAgentCommission_onBet;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Debit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    obj.PlayerBetId = BetId;
                    obj.TransactionRemark = "Amount:₦" + amount + " credit to your wallet w.r.t Commission earned from Agent , Bet Id :" + BetId + " Credit Amount Transaction Id:" + TransactionId;
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

        public static bool CreditCommisionMoneyToSuperAgentWalletOnCashwidrwal(decimal amount, int UserId, string TransactionId)
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
                    obj.TransferType = (int)WalletTransactionType.SuperAgentCommissionOnCustomerCashOut;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Debit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    //obj.PlayerBetId = BetId;
                    obj.TransactionRemark = "Amount:₦" + amount + " credit to your wallet w.r.t Commission earned from Agent on cash widhrawal , Credit Amount Transaction Id:" + TransactionId;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = dbConn.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    dbConn.WalletTransactions.Add(obj);
                    dbConn.SaveChanges();
                    string SincerelyName = "QuickBet Team";
                    String Subject = "Credit Amount";

                    String Body = "Hi " + objuser.Name + ",<br><br>Amount:₦" + amount + " credit to your wallet w.r.t Commission earned from Agent on cash widhrawal , Credit Amount Transaction Id:" + TransactionId +
                    "<br><br>Sincerely,<br>" + SincerelyName;
                    CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                    return true;

                }
            }
            catch (Exception ex)
            {

            }
            return flag;
        }
    }
}
