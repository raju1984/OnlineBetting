using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class PlayerRollBack
    {
        public int Id { get; set; }
        public int? ExternalPlayerId { get; set; }
        public int? AgentCustomerId { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string RoundId { get; set; }
        public string GameId { get; set; }
        public string GameSessionId { get; set; }
        public string ExternalSessionId { get; set; }
        public string TransactionId { get; set; }
        public string RolledBackTransactionId { get; set; }
        public string Status { get; set; }
        public DateTime InsertAt { get; set; }
    }
}
