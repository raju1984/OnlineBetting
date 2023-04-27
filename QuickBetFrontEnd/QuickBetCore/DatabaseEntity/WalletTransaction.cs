using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class WalletTransaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal ClosingBalance { get; set; }
        public int TransType { get; set; }
        public string TransactionId { get; set; }
        public int? PaymentId { get; set; }
        public int TransferType { get; set; }
        public int Status { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? UserId { get; set; }
        public int? PlayerUserId { get; set; }
        public int? AdminUserId { get; set; }
        public string PlayerEmail { get; set; }
        public string NameOnBank { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public int? PlayerBetId { get; set; }
        public int? PlayerWinId { get; set; }
        public string TransactionRemark { get; set; }
        public string Note { get; set; }
        public int? Agentid { get; set; }
        public string Barcode { get; set; }

        public virtual User User { get; set; }
    }
}
