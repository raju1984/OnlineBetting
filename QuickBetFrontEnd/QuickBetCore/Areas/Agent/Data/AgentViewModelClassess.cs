using QuickBetCore.Models;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Agent.Data
{
    public class RedeemLotteryAmountModel
    {
        public decimal Amount { get; set; }
        public int AgentId { get; set; }
        public int winingId { get; set; }
        public string AgentName { get; set; }
        public string AgentNumber { get; set; }
        public Error ErrorMessage { get; set; }

    }
    public class FundTransferModelUser
    {
        [Display(Name = "Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceName = "ValidMessageEmailAddress", ErrorMessageResourceType = typeof(Applicationstring))]
        public string email { get; set; }

        [Display(Name = "Contact Number")]
        public string phone { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, Int32.MaxValue)]
        public decimal amount { get; set; }

        [Required(ErrorMessage = "*")]
        public string message { get; set; }

        public string name { get; set; }
        public string countryCode { get; set; }
        public int UserId { get; set; }
        public Error ErrorMessage { get; set; }
    }

    public class BarCodeViewModel
    {
        public string msg { get; set; }
        public int wincode { get; set; }
        public int Id { get; set; }
        public decimal amount { get; set; }
        public Byte[] barcode { get; set; }
    }
    public class BettingViewModel
    {
        public int Id { get; set; }
        public string agentname { get; set; }
        public string customername { get; set; }
        public string gamename { get; set; }
        public decimal jackpotamount { get; set; }
        public decimal WithdrawrequestAmount { get; set; }
        public int PaidMoneyStatus { get; set; }
        public DateTime datetime { get; set; }
        public decimal betamount { get; set; }
        public decimal quikbetcommision { get; set; }

        public string TransactionId { get; set; }
        public List<WalletAccountWithralRequestModel> wallet { get; set; }
    }
    public class AgentDahsboardViewModel
    {
        public bool isAprove { get; set; }

        public bool IsBalance { get; set; }
        public decimal Balance { get; set; }
        public int NoofCustomer { get; set; }

        public decimal TotalBetAmount { get; set; }
        public decimal TotalWinAmount { get; set; }

        public decimal AgentCommison { get; set; }
        public decimal CashBackonPayment { get; set; }
        public decimal CustomerRetention { get; set; }
        public int UserType { get; set; }
    }
    public class AgentCustomerModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*"), MaxLength(30)]
        [Display(Name = "Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "*"), MaxLength(30)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage = "*"), MinLength(10), MaxLength(14)]
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [StringLength(50, ErrorMessageResourceName = "StringLengthMessagePassword", ErrorMessageResourceType = typeof(Applicationstring), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        public string CountryCode { get; set; }

        public int gameid { get; set; }
    }

    public class WalletAccountWithralRequestModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public int TransactionType { get; set; }

        public string TransactionId { get; set; }

        public string RequestedBy { get; set; }

        public DateTime RequestedDate { get; set; }
        public int ApprovalStatus { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }

    }

    public class withdawalrequestAdminModel
    {
        [Required(ErrorMessage = "*")]
        public int AgentId { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "*")]
        public string Note { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal MinimumAmount { get; set; }
        [Required(ErrorMessage = "*")]
        public decimal AgentAmount { get; set; }
    }
}
