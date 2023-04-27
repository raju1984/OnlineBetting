using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Data
{
    public class UpdateBankDetail
    {
        public int bnkId { get; set; }
        public int type { get; set; }
    }
    public class AddBankDetail
    {
        public string accoutname { get; set; }
        public string accountnumber { get; set; }
        public string bankname { get; set; }
    }
    public class CustomerRefer
    {
        public static bool CreditMoneyToReferAgent(decimal amount, int UserId, int BetId, string TransactionId)
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
                    obj.TransferType = (int)WalletTransactionType.ReferCommission_onBet;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    //Debit
                    obj.ClosingBalance = ClosingBalance + amount;
                    obj.Amount = amount;
                    obj.PlayerBetId = BetId;
                    obj.TransactionRemark = "Amount:₦" + amount + " credit to your wallet w.r.t Refer Commission earned , Bet Id :" + BetId + " Credit Amount Transaction Id:" + TransactionId;
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

        public static void InsertEntryinCashBackTable(int UserId,decimal betAmount)
        {
            try
            {
                using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                {
                    var cashBack = dbConn.CashBackOffersTransactions.Where(a => a.UserId == UserId
                          && a.IsCreditToWallet == false).OrderBy(a => a.CreatedAt).FirstOrDefault();
                    if(cashBack!=null)
                    {
                        var sum = cashBack.AmountSpendToUnlock + betAmount;
                        if(sum> cashBack.AmountToUnlockCashback)
                        {
                            cashBack.IsCreditToWallet = true;
                            cashBack.RedeemAt = DateTime.UtcNow;
                            cashBack.AmountSpendToUnlock = cashBack.AmountToUnlockCashback;
                        }
                        else
                        {
                            cashBack.AmountSpendToUnlock = cashBack.AmountSpendToUnlock + betAmount;
                        }
                        dbConn.SaveChanges();
                        if(cashBack.AmountSpendToUnlock>= cashBack.AmountToUnlockCashback)
                        {
                            PaymentDb.CreditCashBackAmount(cashBack.CashBackAmount, cashBack.UserId, cashBack.Id);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

    }
}
