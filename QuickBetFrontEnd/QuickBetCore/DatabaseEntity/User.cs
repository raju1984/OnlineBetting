using System;
using System.Collections.Generic;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class User
    {
        public User()
        {
            AccountGroupUserMappings = new HashSet<AccountGroupUserMapping>();
            Addresses = new HashSet<Address>();
            AgentCustomerAgents = new HashSet<AgentCustomer>();
            AgentCustomerUsers = new HashSet<AgentCustomer>();
            BankDetails = new HashSet<BankDetail>();
            CashBackOffersTransactions = new HashSet<CashBackOffersTransaction>();
            DisputeChatHistories = new HashSet<DisputeChatHistory>();
            DisputeTickeds = new HashSet<DisputeTicked>();
            HacksawgamingUserTokenMaps = new HashSet<HacksawgamingUserTokenMap>();
            Payments = new HashSet<Payment>();
            PlayerBetAgents = new HashSet<PlayerBet>();
            PlayerBetExternalPlayerIdUsers = new HashSet<PlayerBet>();
            PlaywinAgents = new HashSet<Playwin>();
            PlaywinExternalPlayerIdUsers = new HashSet<Playwin>();
            SupportChatHistories = new HashSet<SupportChatHistory>();
            SupportTickeds = new HashSet<SupportTicked>();
            UserOffers = new HashSet<UserOffer>();
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime? Dob { get; set; }
        public int Gender { get; set; }
        public int UserType { get; set; }
        public int UserStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ProfilePicture { get; set; }
        public string CountryCode { get; set; }
        public string ContactNo { get; set; }
        public int LoginDevice { get; set; }
        public int CurrencyId { get; set; }
        public int? ParentUserId { get; set; }
        public decimal MyWalletbalance { get; set; }
        public decimal AgentCommison { get; set; }
        public decimal AgentCashBackOnPayment { get; set; }
        public int CustomerRetentionPeriod { get; set; }
        public int? ParentAgentId { get; set; }
        public string ReferCode { get; set; }
        public decimal? SuperAgentCashBack { get; set; }
        public bool? IsVerified { get; set; }

        public virtual ICollection<AccountGroupUserMapping> AccountGroupUserMappings { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<AgentCustomer> AgentCustomerAgents { get; set; }
        public virtual ICollection<AgentCustomer> AgentCustomerUsers { get; set; }
        public virtual ICollection<BankDetail> BankDetails { get; set; }
        public virtual ICollection<CashBackOffersTransaction> CashBackOffersTransactions { get; set; }
        public virtual ICollection<DisputeChatHistory> DisputeChatHistories { get; set; }
        public virtual ICollection<DisputeTicked> DisputeTickeds { get; set; }
        public virtual ICollection<HacksawgamingUserTokenMap> HacksawgamingUserTokenMaps { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<PlayerBet> PlayerBetAgents { get; set; }
        public virtual ICollection<PlayerBet> PlayerBetExternalPlayerIdUsers { get; set; }
        public virtual ICollection<Playwin> PlaywinAgents { get; set; }
        public virtual ICollection<Playwin> PlaywinExternalPlayerIdUsers { get; set; }
        public virtual ICollection<SupportChatHistory> SupportChatHistories { get; set; }
        public virtual ICollection<SupportTicked> SupportTickeds { get; set; }
        public virtual ICollection<UserOffer> UserOffers { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
