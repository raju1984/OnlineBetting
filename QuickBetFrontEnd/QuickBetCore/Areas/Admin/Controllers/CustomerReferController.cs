using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class CustomerReferController : Controller
    {
        // GET: Admin/CustomerRefer
        public ActionResult Index()
        {
            ManageReferalModal model = new ManageReferalModal();
            try
            {
                if (TempData["ActionResponse"] != null)
                {
                    ViewBag.response = JsonConvert.DeserializeObject<ApiResponse>((string)TempData["ActionResponse"]); //(ApiResponse)TempData["ActionResponse"];
                }
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var cheeck = db.UserReferDetails.Where(x => x.Id == 1).FirstOrDefault();

                    if (cheeck != null)
                    {
                        model.ReferalDays = cheeck.ReferPeriods;
                        model.ReferalPercentage = cheeck.ReferPercentage;
                    }

                    model.Refer = (from ee in db.ReferUserMappings
                                   join dd in db.Users
                                on ee.ReferedUserId equals dd.Id
                                   join q in db.Users
                                  on ee.ReferedFromId equals q.Id
                                   select new ReferAndEarLists
                                   {
                                       ReferedUsername = dd.Name,
                                       Email = dd.Email,
                                       Commission = ee.Percentage,
                                       ReferedDate = ee.ReferedDate,
                                       ReferedFromName = q.Name
                                   }).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReferalComission(ManageReferalModal modal)
        {
            ApiResponse response = new ApiResponse();
            response.Code = (int)ApiResponseCode.fail;
            try
            {
                if (ModelState.IsValid)
                {
                    using (QuickbetDbEntities db = new QuickbetDbEntities())
                    {
                        var refer = db.UserReferDetails.Where(x => x.Id == 1).FirstOrDefault();
                        if (refer != null)
                        {
                            refer.ReferPercentage = modal.ReferalPercentage;
                            refer.ReferPeriods = modal.ReferalDays;
                            refer.UpdatedDate = DateTime.UtcNow;
                            if (db.SaveChanges() > 0)
                            {
                                response.Code = (int)ApiResponseCode.ok;
                                response.Msg = "Successfully updated!!";
                            }
                        }
                        else
                        {
                            UserReferDetail detail = new UserReferDetail
                            {
                                UpdatedDate = DateTime.UtcNow,
                                ReferPercentage = modal.ReferalPercentage,
                                ReferPeriods = modal.ReferalDays
                            };
                            db.UserReferDetails.Add(detail);
                            if (db.SaveChanges() > 0)
                            {
                                response.Code = (int)ApiResponseCode.ok;
                                response.Msg = "Successfully updated!!";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            TempData["ActionResponse"] = JsonConvert.SerializeObject(response);
            return RedirectToAction("Index", "CustomerRefer");
        }
    }
}