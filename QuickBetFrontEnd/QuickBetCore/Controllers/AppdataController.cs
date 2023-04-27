using Microsoft.AspNetCore.Hosting;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [CustomAuthorizeFilter]
    public class AppdataController : ControllerBase
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
      
        private readonly IWebHostEnvironment webHostEnvironment;
        public AppdataController(IWebHostEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        // GET: api/Appdata/UpdateUserProfile
        [HttpPost]
        public async Task<IActionResult> UpdateUserProfile()
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            resp.metadescription = "Upload file keys: UserId,fullname,displayname,emailaddress";
            try
            {

                int UserId = Convert.ToInt32(HttpContext.Request.Form["UserId"]);
                string fullname = Convert.ToString(HttpContext.Request.Form["fullname"]);
                string displayname = Convert.ToString(HttpContext.Request.Form["displayname"]);
                string emailaddress = Convert.ToString(HttpContext.Request.Form["emailaddress"]);
                string phonenumber = Convert.ToString(HttpContext.Request.Form["phonenumber"]);
                var userobj = db.Users.Where(a => a.Id == UserId).FirstOrDefault();
                if (UserId > 0 && userobj != null)
                {
                    var countuser = db.Users.Where(a => a.Id != UserId && a.Email == emailaddress.Trim()).Count();
                    if (countuser == 0)
                    {
                        if (!string.IsNullOrEmpty(fullname) && !string.IsNullOrEmpty(displayname) && !string.IsNullOrEmpty(emailaddress))
                        {

                            userobj.Name = fullname;
                            userobj.DisplayName = displayname;
                            userobj.Email = emailaddress;
                            userobj.ContactNo = phonenumber;
                            db.SaveChanges();
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = Applicationstring.Success;

                            if (Request.Form.Files.Count > 0)
                            {
                                string uploadsFolder = webHostEnvironment.WebRootPath + "/Content/Images";
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
                            //if (HttpContext.Current.Request.Files.Count > 0)
                            //{
                            //    var file = HttpContext.Current.Request.Files[0];
                            //    if (file.ContentLength > 0)
                            //    {
                            //        var fileName = Guid.NewGuid().ToString() + ".jpg";
                            //        var path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/"), fileName);
                            //        file.SaveAs(path);
                            //        userobj.ProfilePicture = "/Content/Images/" + fileName;
                            //        db.SaveChanges();
                            //    }
                            //}
                            UserDataModel data = new UserDataModel();
                            data.Id = userobj.Id;
                            data.email = userobj.Email;
                            data.name = userobj.Name;
                            data.displayname = userobj.DisplayName;
                            data.profilepicture = userobj.ProfilePicture;
                            data.phone = userobj.ContactNo;
                            resp.data = data;
                        }
                        else
                        {
                            resp.message = "Invalid Model ,";
                            if (string.IsNullOrEmpty(fullname))
                            {
                                resp.message = resp.message + "fullname is empty";
                            }
                            if (string.IsNullOrEmpty(displayname))
                            {
                                resp.message = resp.message + "displayname is empty";
                            }
                            if (string.IsNullOrEmpty(emailaddress))
                            {
                                resp.message = resp.message + "emailaddress is empty";

                            }
                        }
                    }
                    else
                    {
                        resp.message = "email already exist please try with diffrent one!";
                    }
                }
                else
                {

                    resp.message = Applicationstring.InvalidModel;
                    resp.message = resp.message + "UserId is " + UserId;
                }

            }
            catch (Exception ex)
            {
                resp.message = ex.Message;
            }
            return Ok(resp);
        }

        [HttpPost]
        public IActionResult UpdatePassword(UpdatePasswordModelApi updatePasswordModelApi)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        var userobj = db.Users.Where(a => a.Id == updatePasswordModelApi.UserId).FirstOrDefault();
                        if (userobj != null && userobj.Password == updatePasswordModelApi.cuurentpassword.Trim())
                        {
                            userobj.Password = updatePasswordModelApi.newpassword;
                            db.SaveChanges();
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = "Password changed successfully!";
                        }
                        else
                        {
                            resp.message = "Your current password is wrong";
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


        // GET: api/Appdata/GetAdderess?UserId=
        [HttpGet]
        public IActionResult GetAdderess(int UserId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                AddresViewModel model = new AddresViewModel();
                var result = db.Addresses.Where(a => a.UserId == UserId).ToList();
                if (result != null && result.Count() > 0)
                {
                    model.shippingaddress = (from r in result
                                             where r.AddressType == (int)AddressType.Shipping
                                             select new BillingShippingAddressViewModel
                                             {
                                                 Id = r.Id,
                                                 fullname = r.FullName,
                                                 Street = r.Street,
                                                 StreetNO = r.StreetNo,
                                                 City = r.City,
                                                 PhoneNumber = r.Phone,
                                                 ZipCode = r.ZipCode,
                                                 BillingEmail = r.Emailaddress,
                                                 CountryCode = r.CountryCode,
                                             }).ToList();
                    model.billingaddress = (from r in result
                                            where r.AddressType == (int)AddressType.Billing
                                            select new BillingShippingAddressViewModel
                                            {
                                                Id = r.Id,
                                                fullname = r.FullName,
                                                Street = r.Street,
                                                StreetNO = r.StreetNo,
                                                City = r.City,
                                                PhoneNumber = r.Phone,
                                                ZipCode = r.ZipCode,
                                                BillingEmail = r.Emailaddress,
                                                CountryCode = r.CountryCode,
                                            }).ToList();
                }
                resp.data = model;
            }
            catch (Exception ex)
            {
                resp.data = new AddresViewModel();
            }
            return Ok(resp);
        }

        // GET: api/Appdata/GetAdderessById?AdderessId=
        [HttpGet]
        public IActionResult GetAdderessById(int AdderessId)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                BillingShippingAddressViewModel objaddress = new BillingShippingAddressViewModel();
                var address = db.Addresses.Where(a => a.Id == AdderessId).FirstOrDefault();
                if (address != null)
                {
                    objaddress = (from r in db.Addresses
                                  where r.Id == AdderessId
                                  select new BillingShippingAddressViewModel
                                  {
                                      Id = r.Id,
                                      fullname = r.FullName,
                                      Street = r.Street,
                                      StreetNO = r.StreetNo,
                                      City = r.City,
                                      PhoneNumber = r.Phone,
                                      ZipCode = r.ZipCode,
                                      BillingEmail = r.Emailaddress,
                                      CountryCode = r.CountryCode,
                                  }).FirstOrDefault();
                    
                }
                resp.data = objaddress;
            }
            catch (Exception ex)
            {
                resp.data = new AddresViewModel();
            }
            return Ok(resp);
        }

        // GET: api/Appdata/GetCountryList
        [HttpGet]
        public IActionResult GetCountryList()
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.ok;
            try
            {
                var data = (from r in db.Countries
                            select new DropDownKeyValue
                            {
                                Code = r.CountryCode,
                                Name = r.Name
                            }).ToList();
                resp.data = data;
            }
            catch (Exception ex)
            {
                resp.data = new AddresViewModel();
            }
            return Ok(resp);
        }

        //api/Appdata/AddUpdateAddress
        [HttpPost]
        public IActionResult AddUpdateAddress(BillingShippingAddressApiModel billingShippingAddressViewModel)
        {
            ResponseModel resp = new ResponseModel();
            resp.code = (int)ApiResponseCode.fail;
            resp.metadescription = "AddressType: {Shipping = 0,Billing = 1}";

            try
            {
                if (!Enum.IsDefined(typeof(AddressType), billingShippingAddressViewModel.AddressType))
                {
                    resp.message = "please check AddressType parameter";
                    return Ok(resp);
                }
                if (ModelState.IsValid)

                {
                    if (billingShippingAddressViewModel.Id > 0)
                    {
                        Address objaddress = db.Addresses.Where(a => a.Id == billingShippingAddressViewModel.Id).FirstOrDefault();
                        if (objaddress != null)
                        {
                            if (billingShippingAddressViewModel.AddressType == (int)AddressType.Billing)
                            {
                                objaddress.AddressType = (int)AddressType.Billing;
                            }
                            else
                            {
                                objaddress.AddressType = (int)AddressType.Shipping;
                            }
                            objaddress.UserId = billingShippingAddressViewModel.UserId;
                            objaddress.FullName = billingShippingAddressViewModel.fullname;
                            objaddress.Street = billingShippingAddressViewModel.street;
                            objaddress.StreetNo = billingShippingAddressViewModel.streetno;
                            objaddress.City = billingShippingAddressViewModel.towncity;
                            objaddress.Phone = billingShippingAddressViewModel.phone;
                            objaddress.ZipCode = billingShippingAddressViewModel.pincode;
                            objaddress.Emailaddress = billingShippingAddressViewModel.emailaddress;
                            objaddress.CountryCode = billingShippingAddressViewModel.countrycode;
                            db.SaveChanges();
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = "address updated successfully!";
                        }
                        else
                        {
                            resp.message = "Invalid Model state!";
                        }
                    }
                    else
                    {
                        Address objalreadyaddress = db.Addresses.Where(a => a.UserId == billingShippingAddressViewModel.UserId && a.AddressType == billingShippingAddressViewModel.AddressType).FirstOrDefault();
                        if (objalreadyaddress == null)
                        {
                            Address objaddress = new Address();
                            if (billingShippingAddressViewModel.AddressType == (int)AddressType.Billing)
                            {
                                objaddress.AddressType = (int)AddressType.Billing;
                            }
                            else
                            {
                                objaddress.AddressType = (int)AddressType.Shipping;
                            }
                            objaddress.UserId = billingShippingAddressViewModel.UserId;
                            objaddress.FullName = billingShippingAddressViewModel.fullname;
                            objaddress.Street = billingShippingAddressViewModel.street;
                            objaddress.StreetNo = billingShippingAddressViewModel.streetno;
                            objaddress.City = billingShippingAddressViewModel.towncity;
                            objaddress.Phone = billingShippingAddressViewModel.phone;
                            objaddress.ZipCode = billingShippingAddressViewModel.pincode;
                            objaddress.Emailaddress = billingShippingAddressViewModel.emailaddress;
                            objaddress.CountryCode = billingShippingAddressViewModel.countrycode;
                            db.Addresses.Add(objaddress);
                            db.SaveChanges();
                            resp.code = (int)ApiResponseCode.ok;
                            resp.message = "address addedd successfully!";
                        }
                        else
                        {
                            resp.message = "address already addedd , please try to update it!";
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


    }
}
