using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace QuickBetCore.DatabaseEntity
{
    public partial class QuickbetDbEntities : DbContext
    {
        public QuickbetDbEntities()
        {
        }

        public QuickbetDbEntities(DbContextOptions<QuickbetDbEntities> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountGroup> AccountGroups { get; set; }
        public virtual DbSet<AccountGroupObjectMapping> AccountGroupObjectMappings { get; set; }
        public virtual DbSet<AccountGroupUserMapping> AccountGroupUserMappings { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AgentCustomer> AgentCustomers { get; set; }
        public virtual DbSet<AgentPromotionEntry> AgentPromotionEntries { get; set; }
        public virtual DbSet<BankDetail> BankDetails { get; set; }
        public virtual DbSet<Bannermanage> Bannermanages { get; set; }
        public virtual DbSet<CashBackOffersTransaction> CashBackOffersTransactions { get; set; }
        public virtual DbSet<ContactDetail> ContactDetails { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<DisputeChatHistory> DisputeChatHistories { get; set; }
        public virtual DbSet<DisputeTicked> DisputeTickeds { get; set; }
        public virtual DbSet<Gamelist> Gamelists { get; set; }
        public virtual DbSet<HacksawgamingUserTokenMap> HacksawgamingUserTokenMaps { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Picturecarousel> Picturecarousels { get; set; }
        public virtual DbSet<PlayerBet> PlayerBets { get; set; }
        public virtual DbSet<PlayerRollBack> PlayerRollBacks { get; set; }
        public virtual DbSet<Playwin> Playwins { get; set; }
        public virtual DbSet<RandomBonu> RandomBonus { get; set; }
        public virtual DbSet<ReferUserMapping> ReferUserMappings { get; set; }
        public virtual DbSet<SupportChatHistory> SupportChatHistories { get; set; }
        public virtual DbSet<SupportTicked> SupportTickeds { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserOffer> UserOffers { get; set; }
        public virtual DbSet<UserReferDetail> UserReferDetails { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-F893MF9\\SQLEXPRESS;Initial Catalog=QuickbetNewStg;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AccountGroup>(entity =>
            {
                entity.ToTable("AccountGroup");

                entity.Property(e => e.AccountName).HasMaxLength(50);
            });

            modelBuilder.Entity<AccountGroupObjectMapping>(entity =>
            {
                entity.ToTable("AccountGroupObjectMapping");

                entity.HasOne(d => d.AccountGroup)
                    .WithMany(p => p.AccountGroupObjectMappings)
                    .HasForeignKey(d => d.AccountGroupId)
                    .HasConstraintName("FK_AccountGroupObjectMapping_AccountGroup");
            });

            modelBuilder.Entity<AccountGroupUserMapping>(entity =>
            {
                entity.ToTable("AccountGroupUserMapping");

                entity.HasOne(d => d.AccountGroup)
                    .WithMany(p => p.AccountGroupUserMappings)
                    .HasForeignKey(d => d.AccountGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountGroupUserMapping_AccountGroup");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountGroupUserMappings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountGroupUserMapping_Users");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.Emailaddress)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.ZipCode).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Address_Users");
            });

            modelBuilder.Entity<AgentCustomer>(entity =>
            {
                entity.ToTable("AgentCustomer");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.AgentCustomerAgents)
                    .HasForeignKey(d => d.AgentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgentCustomer_Users");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AgentCustomerUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgentCustomer_Users1");
            });

            modelBuilder.Entity<AgentPromotionEntry>(entity =>
            {
                entity.Property(e => e.InsertedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<BankDetail>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.BankDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_BankDetails_Users");
            });

            modelBuilder.Entity<Bannermanage>(entity =>
            {
                entity.ToTable("bannermanage");

                entity.Property(e => e.GoogleplayLink).IsRequired();

                entity.Property(e => e.PlaystoreLink).IsRequired();
            });

            modelBuilder.Entity<CashBackOffersTransaction>(entity =>
            {
                entity.ToTable("CashBackOffersTransaction");

                entity.Property(e => e.AmountSpendToUnlock).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AmountToUnlockCashback).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CashBackAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CashPercentValue).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.RedeemAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CashBackOffersTransactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CashBackOffersTransaction_Users");
            });

            modelBuilder.Entity<ContactDetail>(entity =>
            {
                entity.ToTable("ContactDetail");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.Id).HasMaxLength(50);
            });

            modelBuilder.Entity<DisputeChatHistory>(entity =>
            {
                entity.ToTable("DisputeChatHistory");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Meesage).IsRequired();

                entity.HasOne(d => d.DisputeTicket)
                    .WithMany(p => p.DisputeChatHistories)
                    .HasForeignKey(d => d.DisputeTicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisputeChatHistory_DisputeTicked");

                entity.HasOne(d => d.GenerateByUser)
                    .WithMany(p => p.DisputeChatHistories)
                    .HasForeignKey(d => d.GenerateByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisputeChatHistory_Users");
            });

            modelBuilder.Entity<DisputeTicked>(entity =>
            {
                entity.ToTable("DisputeTicked");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.DisputeTicketId).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DisputeTickeds)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DisputeTicked_Users");
            });

            modelBuilder.Entity<Gamelist>(entity =>
            {
                entity.ToTable("Gamelist");

                entity.Property(e => e.GameId).IsRequired();

                entity.Property(e => e.GameName).IsRequired();

                entity.Property(e => e.JackpotAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TicketCost).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<HacksawgamingUserTokenMap>(entity =>
            {
                entity.ToTable("HacksawgamingUserTokenMap");

                entity.Property(e => e.Createdtime).HasColumnType("datetime");

                entity.Property(e => e.Token).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.HacksawgamingUserTokenMaps)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HacksawgamingUserTokenMap_Users");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.CashBackPercent).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.GatewayName).IsRequired();

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentRquri).HasColumnName("PaymentRQURI");

                entity.Property(e => e.PaymentRsuri).HasColumnName("PaymentRSURI");

                entity.Property(e => e.TransactionId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payments_Users");
            });

            modelBuilder.Entity<Picturecarousel>(entity =>
            {
                entity.ToTable("Picturecarousel");
            });

            modelBuilder.Entity<PlayerBet>(entity =>
            {
                entity.ToTable("PlayerBet");

                entity.Property(e => e.AgentCommison).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.Currency).HasColumnName("currency");

                entity.Property(e => e.CustomerReferCommison).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CustomerReferId).HasColumnName("CustomerReferID");

                entity.Property(e => e.ExternalPlayerIdUserId).HasColumnName("externalPlayerId_UserId");

                entity.Property(e => e.ExternalSessionId).HasColumnName("externalSessionId");

                entity.Property(e => e.GameId)
                    .IsRequired()
                    .HasColumnName("gameId");

                entity.Property(e => e.GameName).HasColumnName("gameName");

                entity.Property(e => e.GameSessionId).HasColumnName("gameSessionId");

                entity.Property(e => e.Insertdate).HasColumnType("datetime");

                entity.Property(e => e.RoundId)
                    .IsRequired()
                    .HasColumnName("roundId");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.PlayerBetAgents)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_PlayerBet_AgentCustomer");

                entity.HasOne(d => d.ExternalPlayerIdUser)
                    .WithMany(p => p.PlayerBetExternalPlayerIdUsers)
                    .HasForeignKey(d => d.ExternalPlayerIdUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlayerBet_Users");
            });

            modelBuilder.Entity<PlayerRollBack>(entity =>
            {
                entity.ToTable("PlayerRollBack");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.Currency).HasColumnName("currency");

                entity.Property(e => e.ExternalPlayerId).HasColumnName("externalPlayerId");

                entity.Property(e => e.ExternalSessionId).HasColumnName("externalSessionId");

                entity.Property(e => e.GameId).HasColumnName("gameId");

                entity.Property(e => e.GameSessionId).HasColumnName("gameSessionId");

                entity.Property(e => e.InsertAt).HasColumnType("datetime");

                entity.Property(e => e.RolledBackTransactionId).HasColumnName("rolledBackTransactionId");

                entity.Property(e => e.RoundId).HasColumnName("roundId");

                entity.Property(e => e.Status).IsRequired();

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");
            });

            modelBuilder.Entity<Playwin>(entity =>
            {
                entity.ToTable("Playwin");

                entity.Property(e => e.AgentPaymentCashback).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BetAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("betAmount");

                entity.Property(e => e.CampaignId).HasColumnName("campaignId");

                entity.Property(e => e.Currency).HasColumnName("currency");

                entity.Property(e => e.ExternalPlayerIdUserId).HasColumnName("externalPlayerId_UserId");

                entity.Property(e => e.ExternalSessionId).HasColumnName("externalSessionId");

                entity.Property(e => e.FreeRoundActivationId).HasColumnName("freeRoundActivationId");

                entity.Property(e => e.FreeRoundsRemaining).HasColumnName("freeRoundsRemaining");

                entity.Property(e => e.GameId).HasColumnName("gameId");

                entity.Property(e => e.GameName).HasColumnName("gameName");

                entity.Property(e => e.GameSessionId).HasColumnName("gameSessionId");

                entity.Property(e => e.InsertAt).HasColumnType("datetime");

                entity.Property(e => e.JackpotAmount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("jackpotAmount");

                entity.Property(e => e.OfferId).HasColumnName("offerId");

                entity.Property(e => e.RoundId).HasColumnName("roundId");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.WinnerCode).HasColumnName("winnerCode");

                entity.Property(e => e.WithDrawAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.PlaywinAgents)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_Playwin_AgentCustomer");

                entity.HasOne(d => d.ExternalPlayerIdUser)
                    .WithMany(p => p.PlaywinExternalPlayerIdUsers)
                    .HasForeignKey(d => d.ExternalPlayerIdUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Playwin_Users");
            });

            modelBuilder.Entity<RandomBonu>(entity =>
            {
                entity.Property(e => e.WinAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<ReferUserMapping>(entity =>
            {
                entity.ToTable("ReferUserMapping");

                entity.Property(e => e.Percentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ReferCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.ReferedDate).HasColumnType("datetime");

                entity.Property(e => e.TimePeriod)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<SupportChatHistory>(entity =>
            {
                entity.ToTable("SupportChatHistory");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Meesage).IsRequired();

                entity.HasOne(d => d.GenerateByUser)
                    .WithMany(p => p.SupportChatHistories)
                    .HasForeignKey(d => d.GenerateByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupportChatHistory_Users");

                entity.HasOne(d => d.SupportTicket)
                    .WithMany(p => p.SupportChatHistories)
                    .HasForeignKey(d => d.SupportTicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupportChatHistory_SupportTicked");
            });

            modelBuilder.Entity<SupportTicked>(entity =>
            {
                entity.ToTable("SupportTicked");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.SupportTicketId).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SupportTickeds)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupportTicked_Users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.AgentCashBackOnPayment).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AgentCommison).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.MyWalletbalance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReferCode)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.SuperAgentCashBack).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserOffer>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserOffers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserOffers_Users");
            });

            modelBuilder.Entity<UserReferDetail>(entity =>
            {
                entity.Property(e => e.ReferPercentage).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WalletTransaction>(entity =>
            {
                entity.ToTable("WalletTransaction");

                entity.Property(e => e.AdminUserId).HasColumnName("Admin_UserId");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Barcode).HasMaxLength(50);

                entity.Property(e => e.ClosingBalance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdated).HasColumnType("datetime");

                entity.Property(e => e.NameOnBank).HasColumnName("Name_on_Bank");

                entity.Property(e => e.PlayerUserId).HasColumnName("Player_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WalletTransactions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_WalletTransaction_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
