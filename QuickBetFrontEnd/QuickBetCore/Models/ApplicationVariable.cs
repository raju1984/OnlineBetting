using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models
{
    public class ApplicationVariable
    {

        public static string currencysymbol = "₦";
        public static string currencycode = "NGN";
        public static string countrycode = "NG";
        public static string partnerid = "quickbet";
        public static string defaultdp = "/img/dp.jpg";
        public static double defaultagentcommison = 2;//%
        public static double defaultagentcashback = 2;//%
        public static int CustomerRetentionPeriod = 180;//days
        public static int MininumBalance = 1000;
        public static decimal defaultSuperAgentCommission = 5; //% 3.	Need to get 5% of their agents earnings
        public static int DefaultMininumBalanceToWithdraw = 1000;

        public static bool isRelease = false;


        public static string genegamstudiobaseurl = isRelease ? "http://staging.geniegamestudios.com" : "https://localhost:44308";


        public static string AppURL = isRelease ? "http://staging.quickbetng.com" : "https://localhost:44321";

    }
}
