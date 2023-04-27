﻿using Microsoft.AspNetCore.Hosting;
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


namespace Quickbet.Areas.SuperAgent.Controllers
{
    [TypeFilter(typeof(CheckSuperAgentSessionExpire))]
    public class SuperAgentProfileSettingsController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SuperAgentProfileSettingsController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: SuperAgentProfileSettings/ProfileSettings
        public ActionResult Index()
        {
            try
            {
                if (TempData["ModelError"] != null)
                {
                    //ApiResponse response = new ApiResponse();
                    //response = (ApiResponse)TempData["ModelError"];
                    //ViewBag.response = response;
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ModelError"]);
                    //TempData["ModelError"].ToString();
                }

            }
            catch (Exception ex)
            {
            }
            return View();
        }

        public ActionResult PasswordChange()
        {
            if (TempData["ModelError"] != null)
            {
                //ApiResponse response = new ApiResponse();
                //response = (ApiResponse)TempData["ModelError"];
                //ViewBag.response = response;
                ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ModelError"]);
                //TempData["ModelError"].ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(IFormCollection collection)
        {
            //getting form data
            ApiResponse response = new ApiResponse();
            try
            {
                string currentpassword = collection["cuurentpassword"];
                string newpassword = collection["newpassword"];
                string repeatePassword = collection["repeatpassword"];

                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (newpassword == repeatePassword)
                    {
                        if (userobj != null && userobj.Password == currentpassword.Trim())
                        {
                            userobj.Password = newpassword;
                            db.SaveChanges();
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = "Password changed successfully!";
                            TempData["ModelError"] = JsonConvert.SerializeObject(response);
                            return RedirectToAction("PasswordChange");
                        }
                        else
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = "Your current password is wrong";
                            TempData["ModelError"] = JsonConvert.SerializeObject(response);
                            return RedirectToAction("PasswordChange");
                        }
                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "New password and confirm password does not macthed!";
                        TempData["ModelError"] = JsonConvert.SerializeObject(response);
                        return RedirectToAction("PasswordChange");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Code = (int)ApiResponseCode.fail;
                response.Msg = ex.Message;
                TempData["ModelError"] = JsonConvert.SerializeObject(response);
                return RedirectToAction("PasswordChange");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(IFormCollection collection)
        {
            ApiResponse response = new ApiResponse();
            //getting form data
            string fullname = collection["fullname"];
            string displayname = collection["displayname"];
            string emailaddress = collection["emailaddress"];
            string phonenumber = collection["PhoneNumber"];
            string CountryCode = collection["CountryCode"];
            phonenumber = phonenumber.Replace(" ", String.Empty);
            //phonenumber = CountryCode + phonenumber;

            try
            {
                if (!string.IsNullOrEmpty(emailaddress) && !string.IsNullOrEmpty(phonenumber) && !string.IsNullOrEmpty(CountryCode))
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
                                userobj.Name = fullname;
                                userobj.DisplayName = displayname;
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
                                response.Code = (int)ApiResponseCode.ok;
                                response.Msg = "profile updated successfully";
                                TempData["ModelError"] = JsonConvert.SerializeObject(response);
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                response.Code = (int)ApiResponseCode.fail;
                                response.Msg = "user not found!";
                                TempData["ModelError"] = JsonConvert.SerializeObject(response);
                                return RedirectToAction("Index");
                            }
                        }


                    }
                    else
                    {
                        response.Code = (int)ApiResponseCode.fail;
                        response.Msg = "email already exist please try with diffrent one!";
                        TempData["ModelError"] = JsonConvert.SerializeObject(response);
                        return View("Index");
                    }

                }
                else
                {
                    response.Code = (int)ApiResponseCode.fail;
                    response.Msg = "Pleae fill all information!";
                    TempData["ModelError"] = JsonConvert.SerializeObject(response);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                response.Code = (int)ApiResponseCode.fail;
                response.Msg = ex.Message;
                TempData["ModelError"] = JsonConvert.SerializeObject(response);
                return RedirectToAction("Index");
            }
        }
    }
}