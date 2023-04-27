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

namespace Quickbet.Areas.Agent.Controllers
{
    [TypeFilter(typeof(CheckAgentSessionExpire))]
    public class AgentProfileSettingController : Controller
    {
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AgentProfileSettingController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: AgentProfileSetting/ProfileSetting
        public ActionResult Index()
        {
            if (TempData["ModelError"] != null)
            {
                ViewData["error"] = JsonConvert.DeserializeObject<string>((string)TempData["ModelError"]);// TempData["ModelError"].ToString();
            }
            return View();
        }
        // Post:UpdateUserProfile 
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile(IFormCollection collection)
        {
            //getting form data
            string fullname = collection["fullname"];
            string displayname = collection["displayname"];
            string emailaddress = collection["emailaddress"];
            string phonenumber = collection["PhoneNumber"];
            string CountryCode = collection["CountryCode"];
            phonenumber = phonenumber.Replace(" ", String.Empty);
            phonenumber = CountryCode + phonenumber;
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
                                emailaddress = emailaddress.Trim().ToLower();

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
                                TempData["ModelError"] = JsonConvert.SerializeObject("Profile updated successfully");
                            }
                            else
                            {
                                TempData["ModelError"] = JsonConvert.SerializeObject("User not found!");
                            }
                        }
                    }
                    else
                    {
                        TempData["ModelError"] = JsonConvert.SerializeObject("Email already exist, Please try different email address!");
                    }
                }
                else
                {
                    TempData["ModelError"] = JsonConvert.SerializeObject("Please fill all information!");
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
                ViewData["error"] = JsonConvert.DeserializeObject<string>((string)TempData["ModelError"]);// TempData["ModelError"].ToString();
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
                string repeatpassword = collection["repeatpassword"];

                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var userobj = db.Users.Where(a => a.Id == userSession.Id).FirstOrDefault();
                    if (repeatpassword == newpassword)
                    {
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
                    else
                    {
                        TempData["ModelError"] = JsonConvert.SerializeObject("New password and confirm password does not matched!");
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["ModelError"] = JsonConvert.SerializeObject(ex.Message);
            }
            return RedirectToAction("Setting");
        }

        #region Agent Shop Adress

        public ActionResult ShopAgentAddress()
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var address = db.Addresses.Where(a => a.UserId == userSession.Id).FirstOrDefault();
                    if (address != null)
                    {
                        model.Id = userSession.Id;
                        model.fullname = address.FullName;
                        model.Street = address.Street;
                        model.StreetNO = address.StreetNo;
                        model.City = address.City;
                        model.PhoneNumber = address.Phone;
                        model.ZipCode = address.ZipCode;
                        model.BillingEmail = address.Emailaddress;
                        model.CountryCode = address.CountryCode;
                    }
                    model.Countrys = (from r in db.Countries
                                      select new DropDownKeyValue
                                      {
                                          Code = r.CountryCode,
                                          Name = r.Name
                                      }).ToList();
                }
                if (TempData["error"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<string>((string)TempData["error"]);// TempData["error"].ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult PostBillingaddress(BillingShippingAddressViewModel billingShippingAddressViewModel)
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (billingShippingAddressViewModel.Id > 0)
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {

                            Address objaddress = db.Addresses.Where(a => a.UserId == billingShippingAddressViewModel.Id).FirstOrDefault();
                            if (objaddress != null)
                            {

                                objaddress.UserId = userSession.Id;
                                objaddress.FullName = billingShippingAddressViewModel.fullname;
                                objaddress.AddressType = (int)AddressType.shopAddress;
                                objaddress.Street = billingShippingAddressViewModel.Street;
                                objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                                objaddress.City = billingShippingAddressViewModel.City;
                                objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                                objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                                objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                                objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                                db.SaveChanges();
                                TempData["error"] = JsonConvert.SerializeObject("address updated successfully!");
                                return RedirectToAction("ShopAgentAddress", new { AddreesId = billingShippingAddressViewModel.Id });
                            }

                            else
                            {
                                billingShippingAddressViewModel.Countrys = (from r in db.Countries
                                                                            select new DropDownKeyValue
                                                                            {
                                                                                Code = r.CountryCode,
                                                                                Name = r.Name
                                                                            }).ToList();
                                ViewData["error"] = "Invalid Model state!";
                                return RedirectToAction("ShopAgentAddress", new { AddreesId = billingShippingAddressViewModel.Id });
                            }
                        }
                    }
                    else
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            Address objaddress = new Address();
                            objaddress.UserId = userSession.Id;
                            objaddress.FullName = billingShippingAddressViewModel.fullname;
                            objaddress.AddressType = (int)AddressType.shopAddress;
                            objaddress.Street = billingShippingAddressViewModel.Street;
                            objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                            objaddress.City = billingShippingAddressViewModel.City;
                            objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                            objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                            objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                            objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                            db.Addresses.Add(objaddress);
                            db.SaveChanges();
                            TempData["error"] = JsonConvert.SerializeObject("address updated successfully!");
                            return RedirectToAction("ShopAgentAddress");
                        }
                    }

                }
                else
                {
                    ViewData["error"] = "Invalid Model state!";
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        billingShippingAddressViewModel.Countrys = (from r in db.Countries
                                                                    select new DropDownKeyValue
                                                                    {
                                                                        Code = r.CountryCode,
                                                                        Name = r.Name
                                                                    }).ToList();
                    }
                    return View("ShopAgentAddress", billingShippingAddressViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
            }
            return RedirectToAction("ShopAgentAddress");
        }

        #endregion
    }
}