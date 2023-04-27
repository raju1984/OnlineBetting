using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.Areas.Agent.Data;
using QuickBetCore.Areas.User.Data;
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
    public class SuperAgentFundsController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SuperAgentFundsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: SuperAgentFunds/Funds
        public ActionResult Index()
        {
            var User = db.Users.Where(x => x.Id == userSession.Id).FirstOrDefault();
            if (User != null)
            {
                ViewBag.BalanceAmount = User.MyWalletbalance;
            }
            return View();
        }
        //SuperAgent/SuperAgentFunds/AddBankDetail
        public ActionResult ManageBankDetail()
        {
            return View();
        }
        //SuperAgent/SuperAgentFunds/GetBankList
        public IActionResult GetBankList()
        {

            List<BankdetailViewModel> model = new List<BankdetailViewModel>();
            try
            {
                model = (from r in db.BankDetails
                         where r.Isdeleted == false && r.UserId == userSession.Id
                         select new BankdetailViewModel
                         {
                             Id = r.Id,
                             bankname = r.BankName,
                             accountname = r.AccountName,
                             accountnumer = r.AccountNumber,
                             isdefault = r.IsDefault
                         }).ToList();
                return PartialView("_BankList", model);
            }
            catch (Exception ex)
            {

            }
            return PartialView("_BankList");
        }
        //SuperAgent/SuperAgentFunds/AddBankDetail
        public JsonResult AddBankDetail([FromBody] AddBankDetail addBank)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(addBank.accoutname) && !string.IsNullOrEmpty(addBank.accountnumber)
                    && !string.IsNullOrEmpty(addBank.bankname))
                {
                    BankDetail objdetail = new BankDetail();
                    objdetail.AccountName = addBank.accoutname;
                    objdetail.BankName = addBank.bankname;
                    objdetail.AccountNumber = addBank.accountnumber;
                    objdetail.UserId = userSession.Id;
                    int conunt = db.BankDetails.Where(a => a.UserId == userSession.Id).Count();
                    if (conunt == 0)
                    {
                        objdetail.IsDefault = true;
                    }
                    db.BankDetails.Add(objdetail);
                    db.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                }
                else
                {
                    resp.Msg = "Invalid Model!";
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });

        }

        public JsonResult UpdateBankDetail([FromBody] UpdateBankDetail updateBank)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (updateBank.type == 1 && updateBank.bnkId > 0)
                {
                    var banklist = db.BankDetails.Where(a => a.UserId == userSession.Id).ToList();
                    if (banklist != null && banklist.Where(a => a.Id == updateBank.bnkId).Count() > 0)
                    {
                        foreach (var item in banklist)
                        {
                            if (item.Id == updateBank.bnkId)
                            {
                                item.IsDefault = true;
                            }
                            else
                            {
                                item.IsDefault = false;
                            }
                        }
                    }
                    db.SaveChanges();
                }
                else if (updateBank.type == 0 && updateBank.bnkId > 0)
                {
                    var bankobj = db.BankDetails.Where(a => a.Id == updateBank.bnkId).FirstOrDefault();
                    if (bankobj != null)
                    {
                        bankobj.Isdeleted = true;
                        db.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });

        }

        public JsonResult withdrawalBalanceRequest([FromBody] WithdrawModel withdraw)
        {
            MyBal resp = new MyBal();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (withdraw.amount >= 100)
                {
                    if (PaymentDb.RequestWalletAmountRedeemSuperAgentToAdmin(withdraw.amount, userSession.Id))
                    {
                        resp.Code = (int)ApiResponseCode.ok;
                        resp.Msg = "Requests have been made and Please contact to Admin to approve this request.";


                        decimal Bal = PaymentDb.GetBalance(userSession.Id);
                        resp.MyWallet = Bal;
                    }
                    else
                    {
                        resp.Msg = "Sorry we are not able take your request right now ,please try again later!";
                    }

                }
                else
                {
                    resp.Msg = "Please enter valid amount!";
                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }
    }
}