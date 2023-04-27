using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models.MobileAppModel
{
    public class DeleteViewModel
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string password { get; set; }
    }
    public class BalanceViewModel
    {
        public decimal balance { get; set; }
        public decimal cashbackbalance { get; set; }
    }
    public class EmailAddressModelApi
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string email { get; set; }
    }
    public class LoginViewModelApi
    {
        [Required]
        public string email { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string password { get; set; }
    }

    public class UpdatePasswordModelApi
    {
        public int UserId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string newpassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string cuurentpassword { get; set; }
    }

    public class UserSignUpViewModelApi
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMessageFullName", ErrorMessageResourceType = typeof(Applicationstring))]
        public string name { get; set; }

        [Display(Name = "Email Address")]
        [RegularExpression(".+@.+\\..+", ErrorMessageResourceName = "ValidMessageEmailAddress", ErrorMessageResourceType = typeof(Applicationstring))]
        public string email { get; set; }

        
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Display(Name = "CountryCode")]
        public string countrycode { get; set; }

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
        public string ReferCode { get; set; }
    }
    public class ResponseModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public string metadescription { get; set; }
    }
    public class CountryStateMapi
    {
        public List<DropDownKeyValue> country { get; set; }
        public List<DropDownKeyValue> states { get; set; }
    }

    public class BillingShippingAddressApiModel
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        [Required]
        public string fullname { get; set; }
        [Required]
        public string street { get; set; }
        [Required]
        public string streetno { get; set; }
        [Required]
        public string towncity { get; set; }

        public string phone { get; set; }
        [Required]
        public string pincode { get; set; }

        [Required]
        public int AddressType { get; set; }

        public string emailaddress { get; set; }
        public string countrycode { get; set; }
        
    }
    public class WithdrawrequestModel
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public decimal amount { get; set; }
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }
    }

    public class AddBankDetailApi
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string accountname { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string accountnumber { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string bankname { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }
    }

    public class UpdateBankDetailApi
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int bnkId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int type { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }
    }

    public class PlayFundTransferAppModel
    {
        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public decimal amount { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string playermail { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }

    }

    public class AddsupportTicketModel
    {

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string title { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }

    }

    public class AddChatToSupportTicketModel
    {

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int SupportTicketId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }

    }
    public class AdddisputeTicketModel
    {

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string title { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public string message { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int UserId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int referenceId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredFiled", ErrorMessageResourceType = typeof(Applicationstring))]
        public int disputetype { get; set; }

    }
    public class AppTransactionHistory
    {
        public decimal balance { get; set; }
        public List<TransactionHistoryViewModel> transactions { get; set; }
    }
}
