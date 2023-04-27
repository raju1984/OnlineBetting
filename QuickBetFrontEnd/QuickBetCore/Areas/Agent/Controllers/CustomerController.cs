using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickbet.Areas.Agent.Controllers
{
    [TypeFilter(typeof(CheckAgentSessionExpire))]
    public class CustomerController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomerController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Agent/Customer
        public ActionResult Index()
        {
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);// (ApiResponse)TempData["ActionResponse"];
            }
            return View();
        }
        public ActionResult GetAgentCustomerlist()
        {
            
            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                model = (from r in db.AgentCustomers.Include("User")
                         where r.AgentId==userSession.Id 
                         select new UserListViewModel
                         {
                             Id = r.User.Id,
                             name = r.User.Name,
                             email = r.User.Email,
                             countrycode = r.User.CountryCode,
                             phone = r.User.ContactNo,
                             UserStatus=r.User.UserStatus
                         }).ToList();
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Customers", model);
        }

        public ActionResult CreateAgentCustomer(int gameid=0)
        {
            AgentCustomerModel customer = new AgentCustomerModel();
            customer.gameid = 0;
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //(ApiResponse)TempData["ActionResponse"];
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUpdateAgentCustomer(AgentCustomerModel reg)
        {
            ApiResponse response = new ApiResponse();
            ModelState.Remove("gameid");
            if (ModelState.IsValid)
            {
                
                string email = reg.Email.Trim().ToLower();
                string ContactNo = reg.ContactNo.Trim().Replace(" ", String.Empty).ToLower();
                bool isany = db.Users.Any(a => a.Email == email.ToLower() || a.ContactNo == ContactNo);
                if (!isany)
                {
                    QuickBetCore.DatabaseEntity.User users = new QuickBetCore.DatabaseEntity.User();
                    users.Email = email;
                    users.Name = reg.Name;
                    users.Password = reg.Password;
                    users.ContactNo = ContactNo;
                    users.CountryCode = reg.CountryCode;
                    users.UserType = (int)UserType.Users;
                    users.UserStatus = (int)UserStatus.active;
                    users.CreatedAt = DateTime.UtcNow;
                    users.UpdatedAt = DateTime.UtcNow;
                    users.AgentCommison = 0;
                    users.AgentCashBackOnPayment = 0;

                    var agentObj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();

                    AgentCustomer agentCustomerObj = new AgentCustomer();
                    agentCustomerObj.AgentId = userSession.Id;
                    agentCustomerObj.CustomerRetentionPeriod = agentObj.CustomerRetentionPeriod;
                    agentCustomerObj.User = users;
                    agentCustomerObj.CreatedAt = DateTime.UtcNow;
                    db.AgentCustomers.Add(agentCustomerObj);
                    db.SaveChanges();
                    string SincerelyName = "QuickBet Team";
                    String Subject = "Welcome mail";

                    String Body = "Hi " + users.Name + "<br><br>"+ "Welcome to quickbet games, Your account successfully generated." +
                    "<br><br>Sincerely,<br>" + SincerelyName ;
                    CommonFunction.SendEmail(users.Email, users.Name, 1, Subject, Body);

                    response.Msg = Applicationstring.Customercreatedsuccessfully;
                    response.Code = (int)ApiResponseCode.ok;
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
                    if (reg.gameid > 0)
                    {
                        return RedirectToAction("PlayAgentGame", "Playgame", new { area = "", customerId = new AES_ALGORITHM().encryptMessage(agentCustomerObj.UserId.ToString()), gameid = reg.gameid });
                    }
                    else
                    {
                        return RedirectToAction("CreateAgentCustomer", "Customer", new { area = "Agent", gameid = reg.gameid });
                    }
                }
                else
                {
                    response.Msg = Applicationstring.Alreadyhavecustomer;
                    response.Code = (int)ApiResponseCode.fail;
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
                    return RedirectToAction("CreateAgentCustomer", "Customer", new { area = "Agent", gameid = reg.gameid });
                }

            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";
            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            return RedirectToAction("CreateAgentCustomer", "Customer", new { area = "Agent" });
        }
    }
}