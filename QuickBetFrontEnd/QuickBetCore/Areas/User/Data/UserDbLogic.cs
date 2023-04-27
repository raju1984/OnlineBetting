using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Data
{
    public class UserDbLogic
    {
        public static UserDashboardViewModel GetUserDashboard(int UserId)
        {
            UserDashboardViewModel userDashboard = new UserDashboardViewModel();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    userDashboard.balance= PaymentDb.GetBalance(UserId);
                    userDashboard.Cashbackbalance = PaymentDb.GetCashbackBalance(UserId);
                    var userOffer = db.UserOffers.Where(a => a.UserId == UserId && a.IsRedeem == false).OrderBy(a => a.CreatedAt).FirstOrDefault();
                    if (userOffer != null && userOffer.Value > 0)
                    {
                        userDashboard.CashBack = userOffer.Value;
                    }
                    else
                    {
                        //Onboard CashBack
                        //if first payment
                        var paymentCount = db.Payments.Count(a => a.Status == (int)PaymentStatusType.PaymentSuccess
                                    && a.UserId == UserId);
                        if (paymentCount == 0)
                        {
                            userDashboard.isInitail = true;
                            //check for onboard offers
                            var offer = db.Offers.Where(a => a.IsActive == true &&
                                       a.CashBackType == (int)CashBackType.onboard).FirstOrDefault();
                            if (offer != null && offer.CashBackPercent > 0)
                            {
                                userDashboard.CashBack = offer.CashBackPercent;
                            }
                        }
                        else
                        {
                            //check for occasionally offers
                            var offer = db.Offers.Where(a => a.IsActive == true && a.CashBackType == (int)CashBackType.occasionally).FirstOrDefault();
                            if (offer != null && offer.CashBackPercent > 0)
                            {
                                userDashboard.CashBack = offer.CashBackPercent;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return userDashboard;
        }
    }
}
