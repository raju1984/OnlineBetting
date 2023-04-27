using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.Areas.Admin.Data;
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
    public class UserManagementController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        // GET: Admin/UserManagement
        public ActionResult Index()
        {
            UserManagmentIndexViewModel model = new UserManagmentIndexViewModel();
            try
            {
                model.Accounts = (from r in db.AccountGroups
                                  where r.Isdeleted == false
                                  select new AccountGroupViewModel
                                  {
                                      Id = r.Id,
                                      Name = r.AccountName,
                                  }).ToList();
                var result = db.Users.Where(r => r.UserType == (int)UserType.Staff).ToList();
                if (result != null)
                {
                    model.Users = (from r in result
                                   where  r.UserType == (int)UserType.Staff && r.UserStatus!=(int)UserStatus.deleted
                                   select new UserManageModel
                                   {
                                       UserId = r.Id,
                                       Name = r.Name,
                                       Email = r.Email,
                                       Status=r.UserStatus,
                                       AccountType = r.AccountGroupUserMappings.FirstOrDefault() != null ? r.AccountGroupUserMappings.FirstOrDefault().AccountGroup.AccountName : "",
                                       AccountTypeId = r.AccountGroupUserMappings.FirstOrDefault() != null ? r.AccountGroupUserMappings.FirstOrDefault().AccountGroupId : 0,
                                   }).ToList();
                }

            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        public ActionResult ManagerGroup()
        {
            ManageAccountGroupViewModel model = new ManageAccountGroupViewModel();
            model.Pages = CommonMetadata.Pages();
            return View(model);
        }
        public ActionResult EditGroup(int GpId)
        {
            ManageAccountGroupViewModel model = new ManageAccountGroupViewModel();
            try
            {
                var gps = db.AccountGroups.Where(a => a.Id == GpId).FirstOrDefault();
                model.Id = gps.Id;
                model.Name = gps.AccountName;
                model.Pages = CommonMetadata.Pages();
                model.UserPermissonPages = new List<DropDownKeyValue>();
                var UserPermissonPages = db.AccountGroupObjectMappings.Where(a => a.AccountGroupId == GpId).ToList();
                if (UserPermissonPages != null && UserPermissonPages.Count() > 0)
                {
                    foreach (var item in model.Pages)
                    {
                        int count = UserPermissonPages.Where(a => a.ObjectType == item.Id).Count();
                        if (count > 0)
                        {
                            model.UserPermissonPages.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("ManagerGroup");
            }
            return View("ManagerGroup", model);
        }
        public ActionResult ManagerUsers()
        {

            UserManageModel model = new UserManageModel();
            model.AccountGp = (from r in db.AccountGroups
                               where r.Isdeleted == false
                               select new DropDownKeyValue
                               {
                                   Id = r.Id,
                                   Name = r.AccountName
                               }).ToList();

            return View(model);
        }
        public ActionResult EditManagerUsers(int StaffId)
        {
            UserManageModel model = new UserManageModel();
            try
            {
                var userobj = db.Users.Where(a => a.Id == StaffId).FirstOrDefault();
                model.UserId = userobj.Id;
                model.Name = userobj.Name;
                model.Email = userobj.Email;
                AccountGroupUserMapping mapping = db.AccountGroupUserMappings.Where(a => a.UserId == StaffId).FirstOrDefault();
                if (mapping != null)
                {
                    model.AccountType = mapping.AccountGroup.AccountName;
                    model.AccountTypeId = mapping.AccountGroupId;
                }
                model.AccountGp = (from r in db.AccountGroups
                                   where r.Isdeleted == false
                                   select new DropDownKeyValue
                                   {
                                       Id = r.Id,
                                       Name = r.AccountName
                                   }).ToList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("ManagerUsers");
            }
            return View("ManagerUsers", model);
        }
        [HttpPost]
        public JsonResult AddAccountGroup([FromBody]ManageAccountGroupViewModel gpmodel)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                //pagepermission = "1,2,3,4";
                if (gpmodel != null && !string.IsNullOrEmpty(gpmodel.Name))
                {
                    if (gpmodel.Id > 0)
                    {
                        AccountGroup gp = db.AccountGroups.Where(a => a.Id == gpmodel.Id).FirstOrDefault();
                        gp.AccountName = gpmodel.Name;
                        var permission = db.AccountGroupObjectMappings.Where(a => a.AccountGroupId == gpmodel.Id).ToList();
                        db.AccountGroupObjectMappings.RemoveRange(permission);
                        db.SaveChanges();
                        if (gpmodel.Pages != null && gpmodel.Pages.Count() > 0)
                        {
                            List<AccountGroupObjectMapping> maps = new List<AccountGroupObjectMapping>();
                            foreach (var item in gpmodel.Pages)
                            {
                                if (item.Id > 0)
                                {
                                    AccountGroupObjectMapping objmap = new AccountGroupObjectMapping();
                                    objmap.AccountGroup = gp;
                                    objmap.ObjectType = item.Id;
                                    maps.Add(objmap);
                                }
                            }
                            resp.Code = (int)ApiResponseCode.ok;
                            db.AccountGroupObjectMappings.AddRange(maps);
                            db.SaveChanges();
                        }
                    }
                    else
                    {

                        AccountGroup gp = new AccountGroup();
                        gp.AccountName = gpmodel.Name;
                        gp.Isdeleted = false;
                        db.AccountGroups.Add(gp);
                        db.SaveChanges();
                        if (gpmodel.Pages != null && gpmodel.Pages.Count() > 0)
                        {
                            List<AccountGroupObjectMapping> maps = new List<AccountGroupObjectMapping>();
                            foreach (var item in gpmodel.Pages)
                            {
                                if (item.Id > 0)
                                {
                                    AccountGroupObjectMapping objmap = new AccountGroupObjectMapping();
                                    objmap.AccountGroup = gp;
                                    objmap.ObjectType = item.Id;
                                    maps.Add(objmap);
                                }
                            }
                            resp.Code = (int)ApiResponseCode.ok;
                            db.AccountGroupObjectMappings.AddRange(maps);
                            db.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        [HttpPost]
        public JsonResult AddUserAccount([FromBody]UserManageModel User)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (User.UserId > 0)
                {
                    QuickBetCore.DatabaseEntity.User usersobj = db.Users.Where(a => a.Id != User.UserId && a.Email == User.Email).FirstOrDefault();
                    if (usersobj == null)
                    {
                        QuickBetCore.DatabaseEntity.User users = db.Users.Where(a => a.Id == User.UserId).FirstOrDefault();
                        users.Email = User.Email;
                        users.Name = User.Name;
                        if (!string.IsNullOrEmpty(User.Password))
                        {
                            users.Password = User.Password;
                        }
                        users.UserType = (int)UserType.Staff;
                        users.UserStatus = (int)UserStatus.active;
                        AccountGroupUserMapping mapping = db.AccountGroupUserMappings.Where(a => a.UserId == User.UserId).FirstOrDefault();
                        if(mapping!=null)
                        {
                            mapping.UserId = users.Id;
                            mapping.AccountGroupId = User.AccountTypeId;
                        }
                        resp.Code = (int)ApiResponseCode.ok;
                        db.SaveChanges();
                    }
                    else
                    {
                        resp.Msg = Applicationstring.InvalidModel;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(User.Name) && !string.IsNullOrEmpty(User.Email)
                   && !string.IsNullOrEmpty(User.Password) && User.AccountTypeId > 0)
                    {

                        if (!DbOperation.CheckUser(User.Email))
                        {
                            QuickBetCore.DatabaseEntity.User users = new QuickBetCore.DatabaseEntity.User();
                            users.Email = User.Email;
                            users.Name = User.Name;
                            if (!string.IsNullOrEmpty(User.Password))
                            {
                                users.Password = User.Password;
                            }
                            users.CreatedAt =DateTime.UtcNow;
                            users.UpdatedAt = DateTime.UtcNow;
                            users.UserType = (int)UserType.Staff;
                            users.UserStatus = (int)UserStatus.active;
                            db.Users.Add(users);
                            if (db.SaveChanges() > 0)
                            {
                                AccountGroupUserMapping mapping = new AccountGroupUserMapping();
                                mapping.UserId = users.Id;
                                mapping.AccountGroupId = User.AccountTypeId;
                                db.AccountGroupUserMappings.Add(mapping);
                                resp.Code = (int)ApiResponseCode.ok;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            resp.Msg = "this email already exists!";
                        }
                    }
                    else
                    {
                        resp.Msg = Applicationstring.InvalidModel;
                    }
                }

            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
            }
            return Json(new { Data = resp });
        }

        [HttpPost]
        public JsonResult BlockUser([FromBody] BlockUnblockModel unblockModel)
        {
            ApiResponse resp = new ApiResponse();
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
        public JsonResult DeleteUser(int Id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                  resp = DbOperation.DeleteUser(Id);
               
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        //Admin/UserManagement
        [HttpPost]
        public JsonResult UpdatePassword([FromBody] ChanegPassowrdViewModel chanegPassowrd )
        {
            //getting form data
            ApiResponse response = new ApiResponse();
            try
            {
                string newpassword = chanegPassowrd.Password;
                string repeatePassword = chanegPassowrd.ConfirmPassword;
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    if(!string.IsNullOrEmpty(newpassword))
                    {
                        var userobj = db.Users.Where(a => a.Id == chanegPassowrd.UserId).FirstOrDefault();
                        if (newpassword == repeatePassword)
                        {
                            if (userobj != null)
                            {
                                userobj.Password = newpassword;
                                db.SaveChanges();
                                response.Code = (int)ApiResponseCode.ok;
                                response.Msg = "Password changed successfully!";
                            }
                            else
                            {
                                response.Code = (int)ApiResponseCode.fail;
                                response.Msg = "Invalid User!";
                            }
                        }
                        else
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = "New password and confirm password does not macthed!";
                        }
                    }
                    else
                    {
                        response.Msg = "Please enter password";
                    }
                   
                }
            }
            catch (Exception ex)
            {
                response.Code = (int)ApiResponseCode.fail;
                response.Msg = ex.Message;
            }
            return Json(response);
        }

        public ActionResult PasswordRest(string UserId)
        {
            try
            {
               
            }
            catch (Exception ex)
            {

            }
            ViewData["UserId"] = UserId;
            return PartialView("_ResetPassword");
        }
    }
}