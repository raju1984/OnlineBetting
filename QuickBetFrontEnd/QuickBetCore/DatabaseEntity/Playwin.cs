using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class Playwin
    {
        public int Id { get; set; }
        public int ExternalPlayerIdUserId { get; set; }
        public int? AgentId { get; set; }
        public string Currency { get; set; }
        public string RoundId { get; set; }
        public string GameId { get; set; }
        public string GameName { get; set; }
        public string GameSessionId { get; set; }
        public string ExternalSessionId { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
        public decimal JackpotAmount { get; set; }
        public decimal? BetAmount { get; set; }
        public string FreeRoundActivationId { get; set; }
        public string CampaignId { get; set; }
        public string OfferId { get; set; }
        public int FreeRoundsRemaining { get; set; }
        public DateTime InsertAt { get; set; }
        public decimal WithDrawAmount { get; set; }
        public int PaidMoneyStatus { get; set; }
        public decimal AgentPaymentCashback { get; set; }
        public string BarCode { get; set; }
        public int WinnerCode { get; set; }
        public int? JackpotType { get; set; }
        public bool? IsJackpotWin { get; set; }
        public bool? IsJackAprovebyAdmin { get; set; }
        public string JackpotName { get; set; }
        public string JackpotImage { get; set; }

        public virtual User Agent { get; set; }
        public virtual User ExternalPlayerIdUser { get; set; }
    }
}
