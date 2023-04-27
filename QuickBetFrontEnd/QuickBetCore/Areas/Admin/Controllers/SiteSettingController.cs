using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Admin.Controllers
{
    [TypeFilter(typeof(CheckAdminSessionExpire))]
    public class SiteSettingController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        private UserSession userSession;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SiteSettingController(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            webHostEnvironment = hostEnvironment;
            userSession = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<UserSession>(SessionVariable.UserSession);
        }
        // GET: Admin/SiteSetting
        public ActionResult ManageCarousel(int carouselId = 0)
        {
            CarouselViewModel result = new CarouselViewModel();
            try
            {
                if (carouselId > 0)
                {
                    var obj = db.Picturecarousels.Where(a => a.Id == carouselId).FirstOrDefault();
                    if (obj != null)
                    {
                        result.Title = obj.Titile;
                        result.Description = obj.Description;
                        result.ButtonName = obj.ButtonName;
                        result.ButtonLink = obj.ButtonLink;
                    }
                }
                result.pictures = (from r in db.Picturecarousels
                                   select new CarouselViewModel
                                   {
                                       PictureUrl = r.PictureUrl,
                                       Id = r.Id,
                                       Status = r.Status,
                                       Title = r.Titile,
                                       Description = r.Description,
                                       ButtonName = r.ButtonName,
                                       ButtonLink = r.ButtonLink
                                   }).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormCollection collection)
        {
            ApiResponse ErrorMessage = new ApiResponse();
            try
            {
                string txtHeading = collection["txtHeading"];
                string txtDescription = collection["txtDescription"];
                string ButtonName = collection["ButtonName"];
                string ButtonLink = collection["ButtonLink"];
                string rowId = collection["hfrowId"];
                string mystring = ButtonLink;
                string last4 = mystring.Substring(0, 4);
                if (last4 != "http")
                {
                    ButtonLink = "http://" + ButtonLink;
                }
                if (!string.IsNullOrEmpty(txtHeading) && !string.IsNullOrEmpty(txtDescription) && !string.IsNullOrEmpty(ButtonName))
                {
                    if (!string.IsNullOrEmpty(rowId))
                    {
                        int id = Convert.ToInt32(rowId);
                        var updateobj = db.Picturecarousels.Where(a => a.Id == id).FirstOrDefault();
                        if (updateobj != null)
                        {
                            updateobj.Titile = txtHeading;
                            updateobj.Description = txtDescription;
                            updateobj.ButtonName = ButtonName;
                            updateobj.ButtonLink = ButtonLink;

                            if (Request.Form.Files.Count > 0)
                            {
                                var file = Request.Form.Files[0];
                                using (var image = Image.FromStream(file.OpenReadStream()))
                                {
                                    if (image.Width >= 1900)
                                    {
                                        string uploadsFolder = webHostEnvironment.WebRootPath + "/assets/images/";
                                        //getting file name and combine with path and save it
                                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                        //using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                                        //{
                                        //    await file.CopyToAsync(fileStream);
                                        //}
                                        //save folder path 
                                        image.Save(uploadsFolder + filename);
                                        updateobj.IsDefault = 0;
                                        updateobj.PictureUrl = "/assets/images/" + filename;
                                    }
                                }

                            }
                            //if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                            //{
                            //    List<Picturecarousel> list = new List<Picturecarousel>();
                            //    HttpPostedFileBase file = Request.Files[0];
                            //    using (System.Drawing.Image myImage =
                            //    System.Drawing.Image.FromStream(file.InputStream))
                            //    {
                            //        //(myImage.Height == 140 && myImage.Width == 140);
                            //        if (myImage.Width >= 1900)
                            //        {
                            //            updateobj.Status = (int)TableRowstatus.active;
                            //            var extension = Path.GetExtension(file.FileName);
                            //            string fname = Guid.NewGuid().ToString(); //file.FileName;
                            //            string savepath = Path.Combine(Server.MapPath("~/assets/images/"), fname + extension);
                            //            file.SaveAs(savepath);
                            //            updateobj.IsDefault = 0;
                            //            updateobj.PictureUrl = "/assets/images/" + fname + extension;
                            //        }
                            //    }
                            //}
                        }
                        db.SaveChanges();
                    }
                    else if (Request.Form.Files.Count > 0)
                    {
                        List<Picturecarousel> list = new List<Picturecarousel>();
                        for (int i = 0; i < Request.Form.Files.Count; i++)
                        {
                            var file = Request.Form.Files[i];
                            using (var image = Image.FromStream(file.OpenReadStream()))
                            {
                                // use image.Width and image.Height
                                if (image.Width >= 1900)
                                {
                                    Picturecarousel mulobj = new Picturecarousel();
                                    mulobj.Status = (int)TableRowstatus.active;
                                    mulobj.ButtonName = ButtonName;
                                    mulobj.ButtonLink = ButtonLink;
                                    mulobj.Titile = txtHeading;
                                    mulobj.Description = txtDescription;
                                    string uploadsFolder = webHostEnvironment.WebRootPath + "/assets/images/";
                                    //getting file name and combine with path and save it
                                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                    //using (var fileStream = new FileStream(Path.Combine(uploadsFolder, filename), FileMode.Create))
                                    //{
                                    //    await file.CopyToAsync(fileStream);
                                    //}
                                    image.Save(uploadsFolder + filename);
                                    mulobj.IsDefault = 0;
                                    mulobj.PictureUrl = "/assets/images/" + filename;
                                    list.Add(mulobj);
                                }

                            }
                        }
                        if (list != null && list.Count() > 0)
                        {
                            db.Picturecarousels.AddRange(list);
                            db.SaveChanges();
                        }
                        ErrorMessage = new ApiResponse { Code = (int)ReturnCode.Success, Msg = "Success" };
                    }
                    else
                    {
                        ErrorMessage = new ApiResponse { Code = (int)ReturnCode.Failed, Msg = "Please choose image!" };
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = new ApiResponse() { Code = (int)ReturnCode.Failed, Msg = ex.Message };
            }
            return RedirectToAction("ManageCarousel");
        }

        [HttpPost]
        public async Task<IActionResult> ImageLogo(IFormCollection collection)
        {
            ApiResponse ErrorMessage = new ApiResponse();
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    using (var image = Image.FromStream(file.OpenReadStream()))
                    {
                        string uploadsFolder = webHostEnvironment.WebRootPath + "/assets/images/logo.png";
                        if (System.IO.File.Exists(uploadsFolder))
                        {
                            System.IO.File.Delete(uploadsFolder);
                        }
                        //save folder path 
                        image.Save(uploadsFolder);
                    }

                }
                else
                {
                    ErrorMessage = new ApiResponse { Code = (int)ReturnCode.Failed, Msg = "Please choose image!" };
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = new ApiResponse() { Code = (int)ReturnCode.Failed, Msg = ex.Message };
            }
            return RedirectToAction("ManageCarousel");
        }

        [HttpPost]
        public JsonResult DeleteMedia(int MulMediaId)
        {
            ApiResponse resp = new ApiResponse();
            try
            {
                var mulmediaobj = db.Picturecarousels.Where(a => a.Id == MulMediaId).FirstOrDefault();
                if (mulmediaobj != null)
                {
                    db.Picturecarousels.Remove(mulmediaobj);
                    db.SaveChanges();
                }
                resp.Code = (int)ReturnCode.Success;
                resp.Msg = "Deleted successfully.";
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Code = (int)ReturnCode.Failed;
                resp.Msg = "Failed to delete.";
                return Json(resp);
            }
        }


        // GET: Admin/ManageContactUs
        public ActionResult ManageContactUs()
        {
            ContactDetail result = new ContactDetail();
            try
            {
                var ContactDetailsobj = db.ContactDetails.FirstOrDefault();
                if (ContactDetailsobj != null)
                {
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        result.ContactEmail = ContactDetailsobj.ContactEmail;
                        result.ContactPhone1 = ContactDetailsobj.ContactPhone1;
                        result.ContactPhone2 = ContactDetailsobj.ContactPhone2;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View("UpdateContact", result);
        }

        // Post:UpdateContact 
        [HttpPost]
        public ActionResult UpdateContact(IFormCollection collection)
        {
            //getting form data
            string email = collection["email"];
            string phone1 = collection["phone1"];
            string phone2 = collection["phone2"];
            try
            {

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(phone1) && !string.IsNullOrEmpty(phone2))
                {
                    QuickbetDbEntities db = new QuickbetDbEntities();
                    var ContactDetailsobj = db.ContactDetails.FirstOrDefault();
                    if (ContactDetailsobj != null)
                    {
                        ContactDetailsobj.ContactEmail = email;
                        ContactDetailsobj.ContactPhone1 = phone1;
                        ContactDetailsobj.ContactPhone2 = phone2;
                        db.SaveChanges();
                        TempData["ModelError"] = JsonConvert.SerializeObject("Updated successfully");
                        return RedirectToAction("ManageContactUs");
                    }
                    else
                    {
                        ContactDetailsobj = new ContactDetail
                        {
                            ContactEmail = email,
                            ContactPhone1 = phone1,
                            ContactPhone2 = phone2,
                        };
                        db.ContactDetails.Add(ContactDetailsobj);
                        db.SaveChanges();
                        TempData["ModelError"] = JsonConvert.SerializeObject("Updated successfully");
                        return RedirectToAction("ManageContactUs");
                    }
                }
                else
                {
                    TempData["ModelError"] = JsonConvert.SerializeObject("Invalid Model");
                    return RedirectToAction("ManageContactUs");
                }
            }
            catch (Exception ex)
            {
                TempData["ModelError"] = JsonConvert.SerializeObject(ex.Message);
                return RedirectToAction("ManageContactUs");
            }
        }
    }
}