using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.APIAuth;
using QuickBetCore.Models.Data;
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
    public class AppAuthController : ControllerBase
    {
        //AppAuth
        [HttpPost]
        public IActionResult Login(LoginViewModelApi log)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                    {
                        User user = new DatabaseEntity.User();
                        if (!string.IsNullOrEmpty(log.email))
                        {
                            user = dbConn.Users.Where(a => a.Email == log.email.Trim()
                       && a.Password == log.password.Trim()
                       && a.UserType == (int)UserType.Users).FirstOrDefault();
                            if (user == null || user.Id == 0)
                            {
                                user = dbConn.Users.Where(a => a.ContactNo == log.email.Trim()
                    && a.Password == log.password.Trim() 
                    && a.UserType == (int)UserType.Users).FirstOrDefault();
                            }
                        }


                        if (user != null)
                        {
                            if (user.UserStatus == (int)UserStatus.active)
                            {
                                if (user.UserType == (int)UserType.Users)
                                {
                                    user.Token = Guid.NewGuid().ToString();
                                    dbConn.SaveChanges();
                                    UserDataModel data = new UserDataModel();
                                    data.Id = user.Id;
                                    data.email = user.Email;
                                    data.name = user.Name;
                                    data.displayname = user.DisplayName;
                                    data.profilepicture = user.ProfilePicture;
                                    data.phone = user.ContactNo;
                                    data.token = user.Token;
                                    resp.data = data;

                                    resp.code = (int)ApiResponseCode.ok;
                                    resp.message = Applicationstring.Success;
                                }
                                else
                                {
                                    resp.message = Applicationstring.Accountblocked;
                                }
                            }
                            else
                            {
                                resp.message = Applicationstring.Accountblocked;
                            }
                        }
                        else
                        {
                            if (user.UserStatus == (int)UserStatus.block || user.UserStatus == (int)UserStatus.deleted)
                            {
                                resp.message = Applicationstring.Youraccountislocked;
                            }
                            else if (user.UserStatus == (int)UserStatus.UserVerificationPending)
                            {
                                resp.message = Applicationstring.Youremailverificationispending;
                            }
                            else
                            {
                                resp.message = Applicationstring.InvalidPasword;
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }


        //api/AppAuth/Registration
        [HttpPost]
        public IActionResult Registration(UserSignUpViewModelApi reg)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                    {

                        if (reg.ReferCode != null)
                        {
                            bool IsAnyRefercodeFound = dbConn.Users.Any(x => x.ReferCode.ToLower().Trim() == reg.ReferCode.ToLower().Trim());
                            if (IsAnyRefercodeFound == false)
                            {
                                resp.message = Applicationstring.InvalidReferCode;
                                return Ok(resp);
                            }
                        }
                        string email = "";
                        string contactNo = "";
                        if (!string.IsNullOrEmpty(reg.email))
                        {
                            email = reg.email.Trim().ToLower();
                            bool isAnyUserWithEmail = dbConn.Users.Any(a => a.Email == email.ToLower());
                            if (isAnyUserWithEmail)
                            {
                                resp.message = Applicationstring.StringUserExistEmailMesssage;
                                return Ok(resp);
                            }


                        }
                        if (!string.IsNullOrEmpty(reg.phone))
                        {
                            contactNo = reg.phone.Trim().Replace(" ", String.Empty).ToLower();
                            bool isAnyUserWithPhone = dbConn.Users.Any(a => a.ContactNo == contactNo.ToLower());
                            if (isAnyUserWithPhone)
                            {
                                resp.message = Applicationstring.StringUserExistPhoneMesssage;
                                return Ok(resp);
                            }
                            if (string.IsNullOrEmpty(reg.countrycode))
                            {
                                resp.message = "Please send country code";
                                return Ok(resp);
                            }

                        }

                        User users = new User();
                        users.Email = reg.email;
                        users.Name = reg.name;
                        users.Password = reg.password;
                        users.Token = Guid.NewGuid().ToString();
                        users.ContactNo = reg.phone;
                        users.CountryCode = reg.countrycode;
                        users.UserType = (int)UserType.Users;
                        users.CreatedAt = DateTime.UtcNow;
                        users.UpdatedAt = DateTime.UtcNow;
                        users.UserStatus = (int)UserStatus.active;
                        dbConn.Users.Add(users);
                        if (dbConn.SaveChanges() > 0)
                        {
                            UserDataModel data = new UserDataModel();
                            data.email = users.Email;
                            data.Id = users.Id;
                            data.name = users.Name;
                            data.token = users.Token;
                            resp.data = data;
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = Applicationstring.Success;
                        }
                        else
                        {
                            resp.message = Applicationstring.somethingwentwrong;
                        }
                    }



                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }


        [HttpPost]
        public IActionResult ForgetPassword(EmailAddressModelApi emailAddressModelApi)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {

                    if (DbOperation.CheckEmail(emailAddressModelApi.email))
                    {
                        resp.code = (int)ApiResponseCode.ok;
                        resp.message = "We have emailed you the instructions for setting your password. If an account exists with the email you entered, you should receive them shortly. " +
                                    "(Please check your spam folder if you don’t receive it in your inbox.)"; ;
                    }
                    else
                    {
                        resp.message = "Please Enter Right Email";
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }

        //api/AppAuth/DeleteAccount
        [HttpPost]
        public IActionResult DeleteAccount(DeleteViewModel deleteView)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    QuickbetDbEntities db = new QuickbetDbEntities();
                    var user = db.Users.Where(a => a.Id == deleteView.UserId && a.Token == deleteView.Token).FirstOrDefault();
                    if(user!=null)
                    {
                        if(user.Password== deleteView.password)
                        {
                            user.Email = user.Email + "_" + DateTime.UtcNow.Ticks;
                            user.ContactNo = user.ContactNo + "_" + DateTime.UtcNow.Ticks;
                            db.SaveChanges();
                            resp.code = (int)ApiResponseCode.ok;
                        }
                        else
                        {
                            resp.message = "Invalid password";
                        }
                    }
                }
                else
                {
                    resp.message = "Invalid model";
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }
    }
}
