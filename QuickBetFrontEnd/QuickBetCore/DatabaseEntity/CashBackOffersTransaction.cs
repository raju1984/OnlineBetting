using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class CashBackOffersTransaction
    {
        public int Id { get; set; }
        public decimal AmountToUnlockCashback { get; set; }
        public decimal AmountSpendToUnlock { get; set; }
        public decimal CashBackAmount { get; set; }
        public string Note { get; set; }
        public bool IsCreditToWallet { get; set; }
        public int UserId { get; set; }
        public int CashBackType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RedeemAt { get; set; }
        public decimal? CashPercentValue { get; set; }
        public int? PaymentId { get; set; }
        public int? UserOfferId { get; set; }

        public virtual User User { get; set; }
    }
}
