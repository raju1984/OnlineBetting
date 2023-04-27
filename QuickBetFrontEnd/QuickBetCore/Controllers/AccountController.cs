using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    public class AccountController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        // GET: Account
        public ActionResult Logout()
        {
            HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, null);
            HttpContext.Session.SetObjectAsJson(SessionVariable.AdminSession, null);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Login(string Email = "")
        {
            LoginViewModel model = new LoginViewModel();
            model.email = Email;
            if (TempData["Registration"] != null)
            {
                ViewData["Registration"] = JsonConvert.DeserializeObject<string>((string)TempData["Registration"]);// TempData["ModelError"].ToString();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel LoginViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LoginViewModel.ErrorMessage = new Error();
                    var userSession = DbOperation.ValidateUser(LoginViewModel);
                    if (userSession.ErrorMessage != null && userSession.ErrorCode == (int)ReturnCode.Success)
                    {
                        if (userSession != null && userSession.Id>0)
                        {
                            HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                            if (userSession.UserType == (int)UserType.Agent || 
                                userSession.UserType == (int)UserType.MobileAgent)
                            {
                                return RedirectToAction("Index", "Dashboard", new { area = "Agent" });
                            }
                            else if (userSession.UserType == (int)UserType.SuperAgent)
                            {
                                return RedirectToAction("Index", "SuperDashboard", new { area = "SuperAgent" });
                            }
                            else if (userSession.UserType == (int)UserType.Nationallottery)
                            {
                                return RedirectToAction("Index", "NationallotteryDashboard", new { area = "Nationallottery" });
                            }
                            else if (userSession.UserType == (int)UserType.Admin)
                            {
                                return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(LoginViewModel.hfparam))
                                {
                                    return RedirectToAction("Index", "Playgame", new { gameid = LoginViewModel.hfparam });
                                }
                                else
                                {
                                    return RedirectToAction("Index", "UserDashboard", new { area = "User" });
                                }
                            }

                        }
                        else
                        {
                            LoginViewModel.ErrorMessage.ErrorMessage = "Something went wrong";
                            LoginViewModel.ErrorMessage.ErrorCode = (int)ReturnCode.Failed;
                        }
                    }
                    else
                    {
                        LoginViewModel.ErrorMessage.ErrorMessage = userSession.ErrorMessage != null? userSession.ErrorMessage: "Something went wrong";
                        LoginViewModel.ErrorMessage.ErrorCode = (int)ReturnCode.Failed;
                    }
                }
                return View(LoginViewModel);
            }
            catch (Exception ex)
            {
                //LoginViewModel.ErrorMessage = new Error() { ErrorCode = "E500", ErrorMessage = Message.E500 };
                return View(LoginViewModel);
            }
        }

        public ActionResult SignUp(string Link = "")
        {
            UserSignUpViewModel model = new UserSignUpViewModel();
            try
            {
                if (Link != null)
                {
                    AES_ALGORITHM _ALGORITHM = new AES_ALGORITHM();
                    model.ReferCode = _ALGORITHM.decryptMessage(Link);
                }
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }
        //user signup
        [HttpPost]
        public ActionResult SignUp(UserSignUpViewModel Registration)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string email = "";
                    string contactNo = "";
                    if (!string.IsNullOrEmpty(Registration.email))
                    {
                         email = Registration.email.Trim().ToLower();
                        bool isAnyUserWithEmail = db.Users.Any(a => a.Email == email.ToLower());
                        if(isAnyUserWithEmail)
                        {
                            Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.StringUserExistEmailMesssage };
                            return View(Registration);
                        }
                            

                    }
                    if (!string.IsNullOrEmpty(Registration.phone))
                    {
                        contactNo = Registration.phone.Trim().Replace(" ", String.Empty).ToLower();
                        bool isAnyUserWithPhone = db.Users.Any(a => a.ContactNo == contactNo.ToLower());
                        if(isAnyUserWithPhone)
                        {
                            Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.StringUserExistPhoneMesssage };
                            return View(Registration);
                        }
                    }
                    if(!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(contactNo))
                    {
                        if (Registration.ReferCode != null)
                        {
                            bool IsAnyRefercodeFound = db.Users.Any(x => x.ReferCode.ToLower().Trim() == Registration.ReferCode.ToLower().Trim());
                            if (IsAnyRefercodeFound == false)
                            {
                                Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidReferCode };
                                return View(Registration);
                            }
                        }
                        Registration.phone = contactNo;
                        Registration.email = email;
                        if (DbOperation.UserRegistration(Registration))
                        {
                            //if(!string.IsNullOrEmpty(Registration.email))
                            //{
                            //    Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Success, ErrorMessage = Applicationstring.RegistrationMail };
                            //    return View(Registration);
                            //}
                            //else
                            //{
                            //    //in case of phone no verificatiopn
                            // TempData["Registration"] = JsonConvert.SerializeObject("Your account has been created successfully you can now log in");
                            //    return RedirectToAction("Login");
                            //}
                            TempData["Registration"] = JsonConvert.SerializeObject("Your account has been created successfully you can now log in");
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.somethingwentwrong };
                        }
                    }
                    else
                    {
                        Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = "Please enter phone or email" };
                    }

                }

            }
            catch (Exception ex)
            {
                Registration.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
                return View(Registration);
            }
            return View(Registration);
        }

        public ActionResult confirm(string Id = "")
        {
            if (Id != null && !string.IsNullOrEmpty(Id))
            {
                ViewBag.email = DbOperation.VerifyUser(Id);
            }
            return View();
        }

        public ActionResult AgentSignUp()
        {
            List<SelectListItem> AgentType = new List<SelectListItem>();
            try
            {
                AgentType.Add(new SelectListItem { Text = "---Select Agent Type--", Value = "", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Mobile Agent", Value = "6", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Shop Agent", Value = "3", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Super Agent", Value = "5", Selected = false });
                ViewBag.AgentType = AgentType;

            }
            catch (Exception ex)
            {
            }
            return View(new AgentSignUpViewModel());
        }
        //user AgentSignUp
        [HttpPost]
        public ActionResult AgentSignUp(AgentSignUpViewModel reg)
        {
            List<SelectListItem> AgentType = new List<SelectListItem>();
            try
            {
                AgentType.Add(new SelectListItem { Text = "---Select Agent Type--", Value = "", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Mobile Agent", Value = "6", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Shop Agent", Value = "3", Selected = false });
                AgentType.Add(new SelectListItem { Text = "Super Agent", Value = "5", Selected = false });
                ViewBag.AgentType = AgentType;

                if (ModelState.IsValid)
                {
                    using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
                    {
                        string email = "";
                        string contactNo = "";
                        if (!string.IsNullOrEmpty(reg.email))
                        {
                            email = reg.email.Trim().ToLower();
                            bool isAnyUserWithEmail = db.Users.Any(a => a.Email == email.ToLower());
                            if (isAnyUserWithEmail)
                            {
                                reg.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.StringUserExistEmailMesssage };
                                return View(reg);
                            }
                        }
                        if (!string.IsNullOrEmpty(reg.phone))
                        {
                            contactNo = reg.phone.Trim().Replace(" ", String.Empty).ToLower();
                            bool isAnyUserWithPhone = db.Users.Any(a => a.ContactNo == contactNo.ToLower());
                            if (isAnyUserWithPhone)
                            {
                                reg.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.StringUserExistPhoneMesssage };
                                return View(reg);
                            }
                        }
                        if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(contactNo))
                        {
                            User users = new User();
                            users.Name = reg.name;
                            users.Email = email;
                            users.ContactNo = contactNo;
                            users.CountryCode = reg.CountryCode;
                            users.Password = reg.password;
                            if (reg.AgentType != 5)
                            {
                                users.AgentCommison = Convert.ToDecimal(ApplicationVariable.defaultagentcommison);
                                users.AgentCashBackOnPayment = Convert.ToDecimal(ApplicationVariable.defaultagentcashback);
                                users.CustomerRetentionPeriod = ApplicationVariable.CustomerRetentionPeriod;
                            }
                            else
                            {
                                users.SuperAgentCashBack = Convert.ToDecimal(ApplicationVariable.defaultSuperAgentCommission);
                            }
                            users.Password = reg.password;
                            users.UserType = reg.AgentType;
                            users.UserStatus = (int)UserStatus.Pending_for_approval;
                            users.CreatedAt = DateTime.UtcNow;
                            users.UpdatedAt = DateTime.UtcNow;
                            dbConn.Users.Add(users);
                            if (dbConn.SaveChanges() > 0)
                            {
                                if(!string.IsNullOrEmpty(users.Email))
                                {
                                    string SincerelyName = "QuickBet Team";
                                    String Subject = "Welcome Mail";
                                    String Body = "Hi " + reg.name + ",<br><br>Thank you for your interest in becoming a QuickbetNG Agent. Our QuickBetNG staff will upon further review of your profile details reach out to you for approval of your account." +
                                    "<br><br>Sincerely,<br>" + SincerelyName;
                                    CommonFunction.SendEmail(reg.email, reg.name, 1, Subject, Body);

                                }
                                UserSession userSession = new UserSession();
                                userSession.email = users.Email;
                                userSession.Id = users.Id;
                                userSession.name = users.Name;
                                userSession.UserType = reg.AgentType;
                                userSession.UserStatus = users.UserStatus;
                                userSession.profilepicture = ApplicatiopnCommonFunction.GetImage(users.ProfilePicture, ImageType.dp.ToString());
                                HttpContext.Session.SetObjectAsJson(SessionVariable.UserSession, userSession);
                                if (reg.AgentType == (int)UserType.SuperAgent)
                                {
                                    return RedirectToAction("Index", "AdminDashboard", new { area = "SuperAgent" });
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Dashboard", new { area = "Agent" });
                                }
                            }
                        }
                        else
                        {
                            reg.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.StringUserExistEmailMesssage };
                        }

                    }
                }
                else
                {
                    reg.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = Applicationstring.InvalidModel };
                }

            }
            catch (Exception ex)
            {
                reg.ErrorMessage = new Error() { ErrorCode = (int)ReturnCode.Failed, ErrorMessage = ex.Message };
                return View(reg);
            }
            return View(reg);
        }


        public ActionResult ForgetPassword()
        {
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string EmailAddress)
        {
            var Password = ApplicatiopnCommonFunction.GetRandomPassword(6);
            if (DbOperation.CheckEmailNew(EmailAddress, Password))
            {
                AES_ALGORITHM aES_ = new AES_ALGORITHM();
                var Email = aES_.encryptMessage(EmailAddress);
                var Pass = aES_.encryptMessage(Password);
                return RedirectToAction("ChangePassword", "Account", new { Email = Email, Pass = Pass });
            }
            else
            {
                ViewData["error"] = Applicationstring.Emailaddressnotrecognised;
                return View();

            }

        }

        public ActionResult ChangePassword(string Email, String Pass)
        {
            AES_ALGORITHM aES_ = new AES_ALGORITHM();
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            try
            {
                if (Email != null && Pass != null)
                {
                    model.Email = aES_.decryptMessage(Email);
                    model.GeneratrdPass = aES_.decryptMessage(Pass);

                    ViewData["error"] = "We have emailed you the instructions for setting your password. If an account exists with the email you entered, you should receive them shortly. " +
                              "(Please check your spam folder if you don’t receive it in your inbox.)";
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.EnterGenetedPass == model.GeneratrdPass)
                    {
                        if (model.ConfirmNewPassword != model.NewPassword)
                        {
                            apiResponse.Code = (int)ApiResponseCode.fail;
                            apiResponse.Msg = "New password and confirm password does not matched!";
                        }
                        else
                        {
                            apiResponse = DbOperation.ForgotPasswordSaveDb(model);
                        }

                    }
                    else
                    {
                        apiResponse.Code = (int)ApiResponseCode.fail;
                        apiResponse.Msg = "Generated password and entered password does not matched!";
                    }
                }
                else
                {
                    apiResponse.Code = (int)ApiResponseCode.fail;
                    apiResponse.Msg = Applicationstring.somethingwentwrong;
                }
            }

            catch (Exception ex)
            {
            }
            if (apiResponse.Code == (int)ApiResponseCode.ok)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewData["error"] = apiResponse.Msg;
                return View(model);
            }
        }
    }
}
