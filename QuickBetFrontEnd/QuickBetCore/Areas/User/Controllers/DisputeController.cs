using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.User.Controllers
{
    [TypeFilter(typeof(CheckUserSessionExpire))]
    public class DisputeController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DisputeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/Dispute
        public IActionResult Index()
        {
            List<SupportTicketViewModel> result = new List<SupportTicketViewModel>();
            try
            {

                result = (from r in db.DisputeTickeds
                          where r.UserId == userSession.Id && r.Status != (int)disputeTicketStatus.deleted
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
        //
        public JsonResult AddDisputeTicket(string title, string message, int referenceId, int disputetype)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DisputeTicked objticket = new DisputeTicked();
                objticket.Title = title;
                objticket.UserId = userSession.Id;
                objticket.CreatedDate = DateTime.UtcNow;
                objticket.DisputeReferenceId = referenceId;
                objticket.DisputeType = disputetype;
                objticket.DisputeTicketId = Guid.NewGuid().ToString();
                objticket.Description = message;
                objticket.Status = (int)supportTicketStatus.Open;
                db.DisputeTickeds.Add(objticket);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }
        public JsonResult CloseDisputeTicket(int SupportTicketId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                DisputeTicked objticket = db.DisputeTickeds.Where(a => a.Id == SupportTicketId).FirstOrDefault();
                objticket.Status = (int)supportTicketStatus.closebyuser;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        public IActionResult GetDisputeChatHistory(int SupportTicketId)
        {

            SupportTicketViewModel model = new SupportTicketViewModel();
            try
            {
                model = (from r in db.DisputeTickeds
                         join std in db.Users
                             on r.UserId equals std.Id
                         where r.Id == SupportTicketId && r.Status != (int)disputeTicketStatus.deleted
                         select new SupportTicketViewModel
                         {
                             Id = r.Id,
                             Title = r.Title,
                             Description = r.Description,
                             CreatedDate = r.CreatedDate,
                             Status = r.Status,
                             Name = std.Name + "(" + std.Email + ")",
                             UserProfilePic = !string.IsNullOrEmpty(std.ProfilePicture) ? r.User.ProfilePicture : "/Areas/User/userassets/img/avatar.jpg",
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
                                       profile = !string.IsNullOrEmpty(r.GenerateByUser.ProfilePicture) ? 
                                       r.GenerateByUser.ProfilePicture : "/Areas/User/userassets/img/avatar.jpg",
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
    }
}
