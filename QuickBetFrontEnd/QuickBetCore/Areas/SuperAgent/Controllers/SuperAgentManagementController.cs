using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickBetCore.Areas.Admin.Data;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.SuperAgent.Data;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Quickbet.Areas.SuperAgent.Controllers
{
    [TypeFilter(typeof(CheckSuperAgentSessionExpire))]
    public class SuperAgentManagementController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SuperAgentManagementController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: SuperSuperAgentManagement/SuperAgentManagement
        public ActionResult Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            model.Agents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
            && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.Pending_for_approval).Count();

            model.TotalAgents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
            && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.active).Count();

            //model.BlockedAgent = db.Users.Where(a => a.UserType == (int)UserType.Agent
            // && a.ParentAgentId == userSession.Id && a.UserStatus == (int)UserStatus.block).Count();

            if (TempData["ActionResponse"] != null)
            {
                //ViewBag.response = (ApiResponse)TempData["ActionResponse"];
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
            }
            return View(model);
        }

        public ActionResult CreateAgent(int AgentId = 0)
        {
            List<SelectListItem> AgentType = new List<SelectListItem>();
            CustomerUploadModel customer = new CustomerUploadModel();
            try
            {
                AgentType.Add(new SelectListItem { Text = "---Select Agent Type--", Value = "", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Mobile Agent", Value = "6", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Shop Agent", Value = "3", Selected = false });
               // AgentType.Add(new SelectListItem { Text = "Super Agent", Value = "5", Selected = false });
                ViewBag.AgentType = AgentType;
                if (AgentId > 0)
                {
                    customer = CustomerOperation.GetCustomer(AgentId, (int)UserType.Agent);
                }
                if (TempData["ActionResponse"] != null)
                {
                   // ViewBag.response = (ApiResponse)TempData["ActionResponse"];
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
                }
            }
            catch (Exception)
            {

              
            }
            
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateAgent(CustomerUploadModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        string uploadsFolder = webHostEnvironment.WebRootPath + "/Content/Images";
                        //getting file name and combine with path and save it
                        var file = Request.Form.Files[0];
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        //save folder path 
                        model.ProfilePicture = "/Content/Images/" + filename;
                    }
                    model.ContactNo = model.ContactNo.Replace(" ", String.Empty);
                    response = AgentCustomerOperation.CreateUpdateCustomer(model, model.Agentype, userSession.Id);
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";

                }
            }
            catch(Exception ex)
            {
                response.Msg = ex.Message;
            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            if (model.Id > 0)
            {
                return RedirectToAction("Index", "SuperAgentManagement", new { area = "SuperAgent" });
            }
            return RedirectToAction("CreateAgent", "SuperAgentManagement", new { area = "SuperAgent" });

        }

        public ActionResult GetAgentlist(string status = "")
        {
            
            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();

                if (string.IsNullOrEmpty(status))
                {
                    users = db.Users.Where(a => a.ParentAgentId == userSession.Id).ToList();
                }
                else
                {
                    int userstatus = Convert.ToInt32(status);
                    users = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent) && a.ParentAgentId == userSession.Id && a.UserStatus == userstatus).ToList();
                }

                if (users != null && users.Count>0)
                {
                    model = (from r in users
                             select new UserListViewModel
                             {
                                 Id = r.Id,
                                 name = r.Name,
                                 email = r.Email,
                                 countrycode = r.CountryCode,
                                 phone = r.ContactNo,
                                 status = r.UserStatus,
                                 profile = ApplicatiopnCommonFunction.GetImage(r.ProfilePicture, ImageType.dp.ToString()),
                                 walletbalance = r.MyWalletbalance,
                                 quickbetcommision = r.AgentCommison,
                                 cashback = r.AgentCashBackOnPayment,
                                 UserType = r.UserType,
                                 CustomerRetentionPeriod = r.CustomerRetentionPeriod,
                                 AgentsEarnings = AgentTotalEarnings(r.Id)
                             }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_Agents", model);
        }

        private Decimal AgentTotalEarnings(int id =0)
        {
            decimal amount = 0;
            try
            {
                if (id > 0)
                {
                    amount = db.WalletTransactions.Where(x => x.UserId == id
                       && (x.TransferType == (int)WalletTransactionType.AgentCommission_onBet || x.TransferType == (int)WalletTransactionType.Cashback_On_WinPrize) && x.Status == (int)WalletTransactionStatusType.TransactionSuccess).Sum(x => (decimal?)x.Amount).GetValueOrDefault();
                }
            }
            catch (Exception ex)
            {
            }
            return amount;
        }
        public ActionResult Bettinglist()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = (from r in db.Users
                     where r.UserType == (int)UserType.Agent
                     && r.UserStatus == (int)UserStatus.active
                     && r.ParentAgentId == userSession.Id
                     select new UserListViewModel
                     {
                         Id = r.Id,
                         name = r.Name,
                         email = r.Email,
                         countrycode = r.CountryCode,
                     }).OrderBy(a => a.name).ToList();
            return View(model);
        }

        public ActionResult GetBarcode(int WinId)
        {
            BarCodeViewModel model = new BarCodeViewModel();
            
            try
            {
                var winobj = db.Playwins.Where(a => a.Id == WinId && a.PaidMoneyStatus == (int)WinPaidMoneyStatus.NotPaid).FirstOrDefault();
                if (string.IsNullOrEmpty(winobj.BarCode))
                {
                    winobj.BarCode = winobj.WinnerCode + ApplicatiopnCommonFunction.AlphanumbericNumber();
                    db.SaveChanges();
                }
                model.Id = winobj.Id;
                model.wincode = winobj.WinnerCode;
                model.amount = winobj.JackpotAmount;
                //model.barcode = ApplicatiopnCommonFunction.GenerateBarcode(winobj.BarCode);
            }
            catch (Exception ex)
            {
                model.msg = ex.Message;
            }
            return PartialView("~/Areas/Agent/Views/Betting/_Barcode.cshtml", model);
        }

        public ActionResult Getwinlist(string viewType = "mm", int AgentId = 0)
        {
            
            List<BettingViewModel> model = new List<BettingViewModel>();
            try
            {
                DateTime filterdate = new DateTime();
                if (viewType == ViewFilters.Today)
                {
                    filterdate = DateTime.UtcNow;
                }
                if (viewType == ViewFilters.Week)
                {
                    filterdate = DateTime.UtcNow.AddDays(-7);

                }
                else if (viewType == ViewFilters.Month)
                {
                    filterdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                }
                if (AgentId > 0)
                {
                    List<Playwin> wins = new List<Playwin>();
                    if (viewType == ViewFilters.LifeTime)
                    {
                        wins = db.Playwins.Include("User1").Where(a => a.AgentId == AgentId).ToList();
                    }
                    else
                    {
                        wins = db.Playwins.Include("User1").Where(a => a.AgentId == AgentId
                        && a.InsertAt.Date >= filterdate.Date).ToList();

                    }
                    var agentobj = db.Users.Where(a => a.Id == AgentId).FirstOrDefault();
                    if (wins != null)
                    {
                        model = (from r in wins
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + 
                                     "(" + r.ExternalPlayerIdUser.Email + ")" : "",
                                     gamename = r.GameName,
                                     datetime = r.InsertAt,
                                     agentname = agentobj.Name + "(" + agentobj.Email + ")",
                                     PaidMoneyStatus = r.PaidMoneyStatus,
                                     jackpotamount = r.JackpotAmount,
                                 }).OrderByDescending(a => a.datetime).ToList();
                    }

                }
                else
                {

                    List<Playwin> wins = new List<Playwin>();
                    if (viewType == ViewFilters.LifeTime)
                    {
                        //wins = db.Playwins.Include("User1").ToList();
                        wins = db.Playwins
     .Include(c => c.ExternalPlayerIdUser)
     .Where(c => c.AgentId == userSession.Id)
     .ToList();
                    }
                    else
                    {
                        // wins = db.Playwins.Include("User1").Where(a => DbFunctions.TruncateTime(a.InsertAt) >= DbFunctions.TruncateTime(filterdate)).ToList();
                        wins = db.Playwins.Where(a => a.InsertAt.Date >= filterdate.Date)
      .Include(c => c.ExternalPlayerIdUser)
      .Where(c => c.AgentId == userSession.Id)
      .ToList();
                    }
                    if (wins != null && wins.Count() > 0)
                    {
                        model = (from r in wins
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : "",
                                     gamename = r.GameName,
                                     datetime = r.InsertAt,
                                     PaidMoneyStatus = r.PaidMoneyStatus,
                                     agentname = GetAgent(r.ExternalPlayerIdUserId),
                                     jackpotamount = r.JackpotAmount,
                                 }).OrderByDescending(a => a.datetime).ToList();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_winninglist", model);
        }

        public string GetAgent(int UserId)
        {
            var agentobj = db.Users.Where(a => a.Id == UserId).FirstOrDefault();
            if (agentobj != null)
            {
                return agentobj.Name + "(" + agentobj.Email + ")";
            }
            return "";
        }

        public ActionResult Getbetlist(string viewType = "mm", int AgentId = 0)
        {
            
            List<BettingViewModel> model = new List<BettingViewModel>();
            try
            {
                DateTime filterdate = new DateTime();
                if (viewType == ViewFilters.Today)
                {
                    filterdate = DateTime.UtcNow;
                }
                if (viewType == ViewFilters.Week)
                {
                    filterdate = DateTime.UtcNow.AddDays(-7);

                }
                else if (viewType == ViewFilters.Month)
                {
                    filterdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                }

                if (AgentId > 0)
                {
                    var agentobj = db.Users.Where(a => a.Id == AgentId).FirstOrDefault();
                    List<PlayerBet> bets = new List<PlayerBet>();
                    if (viewType == ViewFilters.LifeTime)
                    {
                        bets = db.PlayerBets.Include("User1").Where(a => a.ExternalPlayerIdUserId == AgentId).ToList();
                    }
                    else
                    {
                        bets = db.PlayerBets.Include("User1").Where(a => a.ExternalPlayerIdUserId == AgentId 
                        && a.Insertdate.Date >= filterdate.Date).ToList();
                    }

                    if (bets != null && bets.Count() > 0)
                    {
                        model = (from r in bets
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? 
                                     r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : "",
                                     gamename = r.GameName,
                                     agentname = agentobj.Name + "(" + agentobj.Email + ")",
                                     datetime = r.Insertdate,
                                     betamount = r.Amount,
                                     quikbetcommision = r.AgentCommison,
                                 }).OrderByDescending(a => a.datetime).ToList();
                    }

                }
                else
                {
                    List<PlayerBet> bets = new List<PlayerBet>();
                    if (viewType == ViewFilters.LifeTime)
                    {
                        // bets = db.PlayerBets.Include("User1").ToList();
                        bets = db.PlayerBets
      .Include(c => c.ExternalPlayerIdUser)
      .Where(c => c.AgentId == userSession.Id)
      .ToList();
                    }
                    else
                    {
                        // bets = db.PlayerBets.Include("User1").Where(a => DbFunctions.TruncateTime(a.Insertdate) >= DbFunctions.TruncateTime(filterdate)).ToList();
                        bets = db.PlayerBets.Where(a => a.Insertdate.Date >= filterdate.Date)
   .Include(c => c.ExternalPlayerIdUser)
   .Where(c => c.AgentId == userSession.Id).ToList();
                    }

                    if (bets != null && bets.Count() > 0)
                    {
                        model = (from r in bets
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : "",
                                     gamename = r.GameName,
                                     datetime = r.Insertdate,
                                     agentname = GetAgent(r.ExternalPlayerIdUserId),
                                     betamount = r.Amount,
                                     quikbetcommision = r.AgentCommison,
                                 }).OrderByDescending(a => a.datetime).ToList();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_bettinglist", model);
        }

        public ActionResult TranHistoryByUser(int customerid)
        {
            List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
            try
            {
                result = (from r in db.WalletTransactions
                          where r.UserId == customerid
                          select new TransactionHistoryViewModel
                          {
                              TransactionId = r.TransactionId,
                              Date = r.InsertDate,
                              Amount = r.Amount,
                              FinalBalanceAmount = r.ClosingBalance,
                              TransactionType = r.TransType,
                              TransactionStatus = r.Status,
                              Remark = r.TransactionRemark
                          }).OrderByDescending(a => a.Date).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }

        public ActionResult TransferFunds(int agentId = 0)
        {
            FundTransferModel fundTransfer = new FundTransferModel();
            CustomerUploadModel customer = new CustomerUploadModel();
            ApiResponse resp = new ApiResponse();
            if (agentId > 0)
            {
                customer = CustomerOperation.GetCustomer(agentId, (int)UserType.Agent);
                if (customer != null)
                {
                    fundTransfer.UserId = customer.Id;
                    fundTransfer.BeneficiaryName = customer.Name;
                }
                else
                {
                    resp.Code = (int)ApiResponseCode.fail; resp.Msg = "Invalid Request";
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                    return RedirectToAction("Index", "Customers", new { Areas = "Admin" });
                }
            }
            else
            {
                resp.Code = (int)ApiResponseCode.fail; resp.Msg = "Invalid Request";
                TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                return RedirectToAction("Index", "Customers", new { Areas = "Admin" });
            }
            return View(fundTransfer);
        }

        [HttpPost]
        public JsonResult BlockUser([FromBody] BlockUnblockModel unblockModel)
        {
            dashboard resp = new dashboard();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(unblockModel.type) && unblockModel.Id > 0)
                {
                    resp = DbOperation.BlockAnythingSuperAgent(unblockModel.type, unblockModel.Id,userSession.Id);
                }
                else
                {
                    resp.Msg = "Invalid Entry";
                }
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        [HttpPost]
        public JsonResult DeleteCustomer(int Id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                resp = CustomerOperation.DeleteUserSuperAgent(Id,userSession.Id);

                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        #region AgentCustomerList
        public ActionResult AgentCustomers(int AgentId)
        {
            ViewData["AgentId"] = AgentId;
            return View();
        }
        public ActionResult GetAgentCustomerlist(int AgentId)
        {
            var userLists = CustomerOperation.GetAgentCustomers(AgentId);
            return PartialView("_AgentCustomers", userLists);
        }
        #endregion
    }
}