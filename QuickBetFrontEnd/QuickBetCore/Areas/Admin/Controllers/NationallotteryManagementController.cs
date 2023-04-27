using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class NationallotteryManagementController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public NationallotteryManagementController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/NationallotteryManagement
        public ActionResult Index()
        {
            AdminDashboardViewModel model = new AdminDashboardViewModel();

            model.TotalActiveNationallotteryUser = db.Users.Where(a => a.UserType == (int)UserType.Nationallottery
               && a.UserStatus == (int)UserStatus.active).Count();

            model.BlockedNationallotteryUser = db.Users.Where(a => a.UserType == (int)UserType.Nationallottery
              && a.UserStatus == (int)UserStatus.block).Count();
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //(ApiResponse)TempData["ActionResponse"];
            }
            return View(model);
        }
        public ActionResult GetNationallotterylist(string status = "active")
        {
            
            List<UserListViewModel> model = new List<UserListViewModel>();
            try
            {
                List<QuickBetCore.DatabaseEntity.User> users = new List<QuickBetCore.DatabaseEntity.User>();

                if (status == "active")
                {
                    users = db.Users.Where(a => a.UserType == (int)UserType.Nationallottery && a.UserStatus == (int)UserStatus.active).ToList();
                }
                else
                {
                    users = db.Users.Where(a => a.UserType == (int)UserType.Nationallottery && a.UserStatus == (int)UserStatus.block).ToList();
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
                             }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return PartialView("_Nationallottery", model);
        }

        public ActionResult CreateNationallottery()
        {
            CustomerUploadModel customer = new CustomerUploadModel();
            if (TempData["ActionResponse"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);// (ApiResponse)TempData["ActionResponse"];
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdateNationallottery(CustomerUploadModel model)
        {
            ApiResponse response = new ApiResponse();
            if (ModelState.IsValid)
            {
                if (Request.Form.Files.Count > 0)
                {
                    string uploadsFolder = webHostEnvironment.WebRootPath+"/Content/Images";
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
                response = CustomerOperation.CreateUpdateCustomer(model, (int)UserType.Nationallottery);
            }
            else
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Invalid Request";

            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            if (model.Id > 0)
            {
                return RedirectToAction("Index", "NationallotteryManagement", new { area = "Admin" });
            }
            return RedirectToAction("CreateNationallottery", "NationallotteryManagement", new { area = "Admin" });
        }
    }
}