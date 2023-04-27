using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class DisputManagementController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public DisputManagementController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/DisputManagement
        public ActionResult Index()
        {
            List<SupportTicketViewModel> result = new List<SupportTicketViewModel>();
            try
            {

                result = (from r in db.DisputeTickeds
                          where r.Status != (int)disputeTicketStatus.deleted
                          select new SupportTicketViewModel
                          {
                              Id = r.Id,
                              Title = r.Title,
                              Description = r.Description,
                              CreatedDate = r.CreatedDate,
                              Status = r.Status
                          }).OrderByDescending(a => a.CreatedDate).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }

        public JsonResult CloseSupportTicket(int SupportTicketId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DisputeTicked objticket = db.DisputeTickeds.Where(a => a.Id == SupportTicketId).FirstOrDefault();
                objticket.Status = (int)supportTicketStatus.closebyadmin;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }


        public ActionResult GetSupportChatHistory(int SupportTicketId)
        {

            SupportTicketViewModel model = new SupportTicketViewModel();
            try
            {
                model = (from r in db.DisputeTickeds
                         join std in db.Users
                             on r.UserId equals std.Id
                         where r.Status != (int)supportTicketStatus.deleted && r.Id == SupportTicketId
                         select new SupportTicketViewModel
                         {
                             Id = r.Id,
                             Title = r.Title,
                             Description = r.Description,
                             CreatedDate = r.CreatedDate,
                             Status = r.Status,
                             Name = std.Name,
                             UserProfilePic =!string.IsNullOrEmpty(std.ProfilePicture) ? r.User.ProfilePicture : "/Areas/User/userassets/img/avatar.jpg",
                             userEmail = std.Email

                         }).FirstOrDefault();
                if (model != null)
                {
                    var chathistory = db.DisputeChatHistories.Include("GenerateByUser").Where(r => r.GenerateByUser != null && r.DisputeTicketId == SupportTicketId && r.Status == (int)supportChatStatus.active).ToList();
                    model.Chats = new List<SupportChatHistoryModel>();
                    model.Chats = (from r in chathistory
                                   select new SupportChatHistoryModel
                                   {
                                       Id = r.Id,
                                       Message = r.Meesage,
                                       UserType = r.GenerateByUser.UserType,
                                       profile = !string.IsNullOrEmpty(r.GenerateByUser.ProfilePicture) ? r.GenerateByUser.ProfilePicture : "/Areas/User/userassets/img/avatar.jpg",
                                       GenerateByName = r.GenerateByUser.Name + "(" + r.GenerateByUser.Email + ")",
                                       GeneratedById = r.GenerateByUserId,
                                       Created = r.CreatedDate
                                   }).OrderBy(a => a.Created).ToList();
                }

                return PartialView("_DisputeChatHistory", model);
            }
            catch (Exception ex)
            {
                model = new SupportTicketViewModel();
            }
            return PartialView("_DisputeChatHistory", model);
        }

        public JsonResult AddChatToSupportTicket(int SupportTicketId, string message)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DisputeChatHistory objticket = new DisputeChatHistory();
                objticket.GenerateByUserId = userSession.Id;
                objticket.DisputeTicketId = SupportTicketId;
                objticket.CreatedDate = DateTime.UtcNow;
                objticket.Meesage = message;
                objticket.Status = (int)supportChatStatus.active;
                db.DisputeChatHistories.Add(objticket);
                db.SaveChanges();
                resp.Code = (int)ApiResponseCode.ok;

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        public ActionResult SupportTicket()
        {
            return View();
        }
    }
}