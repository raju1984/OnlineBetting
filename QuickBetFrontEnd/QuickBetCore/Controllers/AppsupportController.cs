using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.APIAuth;
using QuickBetCore.Models.MobileAppModel;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [CustomAuthorizeFilter]
    public class AppsupportController : ControllerBase
    {
        QuickbetDbEntities db = new QuickbetDbEntities();

        // GET: api/Appsupport/GetSupportTicket?UserId=
        [HttpGet]
        public IActionResult GetSupportTicket(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            List<SupportTicketViewModel> result = new List<SupportTicketViewModel>();
            try
            {

                result = (from r in db.SupportTickeds
                          where r.UserId == UserId &&
                          r.Status != (int)supportTicketStatus.deleted
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
            resp.data = result;
            return Ok(resp);
        }


        // POST: api/Appsupport/GetSupportTicket?UserId=
        [HttpPost]
        public IActionResult AddSupportTicket([FromBody]AddsupportTicketModel addsupportTicketModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    SupportTicked objticket = new SupportTicked();
                    objticket.Title = addsupportTicketModel.title;
                    objticket.UserId = addsupportTicketModel.UserId;
                    objticket.CreatedDate = DateTime.UtcNow;
                    objticket.SupportTicketId = Guid.NewGuid().ToString();
                    objticket.Description = addsupportTicketModel.message;
                    objticket.Status = (int)supportTicketStatus.Open;
                    db.SupportTickeds.Add(objticket);
                    db.SaveChanges();
                    resp.code = (int)ApiResponseCode.ok;
                    resp.message = "SupportTicket created successfully!";
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(resp);
        }


        // GET: api/Appsupport/GetCloseSupportTicket?SupportTicketId=
        [HttpGet]
        public IActionResult GetCloseSupportTicket(int SupportTicketId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                SupportTicked objticket = db.SupportTickeds.Where(a => a.Id == SupportTicketId).FirstOrDefault();
                objticket.Status = (int)supportTicketStatus.closebyuser;
                db.SaveChanges();
                resp.code = (int)ApiResponseCode.ok;
                resp.message = "SupportTicket closed successfully!";
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            resp.data = resp;
            return Ok(resp);
        }


        // GET: api/Appsupport/GetSupportTicketChatHistory?SupportTicketId=
        [HttpGet]
        public IActionResult GetSupportTicketChatHistory(int SupportTicketId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            resp.metadescription = "Status: closebyadmin = 0 or 3,Open = 1";
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
                    var chathistory = db.SupportChatHistories.Include("GenerateByUser").Where(r => r.GenerateByUser != null
                    && r.SupportTicketId == SupportTicketId
                    && r.Status == (int)supportChatStatus.active).ToList();

                    model.Chats = new List<SupportChatHistoryModel>();
                    model.Chats = (from r in chathistory
                                   select new SupportChatHistoryModel
                                   {
                                       Id = r.Id,
                                       Message = r.Meesage,
                                       profile = !string.IsNullOrEmpty(r.GenerateByUser.ProfilePicture) ? r.GenerateByUser.ProfilePicture : "~/Areas/User/userassets/img/avatar.jpg",
                                       GenerateByName = r.GenerateByUser.Name + "(" + r.GenerateByUser.Email + ")",
                                       GeneratedById = r.GenerateByUserId,
                                       Created = r.CreatedDate
                                   }).OrderBy(a => a.Created).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new SupportTicketViewModel();
            }
            resp.data = model;
            return Ok(resp);
        }


        // POST: api/Appsupport/AddChatToSupportTicket
        [HttpPost]
        public IActionResult AddChatToSupportTicket(AddChatToSupportTicketModel addChatToSupportTicketModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    SupportChatHistory objticket = new SupportChatHistory();
                    objticket.GenerateByUserId = addChatToSupportTicketModel.UserId;
                    objticket.SupportTicketId = addChatToSupportTicketModel.SupportTicketId;
                    objticket.CreatedDate = DateTime.UtcNow;
                    objticket.Meesage = addChatToSupportTicketModel.message;
                    objticket.Status = (int)supportChatStatus.active;
                    db.SupportChatHistories.Add(objticket);
                    db.SaveChanges();
                    resp.code = (int)ApiResponseCode.ok;
                    resp.message = Applicationstring.Success;
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(resp);
        }

        // GET: api/Appsupport/GetDisputeTicket?UserId=
        [HttpGet]
        public IActionResult GetDisputeTicket(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            List<SupportTicketViewModel> result = new List<SupportTicketViewModel>();
            try
            {

                result = (from r in db.DisputeTickeds
                          where r.UserId == UserId && r.Status != (int)disputeTicketStatus.deleted
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
            resp.data = result;
            return Ok(resp);
        }

        // POST: api/Appsupport/AddDisputeTicket
        [HttpPost]
        public IActionResult AddDisputeTicket(AdddisputeTicketModel adddisputeTicketModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            resp.metadescription = "{DisputeType:dispute_on_wallaet_transaction = 0,dispute_on_lottery_ticket = 1}";
            try
            {
                if (ModelState.IsValid)
                {
                    if (Enum.IsDefined(typeof(DisputeType), adddisputeTicketModel.disputetype))
                    {
                        DisputeTicked objticket = new DisputeTicked();
                        objticket.Title = adddisputeTicketModel.title;
                        objticket.UserId = adddisputeTicketModel.UserId;
                        objticket.CreatedDate = DateTime.UtcNow;
                        objticket.DisputeReferenceId = adddisputeTicketModel.referenceId;
                        objticket.DisputeType = adddisputeTicketModel.disputetype;
                        objticket.DisputeTicketId = Guid.NewGuid().ToString();
                        objticket.Description = adddisputeTicketModel.message;
                        objticket.Status = (int)supportTicketStatus.Open;
                        db.DisputeTickeds.Add(objticket);
                        db.SaveChanges();
                        resp.code = (int)ApiResponseCode.ok;
                        resp.message = "dispute created successfully!";
                    }
                    else
                    {
                        resp.message = "please check disputetype parameter";
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(resp);
        }


        // GET: api/Appsupport/GetCloseDisputeTicket?DisputeTicketId=
        [HttpGet]
        public IActionResult GetCloseDisputeTicket(int DisputeTicketId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                DisputeTicked objticket = db.DisputeTickeds.Where(a => a.Id == DisputeTicketId).FirstOrDefault();
                objticket.Status = (int)supportTicketStatus.closebyuser;
                db.SaveChanges();
                resp.code = (int)ApiResponseCode.ok;
                resp.message = "Dispute closed successfully!";
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            resp.data = resp;
            return Ok(resp);
        }


        // GET: api/Appsupport/GetDisputeTicketChatHistory?DisputeTicketId=
        [HttpGet]
        public IActionResult GetDisputeTicketChatHistory(int DisputeTicketId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            resp.metadescription = "Status: closebyadmin = 0 or 3,Open = 1";
            SupportTicketViewModel model = new SupportTicketViewModel();
            try
            {
                model = (from r in db.DisputeTickeds
                         where r.Id == DisputeTicketId && r.Status != (int)disputeTicketStatus.deleted
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
                    var chathistory = db.DisputeChatHistories.Include("GenerateByUser").Where(r => r.GenerateByUser != null
                    && r.DisputeTicketId == DisputeTicketId &&
                    r.Status == (int)supportChatStatus.active).ToList();

                    model.Chats = new List<SupportChatHistoryModel>();
                    model.Chats = (from r in chathistory
                                   select new SupportChatHistoryModel
                                   {
                                       Id = r.Id,
                                       Message = r.Meesage,
                                       profile = !string.IsNullOrEmpty(r.GenerateByUser.ProfilePicture) ? r.GenerateByUser.ProfilePicture : "~/Areas/User/userassets/img/avatar.jpg",
                                       GenerateByName = r.GenerateByUser.Name + "(" + r.GenerateByUser.Email + ")",
                                       GeneratedById = r.GenerateByUserId,
                                       Created = r.CreatedDate
                                   }).OrderBy(a => a.Created).ToList();
                }
            }
            catch (Exception ex)
            {
                model = new SupportTicketViewModel();
            }
            resp.data = model;
            return Ok(resp);
        }


        // POST: api/Appsupport/AddChatToDisputeTicket
        [HttpPost]
        public IActionResult AddChatToDisputeTicket(AddChatToSupportTicketModel addChatToSupportTicketModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    DisputeChatHistory objticket = new DisputeChatHistory();
                    objticket.GenerateByUserId = addChatToSupportTicketModel.UserId;
                    objticket.DisputeTicketId = addChatToSupportTicketModel.SupportTicketId;
                    objticket.CreatedDate = DateTime.UtcNow;
                    objticket.Meesage = addChatToSupportTicketModel.message;
                    objticket.Status = (int)supportChatStatus.active;
                    db.DisputeChatHistories.Add(objticket);
                    db.SaveChanges();
                    resp.code = (int)ApiResponseCode.ok;
                    resp.message = Applicationstring.Success;
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(resp);
        }
    }
}
