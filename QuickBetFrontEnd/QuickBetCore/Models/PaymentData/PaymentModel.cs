using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models.PaymentData
{
    public class PaymentResultModel
    {
        public string transactionId { get; set; }
        public string message { get; set; }
        public int PaymentStatus { get; set; }
        public decimal amount { get; set; }
        public string paymentUrl { get; set; }
    }
  
    public class paystackRequest
    {
        public string first_name { set; get; }

        public string last_name { set; get; }
        public string phone { set; get; }

        public string email { set; get; }
        public string amount { set; get; }

        public string currency { set; get; }
        public string reference { set; get; }

        public string callback_url { set; get; }
        public custom_field metadata { set; get; }
    }
    public class custom_field
    {
        public custom_fields custom_fields { set; get; }

    }
    public class custom_fields
    {
        public string display_name { set; get; }
        public string variable_name { set; get; }
        public string value { set; get; }

    }
    public class PayStackData
    {
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string reference { get; set; }
    }

    public class PayStackRequestResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PayStackData data { get; set; }
    }
    public class PayStatusResponseExpress
    {
        public string result { get; set; }

        public string result_text { get; set; }
        public string order_id { get; set; }

        public string transaction_id { get; set; }
        public string currency { get; set; }

        public string amount { get; set; }
        public string date_processed { get; set; }
        public string token { get; set; }
    }
    public class PayStackWebHookNotification
    {
        public DateTime sent_at { get; set; }
        public string channel { get; set; }
    }

    public class History
    {
        public string type { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }
    public class PayStackStatusLog
    {
        public int start_time { get; set; }
        public int time_spent { get; set; }
        public int attempts { get; set; }
        public int errors { get; set; }
        public bool success { get; set; }
        public bool mobile { get; set; }
        public List<object> input { get; set; }
        public List<PayStackStatusHistory> history { get; set; }
    }
    public class PayStackStatusHistory
    {
        public string type { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }
    public class PayStackStatusAuthorization
    {
        public string authorization_code { get; set; }
        public string bin { get; set; }
        public string last4 { get; set; }
        public string exp_month { get; set; }
        public string exp_year { get; set; }
        public string channel { get; set; }
        public string card_type { get; set; }
        public string bank { get; set; }
        public string country_code { get; set; }
        public string brand { get; set; }
        public bool reusable { get; set; }
        public string signature { get; set; }
        public object account_name { get; set; }
    }

    public class PayStackStatusData
    {
        //public long id { get; set; }
        public string domain { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public long amount { get; set; }
        public object message { get; set; }
        public string gateway_response { get; set; }
        public string paid_at { get; set; }
        public string created_at { get; set; }
        public string channel { get; set; }
        public string currency { get; set; }
        public string ip_address { get; set; }
        public Metadata metadata { get; set; }
        public PayStackStatusLog log { get; set; }
        public int? fees { get; set; }
        public object fees_split { get; set; }
        public PayStackStatusAuthorization authorization { get; set; }
        public PayStackStatusCustomer customer { get; set; }
        public object plan { get; set; }
        public PayStackStatusSplit split { get; set; }
        public object order_id { get; set; }
        public string paidAt { get; set; }
        public string createdAt { get; set; }
        public int requested_amount { get; set; }
        public string transaction_date { get; set; }
        public PayStackStatusPlanObject plan_object { get; set; }
        public PayStackStatusSubaccount subaccount { get; set; }
    }
    public class PayStackStatusSplit
    {
    }

    public class PayStackStatusPlanObject
    {
    }

    public class PayStackStatusSubaccount
    {
    }
    public class PayStackStatusCustomer
    {
        public int id { get; set; }
        public object first_name { get; set; }
        public object last_name { get; set; }
        public string email { get; set; }
        public string customer_code { get; set; }
        public object phone { get; set; }
        public object metadata { get; set; }
        public string risk_action { get; set; }
        public object international_format_phone { get; set; }
    }
    public class PayStackStatusRoot
    {
        public bool status { get; set; }
        public string message { get; set; }
        public PayStackStatusData data { get; set; }
    }
    public class Metadata
    {
        public CustomFields custom_fields { get; set; }
    }
    public class CustomFields
    {
        public string display_name { get; set; }
        public string variable_name { get; set; }
        public string value { get; set; }
    }
}
