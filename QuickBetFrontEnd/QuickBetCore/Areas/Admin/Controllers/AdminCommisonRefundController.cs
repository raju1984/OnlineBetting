using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class AdminCommisonRefundController : Controller
    {
        // GET: Admin/AdminCommisonRefund
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminCommisonRefundController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        public ActionResult Index()
        {
            AdminCommisonViewModel model = new AdminCommisonViewModel();

            try
            {
                //List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
                //var commisonobj = db.Admincommissions.FirstOrDefault();
                //if (commisonobj != null)
                //{
                //    model.Commsion = commisonobj.AdminCommison;
                //}
                //result = (from r in db.AdmincommissionTransactions
                //          select new TransactionHistoryViewModel
                //          {
                //              TransactionId = r.HackshowTransactionId,
                //              Date = r.InsertDate,
                //              TransactionType = r.TransType,
                //              Amount = r.Amount,
                //              FinalBalanceAmount = r.ClosingBalance,
                //              Remark = r.Remark
                //          }).OrderByDescending(a => a.Date).ToList();
                //model.Commsiontransaction = result;
            }
            catch (Exception ex)
            {
                model.Commsion = 0;
                model.Commsiontransaction = new List<TransactionHistoryViewModel>(); ;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SetCommison(decimal commison)
        {
            try
            {
                //if (commison >= 0)
                //{
                //    var commisonobj = db.Admincommissions.FirstOrDefault();
                //    if (commisonobj == null)
                //    {
                //        Admincommission addobj = new Admincommission();
                //        addobj.AdminCommison = commison;
                //        db.Admincommissions.Add(addobj);
                //        db.SaveChanges();
                //    }
                //    else
                //    {
                //        commisonobj.AdminCommison = commison;
                //        db.SaveChanges();
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }


        public ActionResult RefundTransaction()
        {
            RefundMoneyViewModel result = new RefundMoneyViewModel();
            try
            {
                result.Users = (from r in db.Users
                                where r.UserStatus == (int)UserStatus.active && r.UserType == (int)UserType.Users
                                select new DropDownKeyValue
                                {
                                    Id = r.Id,
                                    Name = r.Name + "(" + r.Email + ")"
                                }).OrderBy(a => a.Name).ToList();

                result.Refundtransaction = (from r in db.WalletTransactions
                                            where r.AdminUserId != null && r.AdminUserId > 0
                                            && r.TransferType == (int)WalletTransactionType.Refund_By_Admin
                                            select new TransactionHistoryViewModel
                                            {
                                                TransactionId = r.TransactionId,
                                                Date = r.InsertDate,
                                                TransactionType = r.TransType,
                                                Amount = r.Amount,
                                                Name = r.User != null ? r.User.Name + "(" + r.User.Email + ")" : "",
                                                Remark = r.TransactionRemark
                                            }).OrderByDescending(a => a.Date).ToList();

            }
            catch (Exception ex)
            {

            }
            if (TempData["fundtranfermessage"] != null)
            {
                ViewData["fundtranfermessage"] = JsonConvert.DeserializeObject<string>((string)TempData["fundtranfermessage"]); 
                //TempData["fundtranfermessage"].ToString();
            }
            return View(result);
        }

        [HttpPost]
        public ActionResult RefundMoneyToPlayer(RefundMoneyToUserViewModel refundMoneyToUserViewModel)
        {
            RefundMoneyViewModel result = new RefundMoneyViewModel();
            try
            {
                if (refundMoneyToUserViewModel != null && refundMoneyToUserViewModel.ToUserId > 0 && refundMoneyToUserViewModel.amount > 0)
                {

                    var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == refundMoneyToUserViewModel.ToUserId).OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.TransactionId = Guid.NewGuid().ToString();
                    obj.UserId = refundMoneyToUserViewModel.ToUserId;
                    obj.TransferType = (int)WalletTransactionType.Refund_By_Admin;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = (int)TransType.Credit;
                    obj.Note = refundMoneyToUserViewModel.Message;
                    //Credit
                    obj.AdminUserId = userSession.Id;
                    obj.ClosingBalance = ClosingBalance + refundMoneyToUserViewModel.amount;
                    obj.Amount = refundMoneyToUserViewModel.amount;
                    obj.TransactionRemark = "you have refunded Amount:₦" + refundMoneyToUserViewModel.amount + " to your wallet by the QuickBetCoreTeam" + obj.Note;
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = db.Users.Where(a => a.Id == refundMoneyToUserViewModel.ToUserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    db.WalletTransactions.Add(obj);
                    db.SaveChanges();
                    TempData["fundtranfermessage"] = JsonConvert.SerializeObject("fund transfer successfully!");
                }
                else
                {
                    TempData["fundtranfermessage"] = JsonConvert.SerializeObject("Invalid Input");
                }
            }
            catch (Exception ex)
            {
                TempData["fundtranfermessage"] = JsonConvert.SerializeObject(ex.Message);
            }
            return RedirectToAction("RefundTransaction");
        }
    }
}