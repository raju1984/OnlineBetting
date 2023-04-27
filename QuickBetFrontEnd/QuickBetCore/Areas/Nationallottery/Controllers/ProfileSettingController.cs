using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickBetCore.Areas.Agent.Data;
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


namespace Quickbet.Areas.Nationallottery.Controllers
{
    [TypeFilter(typeof(CheckNationallotterySessionExpire))]
    public class ProfileSettingController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProfileSettingController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Nationallottery/ProfileSetting
        public ActionResult Index()
        {
            if (TempData["ModelError"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<string>((string)TempData["ModelError"]);
                //ViewData["error"] = TempData["ModelError"].ToString();
            }
            return View();
        }
        // Post:UpdateUserProfile 
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(IFormCollection collection)
        {
            //getting form data
            string fullname = collection["fullname"];
            //string displayname = collection["displayname"];
            string emailaddress = collection["emailaddress"];
            string phonenumber = collection["PhoneNumber"];
            string CountryCode = collection["CountryCode"];
            phonenumber = phonenumber.Replace(" ", String.Empty);
            //phonenumber = CountryCode + phonenumber;
            try
            {
                if(!string.IsNullOrEmpty(emailaddress) && !string.IsNullOrEmpty(phonenumber) && !string.IsNullOrEmpty(CountryCode))
                {
                    QuickbetDbEntities db1 = new QuickbetDbEntities();
                    var countuser = db1.Users.Where(a => a.Id != userSession.Id && a.Email == emailaddress.Trim()).Count();
                    if (countuser == 0)
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                            if (userobj != null)
                            {
                                if (Request.Form.Files.Count > 0)
                                {
                                    string uploadsFolder = webHostEnvironment.WebRootPath+ "/Content/Images";
                                    //getting file name and combine with path and save it
                                    var file = Request.Form.Files[0];
                                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                    }
                                    //save folder path 
                                    userobj.ProfilePicture = "/Content/Images/" + filename;
                                }
                                emailaddress = emailaddress.Trim().ToLower();

                                userobj.Name = fullname;
                                var isanyuser = db.Users.Any(a => a.Email.ToLower() == emailaddress && a.Id != userSession.Id);
                                if (!isanyuser)
                                {
                                    userobj.Email = emailaddress;
                                }
                                userobj.CountryCode = CountryCode;
                                userobj.ContactNo = phonenumber;
                                db.SaveChanges();
                                userSession.email = userobj.Email;
                                userSession.name = userobj.Name;
                                userSession.phone = userobj.ContactNo;
                                userSession.displayname = userobj.DisplayName;
                                userSession.profilepicture = userobj.ProfilePicture;
                                TempData["ModelError"] = JsonConvert.SerializeObject("profile updated successfully");
                            }
                            else
                            {
                                TempData["ModelError"] = JsonConvert.SerializeObject("user not found!");
                            }
                        }
                    }
                    else
                    {
                        TempData["error"] = JsonConvert.SerializeObject("email already exist please try with diffrent one!");
                    }
                }
                else
                {
                    TempData["error"] = JsonConvert.SerializeObject("Pleae fill all information!");
                }

            }
            catch (Exception ex)
            {
                TempData["ModelError"] = JsonConvert.SerializeObject(ex.Message);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Setting()
        {
            if (TempData["ModelError"] != null)
            {
                //ViewData["error"] = TempData["ModelError"].ToString();
                ViewData["error"] = JsonConvert.DeserializeObject<string>((string)TempData["ModelError"]);
            }
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePassword(IFormCollection collection)
        {
            //getting form data

            try
            {
                string currentpassword = collection["cuurentpassword"];
                string newpassword = collection["newpassword"];
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (userobj != null && userobj.Password == currentpassword.Trim())
                    {
                        userobj.Password = newpassword;
                        db.SaveChanges();
                        TempData["ModelError"] = JsonConvert.SerializeObject("Password changed successfully!");
                    }
                    else
                    {
                        TempData["ModelError"] = JsonConvert.SerializeObject("Your current password is wrong");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ModelError"] = JsonConvert.SerializeObject(ex.Message);
            }
            return RedirectToAction("Setting");
        }
    }
}