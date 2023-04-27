using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Admin.Controllers
{
    [TypeFilter(typeof(CheckAdminSessionExpire))]
    public class AdminDashboardController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminDashboardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/AdminDashboard
        public ActionResult Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();
            try
            {
                model.totaluser = db.Users.Where(a => a.UserType == (int)UserType.Users).Count();

                model.Agents = db.Users.Where(a => a.UserType == (int)UserType.Agent
                && a.UserStatus == (int)UserStatus.Pending_for_approval).Count();

                var suucesstransfer = db.WalletTransactions.
                    Where(a => a.TransferType == (int)WalletTransactionType.Transfer_To_Account
                    && a.TransType == (int)TransType.Debit).Sum(x => (decimal?)(x.Amount)) ?? 0;

                var reversaount = db.WalletTransactions.Where(a => a.TransferType == (int)WalletTransactionType.Transfer_To_Account
                && a.TransType == (int)TransType.Credit).Sum(x => (decimal?)(x.Amount)) ?? 0;
                model.Transferedfund = suucesstransfer - reversaount;

                model.aprovepending = db.WalletTransactions.Where(a => a.TransferType == (int)WalletTransactionType.Transfer_To_Account
                  && a.Status == (int)WalletTransactionStatusType.TransactionPending).Sum(x => (decimal?)(x.Amount)) ?? 0;

                var ids = db.DisputeTickeds.Where(a => a.DisputeType == (int)DisputeType.dispute_on_wallaet_transaction
                && a.Status == (int)supportTicketStatus.Open).Select(a => a.DisputeReferenceId).ToList();
                decimal sumdispute = 0;
                if (ids != null && ids.Count() > 0)
                {
                    ids = ids.Distinct().ToList();
                    var getdisputetranaction = db.WalletTransactions.Where(a => ids.Contains(a.Id)).ToList();
                    sumdispute = getdisputetranaction.Sum(x => (decimal?)(x.Amount)) ?? 0;
                    if (getdisputetranaction != null && getdisputetranaction.Count() > 0)
                    {
                        model.Disputed = sumdispute;
                    }
                }

                model.RefundAmont = db.WalletTransactions.Where(a => a.TransferType == (int)WalletTransactionType.Refund_By_Admin && a.Status == (int)WalletTransactionStatusType.TransactionSuccess).Sum(x => (decimal?)(x.Amount)) ?? 0;
                //model.Commisonearned = db.AdmincommissionTransactions.Sum(x => (decimal?)(x.Amount)) ?? 0;


            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        public ActionResult ManageOffers()
        {
            List<Offer> offers = new List<Offer>(); 
            try
            {
                offers = db.Offers.Where(a=>a.IsActive==true 
                && (a.CashBackType== (int)CashBackType.onboard 
                || a.CashBackType == (int)CashBackType.occasionally)).ToList();
                if(offers==null || offers.Count()==0)
                {
                    offers = new List<Offer>();
                    offers.Add(new Offer { CashBackPercent = 0, CashBackType = (int)CashBackType.onboard });
                    offers.Add(new Offer { CashBackPercent = 0, CashBackType = (int)CashBackType.occasionally });
                }
            }
            catch(Exception ex)
            {

            }
            return View(offers);
        }

        public ActionResult Minimumcustomerwithdrawal()
        {
            List<Offer> offers = new List<Offer>();
            try
            {
                offers = db.Offers.Where(a => a.IsActive == true && a.CashBackType== (int)CashBackType.minimumwithdraw_amount).ToList();
                if (offers == null || offers.Count() == 0)
                {
                    offers = new List<Offer>();
                    offers.Add(new Offer { CashBackPercent = ApplicationVariable.DefaultMininumBalanceToWithdraw, CashBackType = (int)CashBackType.minimumwithdraw_amount });
                }
            }
            catch (Exception ex)
            {

            }
            return View(offers);
        }
        public ActionResult Managerandombonus()
        {
            GamePricingViewModel model = new GamePricingViewModel();
            try
            {
                var randomBonus = db.RandomBonus.ToList();
                model.gamePricings = new List<GamePricingModel>();
                if (randomBonus != null && randomBonus.Any())
                {
                    foreach (var item in randomBonus)
                    {
                        model.gamePricings.Add(
                            new GamePricingModel
                            {
                                Id = item.Id,
                                NoOfTicket = item.NoOfTicket,
                                WinAmount = item.WinAmount,
                                Payout = (item.NoOfTicket * item.WinAmount),
                                NoOfSoldTicket = item.SoldTicket,
                            });
                    }
                }

            }
            catch (Exception ex)
            {

            }
            if (TempData["actionresponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["actionresponse"]);
            }
            return View(model);
        }

        public ActionResult Addrandombonus()
        {
            GamePricingViewModel model = new GamePricingViewModel();
            model.IsAdd = true;
            try
            {
                model.gamePricings = new List<GamePricingModel>();
            }
            catch (Exception ex)
            {

            }
            if (TempData["actionresponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["actionresponse"]);
            }
            return View("Managerandombonus",model);
        }

        [HttpPost]
        public IActionResult CreateRandomBonus(GamePricingViewModel gamePricingViewModels)
        {
            try
            {
                if (gamePricingViewModels == null || gamePricingViewModels.gamePricings==null)
                {
                    return RedirectToAction("Index");
                }
                if (gamePricingViewModels != null && gamePricingViewModels.gamePricings != null)
                {
                    var randomBonus = db.RandomBonus.ToList();
                    if(randomBonus!=null)
                    {
                        db.RandomBonus.RemoveRange(randomBonus);
                        db.SaveChanges();
                    }
                    foreach (var item in gamePricingViewModels.gamePricings)
                    {
                        if (item.NoOfTicket > 0)
                        {
                            RandomBonu addobj = new RandomBonu();
                            addobj.NoOfTicket = item.NoOfTicket;
                            addobj.WinAmount = item.WinAmount;
                            addobj.SoldTicket = 0;
                            db.RandomBonus.Add(addobj);
                            db.SaveChanges();
                        }
                    }
                    TempData["actionresponse"] = JsonConvert.SerializeObject(new ApiResponse { Code = (int)ApiResponseCode.ok, Msg = Applicationstring.Success });
                }
                else
                {
                    TempData["actionresponse"] = JsonConvert.SerializeObject(new ApiResponse { Code = (int)ApiResponseCode.fail, Msg = Applicationstring.InvalidModel });
                }

            }
            catch (Exception ex)
            {
                TempData["actionresponse"] = JsonConvert.SerializeObject(new ApiResponse { Code = (int)ApiResponseCode.fail, Msg = ex.Message });
            }
            return RedirectToAction("Managerandombonus");
        }
        [HttpPost]
        public JsonResult UpdateCommison([FromBody] UpdateModel updateModel)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(updateModel.type) && updateModel.value >= 0)
                {
                  
                    if (updateModel.type == CashBackType.onboard.ToString())
                    {
                        var offer = db.Offers.Where(a => a.IsActive && a.CashBackType==(int)CashBackType.onboard).FirstOrDefault();
                        if(offer!=null)
                        {
                            offer.CashBackPercent = updateModel.value;
                            db.SaveChanges();
                        }
                        else
                        {
                            offer = new Offer();
                            offer.CashBackType = (int)CashBackType.onboard;
                            offer.CashBackPercent = updateModel.value;
                            offer.IsActive = true;
                            db.Offers.Add(offer);
                            db.SaveChanges();
                        }
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;

                    }
                    else if (updateModel.type == CashBackType.occasionally.ToString())
                    {
                        var offer = db.Offers.Where(a => a.IsActive && a.CashBackType == (int)CashBackType.occasionally).FirstOrDefault();
                        if(offer!=null)
                        {
                            offer.CashBackPercent = updateModel.value;
                            db.SaveChanges();
                        }
                        else
                        {
                            offer = new Offer();
                            offer.CashBackType = (int)CashBackType.occasionally;
                            offer.CashBackPercent = updateModel.value;
                            offer.IsActive = true;
                            db.Offers.Add(offer);
                            db.SaveChanges();
                        }
                      
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = Applicationstring.Successfullyupdated;

                    }
                    else if (updateModel.type == CashBackType.minimumwithdraw_amount.ToString())
                    {
                        var offer = db.Offers.Where(a => a.IsActive && 
                        a.CashBackType == (int)CashBackType.minimumwithdraw_amount).FirstOrDefault();
                        if (offer != null)
                        {
                            offer.CashBackPercent = updateModel.value;
                            db.SaveChanges();
                        }
                        else
                        {
                            offer = new Offer();
                            offer.CashBackType = (int)CashBackType.minimumwithdraw_amount;
                            offer.CashBackPercent = updateModel.value;
                            offer.IsActive = true;
                            db.Offers.Add(offer);
                            db.SaveChanges();
                        }

                        resp.Code = (int)ApiResponseCode.ok;
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
                resp.Code = (int)ApiResponseCode.fail;
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        [HttpPost]
        public JsonResult UpdateCashBack([FromBody] UpdateModel updateModel)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (updateModel.Id>0)
                {

                    var randomBounus = db.RandomBonus.Where(a => a.Id== updateModel.Id).FirstOrDefault();
                    if (randomBounus != null)
                    {
                        randomBounus.WinAmount = updateModel.value;
                        db.SaveChanges();
                        resp.Code = (int)ApiResponseCode.ok;
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
                resp.Code = (int)ApiResponseCode.fail;
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }
        public ActionResult GetWithdrawRequets()
        {
            List<WalletAccountWithralRequestModel> model = new List<WalletAccountWithralRequestModel>();
          
            try
            {
                int userid = userSession.Id;

                model = (from r in db.WalletTransactions
                         join e in db.Users
                            on r.UserId equals e.Id
                         where (r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountAgent 
                         || r.TransferType == (int)WalletTransactionType.WithdrawalWalletAmountSuperAgent
                         || r.TransferType == (int)WalletTransactionType.Transfer_To_Account) 
                         && (r.Status == (int)WalletTransactionStatusType.TransactionInitiated 
                         || r.Status == (int)WalletTransactionStatusType.TransactionPending)
                         orderby r.InsertDate descending 
                         select new WalletAccountWithralRequestModel
                         {
                             Id = r.Id,
                             Amount = r.Amount,
                             RequestedBy = e.Name,
                             RequestedDate = r.InsertDate,
                             TransactionId = r.TransactionId,
                             TransactionType = r.TransType,
                             ApprovalStatus = r.Status,
                             Email=e.Email,
                             PhoneNo=e.ContactNo,

                         }).ToList();


            }
            catch (Exception ex)
            {
            }

             return PartialView("/Areas/Admin/Views/AdminDashboard/_GetWithdrawRequets.cshtml", model);
        }

        [HttpPost]
        public JsonResult ApproveRequest(int id = 0)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (id > 0)
                {
                    var check = db.WalletTransactions.Where(x => x.Id == id).FirstOrDefault();
                    if (check != null)
                    {
                        check.Status = (int)WalletTransactionStatusType.TransactionPending;

                        if (db.SaveChanges() > 0)
                        {
                            var user = db.Users.Where(x => x.Id == check.UserId).FirstOrDefault();
                            string SincerelyName = "QuickBetCore Team";
                            String Subject = "Fund approval mail";

                            String Body = "Hi " + user.Name + "<br><br>" + "Your amount Withdraw request of amount" + check.Amount + " has been approved,Please open login your portal and accept the request if you got money and reject in case you did not recieve the money " +
                            "<br><br>Sincerely,<br>" + SincerelyName;
                            CommonFunction.SendEmail(user.Email, user.Name, 1, Subject, Body);

                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = "Successfully done";

                        }
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "Please try again!";

                    }

                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    response.Msg = "Something went wrong!";
                }
            }
            catch (Exception ex)
            {
                response.Code = (int)ApiResponseCode.fail;
                response.Msg = "Something went wrong!";
            }

            return Json(response);
        }
    }
}