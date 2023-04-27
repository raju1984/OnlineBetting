using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string CurrencyId { get; set; }
        public int UserId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentGatewayReferenceId { get; set; }
        public string OrderNo { get; set; }
        public string GatewayName { get; set; }
        public string GatewayRemark { get; set; }
        public string PaymentRquri { get; set; }
        public string PaymentRsuri { get; set; }
        public string CallbackResponse { get; set; }
        public string PaymentGatewayStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime LastUpdate { get; set; }

        public virtual User User { get; set; }
    }
}
