using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Models.OpayModels
{
    #region NewOpayEndPoint

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Amount
    {
        public double total { get; set; }
        public string currency { get; set; }
    }

    public class Product
    {
        public string description { get; set; }
        public string name { get; set; }
    }

    public class OpayCashierCreateRequest
    {
        public string country { get; set; }
        public string reference { get; set; }
        public Amount amount { get; set; }
        public string returnUrl { get; set; }
        public string callbackUrl { get; set; }
        public string cancelUrl { get; set; }
        public int expireAt { get; set; }
        public UserInfo userInfo { get; set; }
        public Product product { get; set; }
        public string payMethod { get; set; }
    }

    public class UserInfo
    {
        public string userEmail { get; set; }
        public string userId { get; set; }
        public string userMobile { get; set; }
        public string userName { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
  
    public class Data
    {
        public string reference { get; set; }
        public string orderNo { get; set; }
        public string cashierUrl { get; set; }
        public string status { get; set; }
        public Amount amount { get; set; }
        public Vat vat { get; set; }
    }

    public class OpayCreatePaymentResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Vat
    {
        public string total { get; set; }
        public string currency { get; set; }
    }


    #endregion

    public class Payload
    {
        public string country { get; set; }
        public string instrumentId { get; set; }
        public string fee { get; set; }
        public string channel { get; set; }
        public string displayedFailure { get; set; }
        public string reference { get; set; }
        public DateTime updated_at { get; set; }
        public string currency { get; set; }
        public bool refunded { get; set; }

        [JsonProperty("instrument-id")]
        public string InstrumentId { get; set; }
        public DateTime timestamp { get; set; }
        public string amount { get; set; }
        public string sessionId { get; set; }
        public string instrumentType { get; set; }
        public string instrument_id { get; set; }
        public string transactionId { get; set; }
        public string token { get; set; }
        public string bussinessType { get; set; }
        public string payChannel { get; set; }
        public string status { get; set; }
    }

    public class callBackResponse
    {
        public Payload payload { get; set; }
        public string sha512 { get; set; }
        public string type { get; set; }
    }

    public class cashierEnquiryModel
    {
        public string orderNo { get; set; }
        public string reference { get; set; }
        public string country { get; set; }
    }
}
