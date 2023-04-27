using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
namespace Quickbet.Areas.SuperAgent.Controllers
{
    [TypeFilter(typeof(CheckSuperAgentSessionExpire))]
    public class SuperAgentTransactionsHistoryController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SuperAgentTransactionsHistoryController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: SuperAgent/SuperAgentTransactionsHistory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTransactionHistory(string viewType = "mm", string TransactionType = "pending")
        {
            List<TransactionHistoryViewModel> result = new List<TransactionHistoryViewModel>();
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

                if (TransactionType == "pending")
                {
                    result = (from r in db.WalletTransactions
                              where r.UserId == userSession.Id && r.Status == (int)WalletTransactionStatusType.TransactionInitiated
                              select new TransactionHistoryViewModel
                              {
                                  Id = r.Id,
                                  TransactionId = r.TransactionId,
                                  Date = r.InsertDate,
                                  Amount = r.Amount,
                                  FinalBalanceAmount = r.ClosingBalance,
                                  TransactionType = r.TransType,
                                  TransactionStatus = r.Status,
                                  Remark = r.TransactionRemark
                              }).OrderByDescending(a => a.Date).ToList();
                }
                else
                {
                    if (viewType == ViewFilters.LifeTime)
                    {
                        result = (from r in db.WalletTransactions
                                  where r.UserId == userSession.Id
                                  select new TransactionHistoryViewModel
                                  {
                                      Id = r.Id,
                                      TransactionId = r.TransactionId,
                                      Date = r.InsertDate,
                                      Amount = r.Amount,
                                      FinalBalanceAmount = r.ClosingBalance,
                                      TransactionType = r.TransType,
                                      TransactionStatus = r.Status,
                                      Remark = r.TransactionRemark
                                  }).OrderByDescending(a => a.Date).ToList();
                    }
                    else
                    {
                        result = (from r in db.WalletTransactions
                                  where r.UserId == userSession.Id && r.InsertDate.Date >= filterdate.Date
                                  select new TransactionHistoryViewModel
                                  {
                                      Id = r.Id,
                                      TransactionId = r.TransactionId,
                                      Date = r.InsertDate,
                                      Amount = r.Amount,
                                      FinalBalanceAmount = r.ClosingBalance,
                                      TransactionType = r.TransType,
                                      TransactionStatus = r.Status,
                                      Remark = r.TransactionRemark
                                  }).OrderByDescending(a => a.Date).ToList();
                    }

                }

                ViewData["balance"] = PaymentDb.GetBalance(userSession.Id);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_TransactionHistory", result);
        }
    }
}