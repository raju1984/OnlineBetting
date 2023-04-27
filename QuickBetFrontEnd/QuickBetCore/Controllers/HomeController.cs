using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            this.env = env;
        }

        QuickbetDbEntities db = new QuickbetDbEntities();
        //Home/DownloadFile
        public IActionResult QUICKBETV003()
        {
            //string filePath = @"C:\inetpub\wwwroot\QuickbetCore\QuickBetCore\wwwroot\apk\QUICKBETV003.apk";
            return File(env
             .WebRootFileProvider
             .GetFileInfo("/apk/QUICKBETV003.apk")
             .CreateReadStream(),
         "application/vnd.android.package-archive",
         enableRangeProcessing: true);

        }
        // GET: Home
        public ActionResult Index()
        {
            List<CarouselViewModel> result = new List<CarouselViewModel>();
            try
            {
                result = (from r in db.Picturecarousels
                          where r.Status == (int)TableRowstatus.active
                          select new CarouselViewModel
                          {
                              Id = r.Id,
                              PictureUrl = r.PictureUrl,
                              Status = r.Status,
                              Title = r.Titile,
                              Description = r.Description,
                              ButtonName = r.ButtonName,
                              ButtonLink = r.ButtonLink,
                          }).ToList();

                ViewBag.LogoLink = db.Bannermanages.Where(x => x.Id == 1).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return View(result);
        }

        public ActionResult GetGamelist()
        {
            var model = DbOperation.Getgamelist();
            return PartialView("_Gamelist", model);
        }

        public ActionResult Getscratchlist()
        {
            List<GameViewModel> model = new List<GameViewModel>();
            try
            {
                model = DbOperation.Getgamelist(4);
                return PartialView("_Homescratch", model);
            }
            catch (Exception ex)
            {
                model = new List<GameViewModel>();
            }
            return PartialView("_Homescratch", model);
        }

        public ActionResult Getmostwon()
        {
            List<GameViewModel> model = new List<GameViewModel>();
            try
            {
                model = DbOperation.Getgamelist(4, 4);
                return PartialView("_Homemostwon", model);
            }
            catch (Exception ex)
            {
                model = new List<GameViewModel>();
            }
            return PartialView("_Homemostwon", model);
        }

        public ActionResult PlayGame()
        {
            return View();
        }
        public ActionResult Contact()
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
            return View(result);
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Agent()
        {
            return View();
        }

        public ActionResult faq()
        {
            return View();
        }

        public ActionResult termsandservice()
        {
            return View();
        }

        public ActionResult privacypolicy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string EmailAddress, string name, string message)
        {
            if (ApplicatiopnCommonFunction.SendEmail(EmailAddress, name, message, 2))
            {
                ViewData["error"] = "We have taken you request, will contact you soon.)"; ;
                return View();
            }
            else
            {
                ViewData["error"] = "Something went wrong please try again later!";
                return View();

            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
