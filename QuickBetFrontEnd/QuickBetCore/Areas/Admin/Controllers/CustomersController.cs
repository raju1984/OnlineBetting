using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickBetCore.Areas.Admin.Data;
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
    public class CustomersController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CustomersController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/Customers
        public ActionResult Index()
        {
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //(ApiResponse)TempData["ActionResponse"];
            }
            return View();
        }

        public ActionResult GetCustomerlist()
        {
            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                var users = db.Users.Where(a => a.UserType == (int)UserType.Users
                && a.UserStatus != (int)UserStatus.deleted).ToList();

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
                             walletbalance = r.MyWalletbalance
                         }).ToList();
            }
            catch (Exception ex)
            {

            }
            return PartialView("_Customer", model);
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
                    resp = DbOperation.BlockAnything(unblockModel.type, unblockModel.Id);
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
        public JsonResult BlockUserSuperAgent([FromBody] BlockUnblockModel unblockModel)
        {
            dashboard resp = new dashboard();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (!string.IsNullOrEmpty(unblockModel.type) && unblockModel.Id > 0)
                {
                    resp = DbOperation.BlockAnythingSuperAgentAdmin(unblockModel.type, unblockModel.Id);
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
        public ActionResult CreateCustomer(int customerId = 0)
        {
            CustomerUploadModel customer = new CustomerUploadModel();
            if (customerId > 0)
            {
                customer = CustomerOperation.GetCustomer(customerId, (int)UserType.Users);
            }

            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //(ApiResponse)TempData["ActionResponse"];
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateCustomer(CustomerUploadModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    string uploadsFolder = webHostEnvironment.WebRootPath+ "/Content/Images";
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
                response = CustomerOperation.CreateUpdateCustomer(model, (int)UserType.Users);
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";

            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            if (model.Id > 0)
            {
                return RedirectToAction("Index", "Customers", new { Areas = "Admin" });
            }

            return RedirectToAction("CreateCustomer", "Customers", new { Areas = "Admin" });
        }

        [HttpPost]
        public JsonResult DeleteCustomer([FromBody] IdViewModel idView)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                resp = CustomerOperation.DeleteUser(idView.Id);

                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        [HttpPost]
        public JsonResult ResetCustomerPassword([FromBody] IdViewModel idView)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                resp = CustomerOperation.ResetUserPassword(idView.Id);

                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        public ActionResult TransferFunds(int customerId = 0)
        {
            FundTransferModel fundTransfer = new FundTransferModel();
            CustomerUploadModel customer = new CustomerUploadModel();
            ApiResponse resp = new ApiResponse();
            if (customerId > 0)
            {
                customer = CustomerOperation.GetCustomer(customerId, (int)UserType.Users);
                if (customer != null)
                {
                    fundTransfer.UserId = customer.Id;
                    fundTransfer.BeneficiaryName = customer.Name;
                }
                else
                {
                    resp.Code = (int)ApiResponseCode.fail; resp.Msg = "Invalid Request";
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                    return RedirectToAction("Index", "Customers", new { area = "Admin" });
                }
            }
            else
            {
                resp.Code = (int)ApiResponseCode.fail; resp.Msg = "Invalid Request";
                TempData["ActionResponse"] = JsonConvert.SerializeObject(resp);
                return RedirectToAction("Index", "Customers", new { area = "Admin" });
            }
            return View(fundTransfer);
        }

        [HttpPost]
        public ActionResult FundTrsansferToCustomer(FundTransferModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {
                
                response = CustomerOperation.TransferFund(model,userSession.Id);
                if(response.Code==(int)ApiResponseCode.ok)
                {
                    return RedirectToAction("TransactionReciept", "Customers", new { txn = response.txnId, area = "Admin" });
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";
                    TempData["ActionResponse"] = JsonConvert.SerializeObject(response); //response;
                    return RedirectToAction("TransferFunds", "Customers", new { customerId=model.UserId, area = "Admin" });
                }
               
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";
                TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
                return RedirectToAction("TransferFunds", "Customers", new { customerId = model.UserId, area = "Admin" });
            }
            
            
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
    }
}