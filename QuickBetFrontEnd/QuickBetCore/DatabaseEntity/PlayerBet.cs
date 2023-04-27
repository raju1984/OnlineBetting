using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class PlayerBet
    {
        public int Id { get; set; }
        public int ExternalPlayerIdUserId { get; set; }
        public int? AgentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string RoundId { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public string GameSessionId { get; set; }
        public string ExternalSessionId { get; set; }
        public string TransactionId { get; set; }
        public decimal AgentCommison { get; set; }
        public DateTime Insertdate { get; set; }
        public decimal? CustomerReferCommison { get; set; }
        public int? CustomerReferId { get; set; }

        public virtual User Agent { get; set; }
        public virtual User ExternalPlayerIdUser { get; set; }
    }
}
