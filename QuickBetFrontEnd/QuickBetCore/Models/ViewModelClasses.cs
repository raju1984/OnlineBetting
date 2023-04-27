using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuickBetCore.Models
{
    public class GamePricingViewModel
    {
        public int GameId { get; set; }
        public bool IsAdd { get; set; }
        public int GameCateId { get; set; }
        public long TicketCost { get; set; }
        public List<GamePricingModel> gamePricings { get; set; }
    }
    public class GamePricingModel
    {
        public int Id { get; set; }
        public int NoOfTicket { get; set; }
        public int NoOfSoldTicket { get; set; }
        public decimal WinAmount { get; set; }
        public decimal Payout { get; set; }
      
    }
    public class GameMiddleWareModel
    {
        public string GameId { get; set; }
        public string customerId { get; set; }
    }
    public class CustomerUploadModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*"), MaxLength(30)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }


        [Required(ErrorMessage = "*"), MaxLength(30)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }


        // [Required(ErrorMessage = "*"), MaxLength(12)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ProfilePicture { get; set; }


        [Required(ErrorMessage = "*"), MinLength(10), MaxLength(14)]
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "*")]
        public int Agentype { get; set; }

    }

    public class FundTransferModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Type Of Transfer")]
        public int TypeOfTransfer { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Amount")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string transactionId { get; set; }
    }
    public class ReferAndEarnViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceName = "ValidMessageEmailAddress", ErrorMessageResourceType = typeof(Applicationstring))]
        public string Email { get; set; }
        [Required]
        public string ReferCode { get; set; }

        public List<ReferAndEarList> Refer { get; set; }
    }

    public class ReferAndEarList
    {
        public string Email { get; set; }

        public string ReferedUsername { get; set; }

        public DateTime ReferedDate { get; set; }

        public decimal Commission { get; set; }
    }
    public class UserDashboardViewModel
    {
        public decimal winning { get; set; }
        public decimal balance { get; set; }
        public decimal Cashbackbalance { get; set; }
        public List<FundTransferVModel> transactions { get; set; }
        public decimal CashBack { get; set; }
        public string CashbackText { get; set; }
        public bool isInitail { get; set; }
    }

    public class UserDataModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string token { get; set; }
        public string displayname { get; set; }
        public string profilepicture { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class PlayGameViewModel
    {
        public string gameurl { get; set; }
        public decimal cashBack { get; set; }
    }

    public class GameViewModel
    {
        public int Id { get; set; }
        public string gameid { get; set; }
        public string gamename { get; set; }
        public string gameimg { get; set; }
        public bool enable { get; set; }
        public int CateId { get; set; }
        public string CateName { get; set; }
        public string GameCaption { get; set; }
        public decimal JackPot { get; set; }
        public decimal TicketCost { get; set; }
        public bool DeletedFromGeneGame { get; set; }
    }

    public class CarouselViewModel
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; }
        public string Title { get; set; }
        public string ButtonName { get; set; }
        public string ButtonLink { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public List<CarouselViewModel> pictures { get; set; }
    }
    public class ManageAccountGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DropDownKeyValue> Pages { get; set; }
        public List<DropDownKeyValue> UserPermissonPages { get; set; }
    }

    public class UserManagmentIndexViewModel
    {
        public List<AccountGroupViewModel> Accounts { get; set; }
        public List<UserManageModel> Users { get; set; }
    }
    public class AccountGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DropDownKeyValue> Permission { get; set; }
    }

    public class UserManageModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountType { get; set; }
        public int Status { get; set; }
        public List<DropDownKeyValue> AccountGp { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public decimal totaluser { get; set; }
        public decimal aprovepending { get; set; }
        public decimal Transferedfund { get; set; }
        public decimal Disputed { get; set; }

        public decimal RefundAmont { get; set; }
        public decimal Commisonearned { get; set; }

        public decimal Agents { get; set; }
        public decimal TotalAgents { get; set; }
        public decimal BlockedAgent { get; set; }
        public decimal ShopAgent { get; set; }
        public decimal mobileAgent { get; set; }
        public decimal TotalActiveNationallotteryUser { get; set; }
        public decimal BlockedNationallotteryUser { get; set; }

        public decimal AgentToSuperAgentPendingList { get; set; }
        public decimal AgentToSuperAgentList { get; set; }
        public bool isAprove { get; set; }

        public bool IsEntryAvailable { get; set; }

        public decimal TotalAgentsEarning { get; set; }

        public decimal SuperAgentBalance { get; set; }

    }
    public class SupportChatHistoryModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string GenerateByName { get; set; }
        public int GeneratedById { get; set; }
        public int UserType { get; set; }
        public string profile { get; set; }
        public DateTime Created { get; set; }
    }
    public class SupportTicketViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string SupportTicketId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string userEmail { get; set; }
        public string UserProfilePic { get; set; }
        public List<SupportChatHistoryModel> Chats { get; set; }
    }

    public class RefundMoneyViewModel
    {
        public List<DropDownKeyValue> Users { get; set; }
        public List<TransactionHistoryViewModel> Refundtransaction { get; set; }
    }

    public class RefundMoneyToUserViewModel
    {
        [Required]
        public int ToUserId { get; set; }
        [Required]
        public decimal amount { get; set; }
        public string Message { get; set; }
        public Error error { get; set; }
    }
    public class AdminCommisonViewModel
    {
        public decimal Commsion { get; set; }
        public List<TransactionHistoryViewModel> Commsiontransaction { get; set; }
    }
    public class TransactionHistoryViewModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal FinalBalanceAmount { get; set; }
        public int TransactionType { get; set; }
        public int TransactionStatus { get; set; }
        public string Remark { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class FundTransferiew
    {
        public decimal balance { get; set; }

        public List<FundTransferVModel> pending { get; set; }
        public List<FundTransferVModel> approve { get; set; }
        public List<FundTransferVModel> cancelled { get; set; }

    }
    public class FundTransferVModel
    {
        public int Id { get; set; }
        public DateTime Transferdate { get; set; }
        public decimal amount { get; set; }
        public string transactionid { get; set; }
        public string name { get; set; }
        public string remark { get; set; }
        public string description { get; set; }

        public string accountname { get; set; }
        public string accountnumber { get; set; }
        public string bankname { get; set; }
        public string gamename { get; set; }
    }
    public class AddresViewModel
    {
        public List<BillingShippingAddressViewModel> billingaddress { get; set; }
        public List<BillingShippingAddressViewModel> shippingaddress { get; set; }
    }
    public class BillingShippingAddressViewModel
    {
        public int Id { get; set; }

        [Required]
        public string fullname { get; set; }

        public List<DropDownKeyValue> Countrys { get; set; }

        [Required(ErrorMessage = "*")]
        public string BillingEmail { get; set; }

        [Required(ErrorMessage = "*")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "*")]
        public string Street { get; set; }

        [Required(ErrorMessage = "*")]
        public string StreetNO { get; set; }

        [Required(ErrorMessage = "*")]
        public string City { get; set; }

        [Required(ErrorMessage = "*")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "*")]
        public string CountryCode { get; set; }
    }
    public class DropDownKeyValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<DropDownKeyValue> ChildSubPage { get; set; }
        public bool IsAssign { get; set; }
    }
    public class KeyValuePairModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class Error
    {
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
    public class Response
    {
        public string ErrorMessage { get; set; }
        public int Code { get; set; }
    }
    public class MailModelViewModel
    {
        public string From
        {
            get;
            set;
        }
        public string To
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }
    }
    public class LoginViewModel
    {
        [EmailAddress]
        public string email { get; set; }
        [Phone]
        public string phone { get; set; }

        public string CountryCode { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string password { get; set; }
        public string hfparam { get; set; }
        public Error ErrorMessage { get; set; }
    }

    public class UserSignUpViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMessageFullName", ErrorMessageResourceType = typeof(Applicationstring))]
        public string name { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceName = "ValidMessageEmailAddress", ErrorMessageResourceType = typeof(Applicationstring))]
        public string email { get; set; }


        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMessagePassword", ErrorMessageResourceType = typeof(Applicationstring), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMessagePassword", ErrorMessageResourceType = typeof(Applicationstring), MinimumLength = 6)]

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessageResourceName = "CompareMessageIfPasswordMatch", ErrorMessageResourceType = typeof(Applicationstring))]
        public string repearpassword { get; set; }

        //[Required(ErrorMessage = "Field is required"), MinLength(10), MaxLength(14)]
        [Display(Name = "Contact Number")]
        [Phone]
        public string phone { get; set; }
        [Required]
        public string CountryCode { get; set; }

        public Error ErrorMessage { get; set; }

        public string ReferCode { get; set; }

        public bool IsVerified { get; set; }

    }

    public class AgentSignUpViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMessageFullName", ErrorMessageResourceType = typeof(Applicationstring))]
        public string name { get; set; }


        //[Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceName = "ValidMessageEmailAddress", ErrorMessageResourceType = typeof(Applicationstring))]
        public string email { get; set; }


        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMessagePassword", ErrorMessageResourceType = typeof(Applicationstring), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMessagePassword", ErrorMessageResourceType = typeof(Applicationstring), MinimumLength = 6)]

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessageResourceName = "CompareMessageIfPasswordMatch", ErrorMessageResourceType = typeof(Applicationstring))]
        public string repearpassword { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "phone")]
        [Phone]
        public string phone { get; set; }

        public string CountryCode { get; set; }

        public Error ErrorMessage { get; set; }

        public int AgentType { get; set; }
    }
    public class UserListViewModel
    {
        public int Id { get; set; }
        public string displayname { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string countrycode { get; set; }
        public string phone { get; set; }
        public int status { get; set; }
        public string profile { get; set; }
        public decimal walletbalance { get; set; }
        public decimal quickbetcommision { get; set; }
        public decimal cashback { get; set; }
        public int CustomerRetentionPeriod { get; set; }
        public int UserStatus { get; set; }
        public int UserType { get; set; }

        public string ParentAgentId { get; set; }

        public decimal AgentsEarnings { get; set; }
    }

    public class BankdetailViewModel
    {
        public int Id { get; set; }
        public string bankname { get; set; }
        public string accountname { get; set; }
        public string accountnumer { get; set; }
        public bool isdefault { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "*")]
        public String Email { get; set; }
        [Required(ErrorMessage = "*")]
        public string GeneratrdPass { get; set; }
        [Required(ErrorMessage = "*")]
        public string EnterGenetedPass { get; set; }
        [Required(ErrorMessage = "*")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "*")]
        public string ConfirmNewPassword { get; set; }
    }
    public class dashboard : ApiResponse
    {
        public int ActiveUser { get; set; }
        public int BlockUser { get; set; }

        public decimal AgentBalance { get; set; }
    }
    public class MyBal : ApiResponse
    {
        public decimal MyWallet { get; set; }
    }

    public class pendingTransactionsMobileApp
    {
        public int Id { get; set; }
        public string transactionid { get; set; }
        public decimal amount { get; set; }

        public string remark { get; set; }
        public DateTime Transferdate { get; set; }

    }
    public class approveTransactionsMobileApp
    {
        public int Id { get; set; }
        public string transactionid { get; set; }
        public decimal amount { get; set; }

        public string remark { get; set; }
        public DateTime Transferdate { get; set; }

    }

    public class cancelledTransactionsMobileApp
    {
        public int Id { get; set; }
        public string transactionid { get; set; }
        public decimal amount { get; set; }

        public string remark { get; set; }
        public DateTime Transferdate { get; set; }

    }

    public class ManagePagesModel
    {
        public int Id { get; set; }

        public string GooglePlayLink { get; set; }

        public string AppleStoreLink { get; set; }
    }

    public class ManageReferalModal
    {
        [Required(ErrorMessage = "*")]
        public decimal ReferalPercentage { get; set; }
        [Required(ErrorMessage = "*")]
        public int ReferalDays { get; set; }

        public List<ReferAndEarLists> Refer { get; set; }
    }
    public class ReferAndEarLists
    {
        public string Email { get; set; }

        public string ReferedUsername { get; set; }
        public string ReferedFromName { get; set; }

        public DateTime ReferedDate { get; set; }

        public decimal Commission { get; set; }
    }
}