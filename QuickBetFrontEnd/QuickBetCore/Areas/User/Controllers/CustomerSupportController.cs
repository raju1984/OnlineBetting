using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.Areas.Agent.Data;
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
    public class CustomerSupportController : Controller
    {
       
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomerSupportController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/CustomerSupport
        public IActionResult Index()
        {
            List<SupportTicketViewModel> result = new List<SupportTicketViewModel>();
            try
            {

                result = (from r in db.SupportTickeds
                          where r.UserId == userSession.Id && r.Status != (int)supportTicketStatus.deleted
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
            return View("Support", result);
        }

        public JsonResult AddSupportTicket([FromBody] SupportTicketModel supportTicket)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                SupportTicked objticket = new SupportTicked();
                objticket.Title = supportTicket.title;
                objticket.UserId = userSession.Id;
                objticket.CreatedDate = DateTime.UtcNow;
                objticket.SupportTicketId = Guid.NewGuid().ToString();
                objticket.Description = supportTicket.message;
                objticket.Status = (int)supportTicketStatus.Open;
                db.SupportTickeds.Add(objticket);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }
        public JsonResult CloseSupportTicket(int SupportTicketId)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                SupportTicked objticket = db.SupportTickeds.Where(a => a.Id == SupportTicketId).FirstOrDefault();
                objticket.Status = (int)supportTicketStatus.closebyuser;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        public IActionResult GetSupportChatHistory(int SupportTicketId)
        {

            SupportTicketViewModel model = new SupportTicketViewModel();
            try
            {
                model = (from r in db.SupportTickeds
                         where r.Id == SupportTicketId && r.Status != (int)supportTicketStatus.deleted
                         select new SupportTicketViewModel
                         {
                             Id = r.Id,
                             Title = r.Title,
                             Description = r.Description,
                             CreatedDate = r.CreatedDate,
                             Status = r.Status,
                         }).FirstOrDefault();
                if (model != null)
                {
                    var chathistory = db.SupportChatHistories.Include("GenerateByUser").Where(r => r.GenerateByUser != null && r.SupportTicketId == SupportTicketId && r.Status == (int)supportChatStatus.active).ToList();
                    model.Chats = new List<SupportChatHistoryModel>();
                    model.Chats = (from r in chathistory
                                   select new SupportChatHistoryModel
                                   {
                                       Id = r.Id,
                                       Message = r.Meesage,
                                       profile = !string.IsNullOrEmpty(r.GenerateByUser.ProfilePicture) ? r.GenerateByUser.ProfilePicture : "/Areas/User/userassets/img/avatar.jpg",
                                       GenerateByName = r.GenerateByUser.Name + "(" + r.GenerateByUser.Email + ")",
                                       GeneratedById = r.GenerateByUserId,
                                       Created = r.CreatedDate
                                   }).OrderBy(a => a.Created).ToList();
                }

                return PartialView("_SupportChatHistory", model);
            }
            catch (Exception ex)
            {
                model = new SupportTicketViewModel();
            }
            return PartialView("_SupportChatHistory", model);
        }

        public JsonResult AddChatToSupportTicket(int SupportTicketId, string message)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                SupportChatHistory objticket = new SupportChatHistory();
                objticket.GenerateByUserId = userSession.Id;
                objticket.SupportTicketId = SupportTicketId;
                objticket.CreatedDate = DateTime.UtcNow;
                objticket.Meesage = message;
                objticket.Status = (int)supportChatStatus.active;
                db.SupportChatHistories.Add(objticket);
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
