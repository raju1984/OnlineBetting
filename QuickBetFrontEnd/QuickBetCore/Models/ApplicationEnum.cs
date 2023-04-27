using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuickBetCore.Models
{
    public struct ViewFilters
    {
        public static string Today { get { return "day"; } }
        public static string Week { get { return "w"; } }
        public static string Month { get { return "mm"; } }
        public static string LifeTime { get { return "all"; } }
    }
    public class CommonMetadata
    {
        public static string BaseCurrency = "NGN";
        public static List<KeyValuePairModel> GetviewType()
        {
            List<KeyValuePairModel> result = new List<KeyValuePairModel>();
            result.Add(new KeyValuePairModel { value = ViewFilters.Month, key = "This Month" });
            result.Add(new KeyValuePairModel { value = ViewFilters.Week, key = "This Week" });
            result.Add(new KeyValuePairModel { value = ViewFilters.Today, key = "Today" });
            result.Add(new KeyValuePairModel { value = ViewFilters.LifeTime, key = "LifeTime" });
            return result;
        }
        public static List<DropDownKeyValue> Pages()
        {
            List<DropDownKeyValue> result = new List<DropDownKeyValue>();

            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Dashboard, Name = "Dashboard", });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Customers, Name = "Customers" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Transactions_History, Name = "Transactions History" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Paystack_Transactions, Name = "Paystack Transactions" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Transfer_Approve_Reject_list, Name = "Transfer Approve/Reject list" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Dispute_Management, Name = "Dispute Management" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Support_Management, Name = "Support Management" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.User_Management, Name = "User Management" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Lanading_Page, Name = "Landing Page" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Manage_ContactUs, Name = "Manage ContactUs" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Manage_Game, Name = "Manage Game" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Manage_Commison, Name = "Manage Commison" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Manage_Refund, Name = "Manage Refund" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Card_History, Name = "Card History" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Agent, Name = "Agent Management" });
            result.Add(new DropDownKeyValue { Id = (int)PageObjecType.Nationallottery, Name = "Nationallottery Management" });
            return result;
        }
        //public static List<DropDownKeyValue> GetSubPage(int Type)
        //{
        //    List<DropDownKeyValue> result = new List<DropDownKeyValue>();
        //    if (Type == (int)PageObjecType.Dashboard)
        //    {
        //        result.Add(new DropDownKeyValue { Id = (int)PageObjecType.DashboardChild_1, Name = "DashboardChild_1", ChildSubPage = GetSubPage((int)PageObjecType.DashboardChild_2) });
        //        result.Add(new DropDownKeyValue { Id = (int)PageObjecType.DashboardChild_2, Name = "DashboardChild_2" });
        //    }
        //    else if (Type == (int)PageObjecType.DashboardChild_2)
        //    {
        //        result.Add(new DropDownKeyValue { Id = (int)PageObjecType.DashboardChild_1_1, Name = "DashboardChild_1_1" });
        //    }
        //    return result;
        //}

    }
    public enum OfferType
    {
        RandomBonus,
    }
    public enum CashBackType
    {
        occasionally,
        onboard,
        minimumwithdraw_amount,
        RandomBonus
    }

    public enum WinPaidMoneyStatus
    {
        NotPaid = 0,
        Paid_Request_Initiated = 1,
        Paid_By_Agent_Waiting_User_Aproval = 2,
        Paid_By_Agent_Aproved_By_User = 3,
        Paid_By_Quickbet = 4
    }
    public enum ImageType
    {
        dp
    }

    public enum OpayGateWayStatus
    {
        INITIAL,
        PENDING,
        SUCCESS,
        FAIL,
    }
    public enum PaymentGateWayType
    {
        Paystack,
        Opay
    }
    public enum TableRowstatus
    {
        active = 1,
        delete = 0
    }
    public enum PageObjecType
    {
        Dashboard = 1,
        Customers,
        Transactions_History,
        Paystack_Transactions,
        Transfer_Approve_Reject_list,
        Dispute_Management,
        Support_Management,
        User_Management,
        Lanading_Page,
        Manage_Game,
        Manage_Commison,
        Manage_Refund,
        Manage_ContactUs,
        Card_History,
        Agent,
        Nationallottery
    }
    public enum DisputeType
    {
        dispute_on_wallaet_transaction = 0,
        dispute_on_lottery_ticket = 1,
    }
    public enum disputeTicketStatus
    {
        closebyadmin = 0,
        Open = 1,
        deleted = 2,
        closebyuser = 3,
    }
    public enum supportTicketStatus
    {
        closebyadmin = 0,
        Open = 1,
        deleted = 2,
        closebyuser = 3,
    }
    public enum supportChatStatus
    {
        active = 1,
        deleted = 0,

    }
    public enum WalletTransactionType
    {
        Recived_from_Gateway = 1,
        TranferTo_Player_Account = 2,
        Transfer_To_Account = 3,
        Deducted_Bet = 4,
        Credit_Bet_Win = 5,
        Reverse_Bet = 6,
        Refund_By_Admin = 7,
        Deducted_by_Admin = 8,
        Credited_by_Admin = 9,
        AgentCommission_onBet = 10,
        Cashback_On_WinPrize = 11,
        Withdraw_Jackbot = 12,
        Reverse_Withdraw_Jackbot = 13,
        WithdrawalWalletAmountAgent = 14,
        Reverse_WithdrawAmount_AgentWallet = 15,
        withrawalWalletAmountCustomer = 16,
        Reverse_WithdrawAmount_CustomerWallet = 17,
        RejectedAmountBySelfCustomer = 18,
        ReferCommission_onBet = 19,
        SuperAgentCommission_onBet = 20,
        WithdrawalWalletAmountSuperAgent = 21,
        Reverse_WithdrawAmount_SuperAgentWallet = 22,
        SuperAgentCommissionOnCustomerCashOut = 23,
        RejectedAmountSelfSuperAgent = 24,
        CustomerWithrawalCreditToAgentWallet = 25,
        CashBackAmount=26,
    }
    public enum TransType
    {
        Debit = 0,
        Credit = 1
    }
    public class ApiResponse
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public string txnId { get; set; }

    }

    public enum ApiResponseCode
    {
        ok = 200,
        fail = 201
    }
    public enum PaymentStatusType
    {
        PaymentInitiated = 200,//pending
        PaymentSuccess = 201,//ok//Payment Status 
        PaymentPending = 202,//pending
        PaymentFailed = 203,//failed
        PaymentCancel = 204,//failed,
        PaymentUnknownStatus = 205,//failed,
    }

    public enum WalletTransactionStatusType
    {
        TransactionInitiated = 200,//pending
        TransactionSuccess = 201,//ok//Payment Status 
        TransactionPending = 202,//pending
        TransactionFailed = 203,//failed
        TransactionCancel = 204,//failed,
        TransactionSuccess_butfailedDuring_OurInsertInLocaldb = 204,//failed,
    }
    public enum AddressType
    {
        //usertype in userstable
        Shipping = 0,
        Billing = 1,
        shopAddress = 2,
    }
    public enum ReturnCode
    {
        //usertype in userstable
        Success = 0,
        Failed = 1,
    }
    public enum UserStatus
    {
        //usertype in userstable
        block = 0,
        active = 1,
        Pending_for_approval = 2,
        deleted = 3,
        UserVerificationPending = 4,
    }
    public enum UserType
    {                       //usertype in userstable
        Admin = 0,
        Users = 1,
        Staff = 2,
        Agent = 3,//Agent/Shop Agent
        Nationallottery = 4,
        SuperAgent = 5,
        MobileAgent = 6
    }

    public enum ApprovalStatus
    {
        pendingForapproval = 0,
        approvedbyAdmin = 1,
        rejectedbyAdmin = 2,
        acceptedbyAgent = 3,
        rejectedbyAgent = 4
    }
}