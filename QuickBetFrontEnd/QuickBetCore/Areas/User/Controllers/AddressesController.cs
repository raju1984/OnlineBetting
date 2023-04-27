using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class AddressesController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AddressesController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: User/Addresses
        public IActionResult Index()
        {
            AddresViewModel model = new AddresViewModel();
            var result = db.Addresses.Where(a => a.UserId == userSession.Id).ToList();
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
            return View(model);
        }

        public IActionResult Billingaddress()
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();
            model.Countrys = (from r in db.Countries
                              select new DropDownKeyValue
                              {
                                  Code = r.CountryCode,
                                  Name = r.Name
                              }).ToList();
            return View(model);
        }
        public IActionResult EditBillingaddress(int AddreesId)
        {
            BillingShippingAddressViewModel objaddress = new BillingShippingAddressViewModel();
            try
            {
                var address = db.Addresses.Where(a => a.Id == AddreesId).FirstOrDefault();
                if (address != null)
                {
                    objaddress.Id = userSession.Id;
                    objaddress.fullname = address.FullName;
                    objaddress.Street = address.Street;
                    objaddress.StreetNO = address.StreetNo;
                    objaddress.City = address.City;
                    objaddress.PhoneNumber = address.Phone;
                    objaddress.ZipCode = address.ZipCode;
                    objaddress.BillingEmail = address.Emailaddress;
                    objaddress.CountryCode = address.CountryCode;
                }
                objaddress.Countrys = (from r in db.Countries
                                       select new DropDownKeyValue
                                       {
                                           Code = r.CountryCode,
                                           Name = r.Name
                                       }).ToList();

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            if (TempData["error"] != null)
            {
                ViewBag.response = JsonConvert.DeserializeObject<string>((string)TempData["error"]);
                //ViewBag.response = TempData["error"].ToString();
            }
            return View("Billingaddress", objaddress);
        }
        [HttpPost]
        public IActionResult PostBillingaddress(BillingShippingAddressViewModel billingShippingAddressViewModel)
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();
            try
            {
                if (!string.IsNullOrEmpty(billingShippingAddressViewModel.CountryCode))
                {
                    if (billingShippingAddressViewModel.Id > 0)
                    {
                        Address objaddress = db.Addresses.Where(a => a.Id == billingShippingAddressViewModel.Id).FirstOrDefault();
                        if (objaddress != null)
                        {
                            objaddress.UserId = userSession.Id;
                            objaddress.FullName = billingShippingAddressViewModel.fullname;
                            objaddress.AddressType = (int)AddressType.Billing;
                            objaddress.Street = billingShippingAddressViewModel.Street;
                            objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                            objaddress.City = billingShippingAddressViewModel.City;
                            objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                            objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                            objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                            objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                            db.SaveChanges();
                            TempData["error"] = JsonConvert.SerializeObject("address updated successfully!");
                            return RedirectToAction("EditBillingaddress", new { AddreesId = billingShippingAddressViewModel.Id });
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
                            return RedirectToAction("EditBillingaddress", new { AddreesId = billingShippingAddressViewModel.Id });
                        }
                    }
                    else
                    {
                        Address objaddress = new Address();
                        objaddress.UserId = userSession.Id;
                        objaddress.FullName = billingShippingAddressViewModel.fullname;
                        objaddress.AddressType = (int)AddressType.Billing;
                        objaddress.Street = billingShippingAddressViewModel.Street;
                        objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                        objaddress.City = billingShippingAddressViewModel.City;
                        objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                        objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                        objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                        objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                        db.Addresses.Add(objaddress);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    ViewData["error"] = "Invalid Model state!";
                    billingShippingAddressViewModel.Countrys = (from r in db.Countries
                                                                select new DropDownKeyValue
                                                                {
                                                                    Code = r.CountryCode,
                                                                    Name = r.Name
                                                                }).ToList();
                    return View("Billingaddress", billingShippingAddressViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
            }
            billingShippingAddressViewModel.Countrys = (from r in db.Countries
                                                        select new DropDownKeyValue
                                                        {
                                                            Code = r.CountryCode,
                                                            Name = r.Name
                                                        }).ToList();
            return View("Billingaddress", billingShippingAddressViewModel);
        }
        public IActionResult Shippingaddress()
        {
            BillingShippingAddressViewModel model = new BillingShippingAddressViewModel();
            try
            {

                model.Countrys = (from r in db.Countries
                                  select new DropDownKeyValue
                                  {
                                      Code = r.CountryCode,
                                      Name = r.Name
                                  }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }
        public IActionResult EditShippingaddress(int AddreesId)
        {
            BillingShippingAddressViewModel objaddress = new BillingShippingAddressViewModel();
            try
            {
                var address = db.Addresses.Where(a => a.Id == AddreesId).FirstOrDefault();
                if (address != null)
                {
                    objaddress.Id = userSession.Id;
                    objaddress.fullname = address.FullName;
                    objaddress.Street = address.Street;
                    objaddress.StreetNO = address.StreetNo;
                    objaddress.City = address.City;
                    objaddress.PhoneNumber = address.Phone;
                    objaddress.ZipCode = address.ZipCode;
                    objaddress.BillingEmail = address.Emailaddress;
                    objaddress.CountryCode = address.CountryCode;
                }
                objaddress.Countrys = (from r in db.Countries
                                       select new DropDownKeyValue
                                       {
                                           Code = r.CountryCode,
                                           Name = r.Name
                                       }).ToList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            if (TempData["error"] != null)
            {
                //ViewBag.response = TempData["error"].ToString();
                ViewBag.response = JsonConvert.DeserializeObject<string>((string)TempData["error"]);
            }
            return View("Shippingaddress", objaddress);
        }
        [HttpPost]
        public IActionResult PostShippingaddress(BillingShippingAddressViewModel billingShippingAddressViewModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(billingShippingAddressViewModel.CountryCode))
                {
                    if (billingShippingAddressViewModel.Id > 0)
                    {
                        Address objaddress = db.Addresses.Where(a => a.Id == billingShippingAddressViewModel.Id).FirstOrDefault();

                        if (objaddress != null)
                        {
                            objaddress.UserId = userSession.Id;
                            objaddress.FullName = billingShippingAddressViewModel.fullname;
                            objaddress.AddressType = (int)AddressType.Shipping;
                            objaddress.Street = billingShippingAddressViewModel.Street;
                            objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                            objaddress.City = billingShippingAddressViewModel.City;
                            objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                            objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                            objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                            objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                            db.SaveChanges();
                            TempData["error"] = JsonConvert.SerializeObject("address updated successfully!");
                            return RedirectToAction("EditShippingaddress", new { AddreesId = billingShippingAddressViewModel.Id });
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
                            return RedirectToAction("EditShippingaddress", new { AddreesId = billingShippingAddressViewModel.Id });
                        }
                    }
                    else
                    {
                        Address objaddress = new Address();
                        objaddress.UserId = userSession.Id;
                        objaddress.FullName = billingShippingAddressViewModel.fullname;
                        objaddress.AddressType = (int)AddressType.Shipping;
                        objaddress.Street = billingShippingAddressViewModel.Street;
                        objaddress.StreetNo = billingShippingAddressViewModel.StreetNO;
                        objaddress.City = billingShippingAddressViewModel.City;
                        objaddress.Phone = billingShippingAddressViewModel.PhoneNumber;
                        objaddress.ZipCode = billingShippingAddressViewModel.ZipCode;
                        objaddress.Emailaddress = billingShippingAddressViewModel.BillingEmail;
                        objaddress.CountryCode = billingShippingAddressViewModel.CountryCode;
                        db.Addresses.Add(objaddress);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    ViewData["error"] = "Invalid Model state!";
                    billingShippingAddressViewModel.Countrys = (from r in db.Countries
                                                                select new DropDownKeyValue
                                                                {
                                                                    Code = r.CountryCode,
                                                                    Name = r.Name
                                                                }).ToList();
                    return View("Shippingaddress", billingShippingAddressViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.Message;
            }
            return View("Shippingaddress", billingShippingAddressViewModel);
        }
    }
}
