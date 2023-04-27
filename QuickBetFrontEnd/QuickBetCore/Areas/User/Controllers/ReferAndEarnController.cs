using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Controllers
{
    [TypeFilter(typeof(CheckUserSessionExpire))]
    public class ReferAndEarnController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReferAndEarnController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/ReferAndEarn
        public IActionResult Index()
        {
            ReferAndEarnViewModel model = new ReferAndEarnViewModel();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var getReferCode = db.Users.Where(x => x.Id == userSession.Id).FirstOrDefault();
                    if (getReferCode != null)
                    {
                        model.Id = getReferCode.Id;
                        if (getReferCode.ReferCode == null)
                        {
                            model.ReferCode = "QB" + CommonFunction.GenerateReferalCode();
                            // model.ReferCode = "116694";

                            var check = db.Users.Any(x => x.ReferCode.ToLower().Trim() == model.ReferCode.ToLower().Trim());

                            if (check == false)
                            {
                                getReferCode.ReferCode = model.ReferCode.Trim();
                                db.SaveChanges();
                            }
                            else
                            {
                                model.ReferCode = "";
                            }

                        }
                        else
                        {
                            model.ReferCode = getReferCode.ReferCode;
                        }
                    }

                    var ReferAndEarn = db.UserReferDetails.Where(x => x.Id == 1).FirstOrDefault();

                    if (ReferAndEarn != null)
                    {
                        ViewBag.ReferAndEarn = ReferAndEarn;
                    }

                    model.Refer = (from dd in db.Users
                                   join ee in db.ReferUserMappings
                                on dd.Id equals ee.ReferedUserId
                                   where ee.ReferedFromId == userSession.Id
                                   select new ReferAndEarList
                                   {
                                       ReferedUsername = dd.Name,
                                       Email = dd.Email,
                                       Commission = ee.Percentage,
                                       ReferedDate = ee.ReferedDate
                                   }).ToList();


                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        public JsonResult CheckReferCodeandUpdate(string ReferCode)
        {
            ApiResponse response = new ApiResponse();
            response.Code = (int)ApiResponseCode.fail;
            try
            {
                response = DbOperation.CheckReferCodeandUpdate(userSession.Id, ReferCode);
            }
            catch (Exception ex)
            {
                response.Msg = ex.Message;
            }
            return Json(response);
        }

        [HttpPost]
        public JsonResult SendEmailToFriend([FromBody]ReferAndEarnViewModel model)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    response= DbOperation.SendInvitationEmail(userSession.Id, model.Email);
                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    response.Msg = "Please fill all the details!";


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
