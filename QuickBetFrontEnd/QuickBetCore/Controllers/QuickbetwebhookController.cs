using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.SuperAgent.Data;
using QuickBetCore.Areas.User.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Models.GenegameStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    public class webhooksmodel
    {

        //AccessRequest
        public string action { get; set; }
        public string secret { get; set; }
        public string clinetId { get; set; }
        public string token { get; set; }
        public int gameId { get; set; }
        public string gameName { get; set; }

        //BalanceRequest
        public int externalPlayerId { get; set; }
        public string externalSessionId { get; set; }
        public int agentId { get; set; }

        //Playerbet
        public decimal amount { get; set; }
        public string currency { get; set; }
        public decimal betId { get; set; }
        public long gameSessionId { get; set; }
        public string transactionId { get; set; }

        //Playerwin
        public string type { get; set; }
        public FreeRoundData freeRoundData { get; set; }
        public decimal jackpotAmount { get; set; }
        
        public int winnerCode { get; set; }
        public bool isJackpotWin { get; set; }
        public int jackpotType { get; set; }
        public string jackpotName { get; set; }
        public string jackpotImage { get; set; }
        ///Refer code 
        public decimal ReferAmount { get; set; }

        public int ReferedFrom { get; set; }

        //SupeerAgent commission

        public decimal SuperAgentCommissionAmount { get; set; }

        public int SuperAgentId { get; set; }
    }
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuickbetwebhookController : ControllerBase
    {
        QuickbetDbEntities dbConn = new QuickbetDbEntities();

        //api/Quickbetwebhook/webhook
        [HttpPost]
        public IActionResult webhook(webhooksmodel request)
        {
            AuthenticateResponse globalresp = new AuthenticateResponse();
            globalresp.statusCode = (int)hacksawgamingstatuscode.Invalid_user_token_expired;
       


        //api/Quickbetwebhook/paystackwebshook
        //[HttpPost]
        //public IActionResult paystackwebshook(PaystackWebHookResponse paystacl)
        //{
        //    CusomlogWriter.Loghacksawgaming("PayStackPayment webhook recived:" + new JavaScriptSerializer().Serialize(paystacl), "paystackCallback");
        //    return Ok();
        //}
        //api/Quickbetwebhook/paystackwebshooklive
        [HttpPost]
        public IActionResult paystackwebshooklive()
        {
            return Ok();
        }
        //api/Quickbetwebhook/opaywebshook
        [HttpPost]
        public IActionResult opaywebshook()
        {
            return Ok();
        }
        //api/Quickbetwebhook/opaywebshooklive
        [HttpPost]
        public IActionResult opaywebshooklive()
        {
            return Ok();
        }
    }
}
