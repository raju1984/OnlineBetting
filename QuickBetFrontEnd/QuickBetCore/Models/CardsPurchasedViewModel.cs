using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models
{
    public class BetViewModel
    {
        public int Id { get; set; }
        public int Player_UserId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerEmail { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string gameId { get; set; }
        public string GameName { get; set; }
        public string transactionId { get; set; }
        public DateTime Insertdate { get; set; }
    }

    public class WinViewModel
    {
        public int Id { get; set; }
        public int Player_UserId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerEmail { get; set; }
        public decimal betamount { get; set; }
        public decimal winamount { get; set; }
        public decimal withdrawamount { get; set; }
        public string currency { get; set; }
        public string gameId { get; set; }
        public string GameName { get; set; }
        public string type { get; set; }
        public int freeRoundsRemaining { get; set; }
        public string transactionId { get; set; }
        public DateTime Insertdate { get; set; }
        public int PaidMoneyStatus { get; set; }
        public int? jackpotType { get; set; }
        public string JackpotText { get; set; }
        public bool? IsJackAprovebyAdmin { get; set; }
        public string JackPotImage { get; set; }
    }
}
