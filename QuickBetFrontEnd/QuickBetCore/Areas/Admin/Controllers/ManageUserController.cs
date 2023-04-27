using Microsoft.AspNetCore.Hosting;
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

    public class ManageUserController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ManageUserController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/ManageUser
        public ActionResult Index()
        {
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);// (ApiResponse)TempData["ActionResponse"];
                }
            }
            catch (Exception ex)
            {
            }
            return View();
        }


        public ActionResult ManageBanners()
        {
            ManagePagesModel model = new ManagePagesModel();
            
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]);// (ApiResponse)TempData["ActionResponse"];
                }
                var pages = db.Bannermanages.Where(x => x.Id == 1).FirstOrDefault();
                if (pages != null)
                {
                    model.Id = pages.Id;
                    model.AppleStoreLink = pages.PlaystoreLink;
                    model.GooglePlayLink = pages.GoogleplayLink;
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateBanner(ManagePagesModel model)
        {
            
            ApiResponse response = new ApiResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id > 0)
                    {
                        var banner = db.Bannermanages.Where(x => x.Id == model.Id).FirstOrDefault();
                        if (banner != null)
                        {
                            banner.PlaystoreLink = model.AppleStoreLink;
                            banner.GoogleplayLink = model.GooglePlayLink;
                            if (db.SaveChanges() > 0)
                            {
                                response.Code = (int)ApiResponseCode.ok;
                                response.Msg = "Updated Successfully..";
                            }
                        }
                    }
                    else
                    {

                        Bannermanage banner = new Bannermanage()
                        {
                            GoogleplayLink = model.GooglePlayLink,
                            PlaystoreLink = model.AppleStoreLink
                        };

                         db.Bannermanages.Add(banner);
                        if (db.SaveChanges() > 0)
                        {
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = "Inserted Successfully..";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            return RedirectToAction("ManageBanners", "ManageUser");
        }
    }
}