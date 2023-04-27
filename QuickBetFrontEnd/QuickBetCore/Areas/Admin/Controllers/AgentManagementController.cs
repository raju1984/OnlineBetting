using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickBetCore.Areas.Admin.Data;
using QuickBetCore.Areas.Agent.Data;
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
namespace QuickBetCore.Areas.Admin.Controllers
{
    [TypeFilter(typeof(CheckAdminSessionExpire))]
    public class AgentManagementController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AgentManagementController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/AgentManagement
        public ActionResult Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            model.Agents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
                && a.UserStatus == (int)UserStatus.Pending_for_approval).Count();

            model.TotalAgents = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
               && a.UserStatus == (int)UserStatus.active).Count();

            model.BlockedAgent = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent)
              && a.UserStatus == (int)UserStatus.block).Count();

            model.ShopAgent = db.Users.Where(a => (a.UserType == (int)UserType.Agent) && a.UserStatus == (int)UserStatus.active).Count();

            model.mobileAgent = db.Users.Where(a => (a.UserType == (int)UserType.MobileAgent) && a.UserStatus == (int)UserStatus.active).Count();



            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
            }
            return View(model);
        }
        public ActionResult GetAgentlist(string status = "")
        {

            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();

                if (string.IsNullOrEmpty(status))
                {
                    users = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent) && a.UserStatus == (int)UserStatus.active).ToList();
                }
                else
                {
                    int userstatus = Convert.ToInt32(status);
                    if (userstatus == 11)
                    {
                        users = db.Users.Where(a => (a.UserType == (int)UserType.Agent) && a.UserStatus == (int)UserStatus.active).ToList();
                    }
                    else if (userstatus == 12)
                    {
                        users = db.Users.Where(a => (a.UserType == (int)UserType.MobileAgent) && a.UserStatus == (int)UserStatus.active).ToList();
                    }
                    else
                    {
                        users = db.Users.Where(a => (a.UserType == (int)UserType.Agent || a.UserType == (int)UserType.MobileAgent) && a.UserStatus == userstatus).ToList();
                    }
                }

                if (users != null)
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
                                 ParentAgentId = GetAgentname((int)(r.ParentAgentId == null ? 0 : r.ParentAgentId)),
                                 CustomerRetentionPeriod = r.CustomerRetentionPeriod
                             }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_Agents", model);
        }



        public string GetAgentname(int id)
        {
            string Name = "NA";
            try
            {

                if (id > 0)
                {
                    var check = db.Users.Where(x => x.Id == id).FirstOrDefault();
                    if (check != null)
                    {
                        Name = check.Name;
                    }

                }

            }
            catch (Exception ex)
            {


            }
            return Name;

        }
        public ActionResult GetSuperAgentlist(string status = "")
        {

            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();

                if (string.IsNullOrEmpty(status))
                {
                    users = db.Users.Where(a => a.UserType == (int)UserType.SuperAgent && a.UserStatus == (int)UserStatus.active).ToList();
                }
                else
                {
                    int userstatus = Convert.ToInt32(status);
                    users = db.Users.Where(a => a.UserType == (int)UserType.SuperAgent && a.UserStatus == userstatus).ToList();
                }

                if (users != null)
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
                                 quickbetcommision = Convert.ToDecimal(r.SuperAgentCashBack == null ? 0 : r.SuperAgentCashBack),
                                 //  cashback = r.AgentCashBackOnPayment,
                                 // CustomerRetentionPeriod = r.CustomerRetentionPeriod
                             }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_SuperAgents", model);
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
                AgentType.Add(new SelectListItem { Text = "Super Agent", Value = "5", Selected = false });
                ViewBag.AgentType = AgentType;

                if (AgentId > 0)
                {
                    customer = CustomerOperation.GetCustomer(AgentId, (int)UserType.Agent);
                }
                if (TempData["ActionResponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
                }
            }
            catch (Exception ex)
            {
            }

            return View(customer);
        }
        public ActionResult CreateSuperAgent(int AgentId = 0)
        {
            CustomerUploadModel customer = new CustomerUploadModel();
            if (AgentId > 0)
            {
                customer = CustomerOperation.GetCustomer(AgentId, (int)UserType.Agent);
            }
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateAgent(CustomerUploadModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    //getting file name and combine with path and save it
                    string uploadsFolder = webHostEnvironment.WebRootPath + "/Content/Images/";
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    //save folder path 
                    model.ProfilePicture = "/Content/Images/" + file.FileName;
                }
                model.ContactNo = model.ContactNo.Replace(" ", String.Empty);
                if (model.Agentype == (int)UserType.Agent)
                {
                    response = CustomerOperation.CreateUpdateCustomer(model, (int)UserType.Agent);
                }
                else if (model.Agentype == (int)UserType.MobileAgent)
                {
                    response = CustomerOperation.CreateUpdateCustomer(model, (int)UserType.MobileAgent);
                }
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";

            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            if (model.Id > 0)
            {
                return RedirectToAction("Index", "AgentManagement", new { area = "Admin" });
            }
            return RedirectToAction("CreateAgent", "AgentManagement", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateSuperAgent(CustomerUploadModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    //getting file name and combine with path and save it
                    string uploadsFolder = webHostEnvironment.WebRootPath + "/Content/Images/";
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    //save folder path 
                    model.ProfilePicture = "/Content/Images/" + file.FileName;
                }
                model.ContactNo = model.ContactNo.Replace(" ", String.Empty);
                response = CustomerOperation.CreateUpdateCustomer(model, (int)UserType.SuperAgent);
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";

            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            if (model.Id > 0)
            {
                return RedirectToAction("SuperAgents", "AgentManagement", new { area = "Admin" });
            }
            return RedirectToAction("CreateSuperAgent", "AgentManagement", new { area = "Admin" });
        }
        public ActionResult TransferFunds(int agentId = 0)
        {
            FundTransferModel fundTransfer = new FundTransferModel();
            CustomerUploadModel customer = new CustomerUploadModel();
            ApiResponse resp = new ApiResponse();
            if (agentId > 0)
            {
                var GetAgent = db.Users.Where(x => x.Id == agentId).FirstOrDefault();
                if (GetAgent != null)
                {
                    customer = CustomerOperation.GetCustomer(agentId, GetAgent.UserType);
                }
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
        public ActionResult FundTrsansferToAgent(FundTransferModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {
                response = CustomerOperation.TransferFund(model, userSession.Id);
                if (response.Code == (int)ApiResponseCode.ok)
                {
                    return RedirectToAction("TransactionReciept", "AgentManagement", new { txn = response.txnId, area = "Admin" });
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
                    return RedirectToAction("TransferFunds", "AgentManagement", new { agentId = model.UserId, area = "Admin" });
                }
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";
            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            return RedirectToAction("Index", "AgentManagement", new { area = "Admin" });
        }
        public ActionResult TransactionReciept(int txn)
        {
            FundTransferModel model = new FundTransferModel();
            try
            {
                var transaction = db.WalletTransactions.Include("User").Where(a => a.Id == txn).FirstOrDefault();
                model.Amount = Convert.ToDouble(transaction.Amount);
                model.BeneficiaryName = transaction.User.Name;
                model.Description = transaction.Note;
                model.transactionId = transaction.Id.ToString();
                model.TypeOfTransfer = transaction.TransType;
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        [HttpPost]
        public JsonResult UpdateCommison([FromBody]UpdateModel updateModel)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(updateModel.type) && updateModel.Id > 0)
                {
                    var agentobj = db.Users.Where(a => a.Id == updateModel.Id).FirstOrDefault();
                    if (updateModel.type == "qbc")
                    {
                        agentobj.AgentCommison = updateModel.value;
                        db.SaveChanges(); resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;
                    }
                    else if (updateModel.type == "cashback")
                    {
                        agentobj.AgentCashBackOnPayment = updateModel.value;
                        db.SaveChanges(); resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;

                    }
                    else if (updateModel.type == "retention")
                    {
                        agentobj.CustomerRetentionPeriod = Convert.ToInt32(updateModel.value);
                        db.SaveChanges(); resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;
                    }
                    else if (updateModel.type == "SuperAgentCommission")
                    {
                        agentobj.SuperAgentCashBack = updateModel.value;
                        db.SaveChanges(); resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;
                    }
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

        // GET: Admin/AgentManagement
        public ActionResult Bettinglist()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            model = (from r in db.Users
                     where r.UserType == (int)UserType.Agent
                     && r.UserStatus == (int)UserStatus.active
                     select new UserListViewModel
                     {
                         Id = r.Id,
                         name = r.Name,
                         email = r.Email,
                         countrycode = r.CountryCode,
                     }).OrderBy(a => a.name).ToList();
            return View(model);
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
                        wins = db.Playwins.Include("ExternalPlayerIdUser").Where(a => a.AgentId == AgentId).ToList();
                    }
                    else
                    {
                        wins = db.Playwins.Include("ExternalPlayerIdUser").Where(a => a.AgentId == AgentId
                        && a.InsertAt.Date >= filterdate.Date).ToList();
                    }
                    var agentobj = db.Users.Where(a => a.Id == AgentId).FirstOrDefault();
                    if (wins != null)
                    {
                        model = (from r in wins
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : "",
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
                        wins = db.Playwins.Include("ExternalPlayerIdUser").ToList();
                    }
                    else
                    {
                        wins = db.Playwins.Include("ExternalPlayerIdUser").Where(a => a.InsertAt.Date >= filterdate.Date).ToList();
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
                        bets = db.PlayerBets.Include("ExternalPlayerIdUser").Where(a => a.ExternalPlayerIdUserId == AgentId).ToList();
                    }
                    else
                    {
                        bets = db.PlayerBets.Include("ExternalPlayerIdUser").Where(a => a.ExternalPlayerIdUserId == AgentId
                        && a.Insertdate.Date >= filterdate.Date).ToList();
                    }

                    if (bets != null && bets.Count() > 0)
                    {
                        model = (from r in bets
                                 select new BettingViewModel
                                 {
                                     Id = r.Id,
                                     customername = r.ExternalPlayerIdUser != null ? r.ExternalPlayerIdUser.Name + "(" + r.ExternalPlayerIdUser.Email + ")" : "",
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
                        bets = db.PlayerBets.Include("ExternalPlayerIdUser").ToList();
                    }
                    else
                    {
                        bets = db.PlayerBets.Include("ExternalPlayerIdUser").Where(a => a.Insertdate.Date
                        >= filterdate.Date).ToList();
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
        [HttpPost]
        public JsonResult PayLottery(int WinId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                var winobj = db.Playwins.Where(a => a.Id == WinId && a.PaidMoneyStatus == (int)WinPaidMoneyStatus.NotPaid).FirstOrDefault();
                if (winobj != null)
                {
                    winobj.PaidMoneyStatus = (int)WinPaidMoneyStatus.Paid_By_Quickbet;
                    db.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                    resp.Msg = Applicationstring.Paymentcompletedsuccessfully;
                    return Json(resp);
                }
                else
                {
                    resp.Msg = "Invalid Lottery!";
                    return Json(resp);
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        public ActionResult SuperAgents()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            model.Agents = db.Users.Where(a => a.UserType == (int)UserType.SuperAgent
                && a.UserStatus == (int)UserStatus.Pending_for_approval).Count();

            model.TotalAgents = db.Users.Where(a => a.UserType == (int)UserType.SuperAgent
               && a.UserStatus == (int)UserStatus.active).Count();

            model.BlockedAgent = db.Users.Where(a => a.UserType == (int)UserType.SuperAgent
              && a.UserStatus == (int)UserStatus.block).Count();

            model.AgentToSuperAgentPendingList = db.AgentPromotionEntries.Where(x => x.IsApproved == false && x.ApprovedAdminId == 0).Count();

            model.AgentToSuperAgentList = (from d in db.AgentPromotionEntries
                                           join ee in db.Users
                                           on d.AgentId equals ee.Id
                                           where d.IsApproved == true
                                           select d).Count();


            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);
            }
            return View(model);
        }

        public ActionResult GetAgentToSuperAgent(string status = "")
        {

            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();

                if (status == "0")
                {
                    model = (from ee in db.Users
                             join d in db.AgentPromotionEntries
                              on ee.Id equals d.AgentId
                             where d.IsApproved == true && ee.UserType == (int)UserType.SuperAgent && ee.UserStatus == (int)UserStatus.active
                             select new UserListViewModel
                             {
                                 Id = ee.Id,
                                 name = ee.Name,
                                 email = ee.Email,
                                 countrycode = ee.CountryCode,
                                 phone = ee.ContactNo,
                                 status = ee.UserStatus,
                                 profile = "0",
                                 walletbalance = ee.MyWalletbalance,

                             }).ToList();
                }
                else
                {
                    model = (from ee in db.Users
                             join d in db.AgentPromotionEntries
                              on ee.Id equals d.AgentId
                             where d.IsApproved == false && d.ApprovedAdminId == 0 && (ee.UserType == (int)UserType.Agent || ee.UserType == (int)UserType.MobileAgent) && ee.UserStatus == (int)UserStatus.active

                             select new UserListViewModel
                             {
                                 Id = ee.Id,
                                 name = ee.Name,
                                 email = ee.Email,
                                 countrycode = ee.CountryCode,
                                 phone = ee.ContactNo,
                                 status = ee.UserStatus,
                                 profile = "1",
                                 walletbalance = ee.MyWalletbalance,

                             }).ToList();
                }



            }
            catch (Exception ex)
            {

            }
            return PartialView("_GetAgentToSuperAgent", model);
        }

        public JsonResult AproveSuperAgent(int id)
        {
            dashboard response = new dashboard();
            try
            {
                if (id > 0)
                {

                    var check = db.Users.Where(x => x.Id == id && x.UserStatus == (int)UserStatus.active &&
                   (x.UserType == (int)UserType.MobileAgent || x.UserType == (int)UserType.Agent)
                    ).FirstOrDefault();
                    if (check != null)
                    {
                        check.UserType = (int)UserType.SuperAgent;
                        check.SuperAgentCashBack = ApplicationVariable.defaultSuperAgentCommission;

                        //check.AgentCashBackOnPayment = 0;
                        // check.CustomerRetentionPeriod = 0;
                        var approve = db.AgentPromotionEntries.Where(x => x.AgentId == check.Id).FirstOrDefault();
                        approve.IsApproved = true;
                        approve.UpdatedDate = DateTime.UtcNow;
                        approve.ApprovedAdminId = userSession.Id;

                        if (db.SaveChanges() > 0)
                        {
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = Applicationstring.Success;
                        }
                        response.ActiveUser = db.AgentPromotionEntries.Where(x => x.IsApproved == false && x.ApprovedAdminId == 0).Count();
                        response.BlockUser = (from d in db.AgentPromotionEntries
                                              join ee in db.Users
                                              on d.AgentId equals ee.Id
                                              where d.IsApproved == true
                                              select d).Count();
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "User Id does not exist! Please try again";
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Json(response);
        }

        public JsonResult RejectSuperAgent(int id)
        {
            dashboard response = new dashboard();
            try
            {
                if (id > 0)
                {

                    var check = db.Users.Where(x => x.Id == id && x.UserStatus == (int)UserStatus.active &&
                   (x.UserType == (int)UserType.MobileAgent || x.UserType == (int)UserType.Agent)
                    ).FirstOrDefault();
                    if (check != null)
                    {


                        var reject = db.AgentPromotionEntries.Where(x => x.AgentId == check.Id).FirstOrDefault();
                        if (reject != null)
                        {
                            reject.IsApproved = false;
                            reject.UpdatedDate = DateTime.UtcNow;
                            reject.ApprovedAdminId = userSession.Id;
                        }
                        if (db.SaveChanges() > 0)
                        {
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = Applicationstring.Success;
                        }
                        response.ActiveUser = db.AgentPromotionEntries.Where(x => x.IsApproved == false && x.ApprovedAdminId == 0).Count();
                        response.BlockUser = (from d in db.AgentPromotionEntries
                                              join ee in db.Users
                                              on d.AgentId equals ee.Id
                                              where d.IsApproved == true
                                              select d).Count();
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "User Id does not exist! Please try again";
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Json(response);
        }

        #region Shop Addesss details 


        public ActionResult getShopAddressDetails(int id = 0)
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();

            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var address = db.Addresses.Where(a => a.UserId == id).FirstOrDefault();
                    if (address != null)
                    {
                        model.Id = id;
                        model.fullname = address.FullName;
                        model.Street = address.Street;
                        model.StreetNO = address.StreetNo;
                        model.City = address.City;
                        model.PhoneNumber = address.Phone;
                        model.ZipCode = address.ZipCode;
                        model.BillingEmail = address.Emailaddress;
                        model.CountryCode = address.CountryCode;
                    }
                }

                return PartialView("_getShopAddressDetails", model);
            }
            catch
            {

            }
            return PartialView("_getShopAddressDetails");
        }
        #endregion

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

        public ActionResult SuperAgentClient(int AgentId)
        {
            ViewData["AgentId"] = AgentId;
            return View();
        }
        public ActionResult GetSuperAgentClientlist(int AgentId)
        {

            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();
                users = db.Users.Where(a => a.ParentAgentId == AgentId).ToList();
                if (users != null)
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
                                 ParentAgentId = GetAgentname((int)(r.ParentAgentId == null ? 0 : r.ParentAgentId)),
                                 CustomerRetentionPeriod = r.CustomerRetentionPeriod
                             }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_SuperAgentClient", model);
        }
    }
}