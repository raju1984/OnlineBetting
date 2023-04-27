using Microsoft.AspNetCore.Mvc;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Filters;
using QuickBetCore.Models;
using QuickBetCore.Models.Data;
using QuickBetCore.Models.GenegameStudio;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Admin.Controllers
{
    public class GameManage
    {
        public int isenable { get; set; }
        public int Id { get; set; }
    }
    [TypeFilter(typeof(CheckAdminSessionExpire))]
    public class ManageGameController : Controller
    {
        QuickbetDbEntities db = new QuickbetDbEntities();
        // GET: Admin/ManageGame
        public ActionResult Index()
        {
            List<GameViewModel> model = new List<GameViewModel>();
            try
            {
                 model = DbOperation.Getgamelist();
            }
            catch (Exception ex)
            {

            }
            return View(model);
        }

        [HttpPost]
        public JsonResult Enabledisablegame([FromBody] GameManage gameManage)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                if (gameManage.Id > 0)
                {
                    var obj = db.Gamelists.Where(a => a.Id == gameManage.Id).FirstOrDefault();
                    obj.IsEnable = gameManage.isenable == 1 ? true : false;
                    db.SaveChanges();
                }
                else
                {
                    resp.Msg = "Invalid Entry";
                }
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }

        [HttpPost]
        public JsonResult SyncGame()
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            try
            {
                var listofgame = new GenegameStudioHdb().GetAllGamelist();
                if (listofgame != null && listofgame.Count() > 0)
                {
                    var gameobj = (from r in listofgame
                                   select new Gamelist
                                   {
                                       GameId = r.gameId.ToString(),
                                       GameName = r.name,
                                       Gameimg = r.coverImage,
                                       JackpotAmount = r.MaxJackpotAmount,
                                       TicketCost = r.TicketCost,
                                       Caption = r.caption,
                                       IsEnable = true,
                                       CateId = r.CateId,
                                       CateName = r.CateName
                                   }).ToList();
                    if (gameobj != null && gameobj.Count() > 0)
                    {
                        var dbgamelist = db.Gamelists;
                        if (dbgamelist != null && dbgamelist.Count() > 0)
                        {
                            foreach (var item in gameobj)
                            {
                                var updategameobj = dbgamelist.Where(a => a.GameId == item.GameId).FirstOrDefault();
                                if (updategameobj != null)
                                {
                                    updategameobj.GameName = item.GameName;
                                    updategameobj.Gameimg = item.Gameimg;
                                    updategameobj.Caption = item.Caption;
                                    updategameobj.JackpotAmount = item.JackpotAmount;
                                    updategameobj.TicketCost = item.TicketCost;
                                    updategameobj.CateId = item.CateId;
                                    updategameobj.CateName = item.CateName;
                                    updategameobj.DeletedFromGeneGame = false;
                                    db.SaveChanges();
                                }
                                else
                                {

                                    db.Gamelists.Add(new Gamelist
                                    {
                                        GameId = item.GameId,
                                        GameName = item.GameName,
                                        Gameimg = item.Gameimg,
                                        Caption = item.Caption,
                                        JackpotAmount = item.JackpotAmount,
                                        TicketCost = item.TicketCost,
                                        IsEnable = true,
                                        DeletedFromGeneGame=false,
                                        CateId = item.CateId,
                                        CateName = item.CateName
                                    });
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            db.Gamelists.AddRange(gameobj);
                            db.SaveChanges();
                        }
                        var gameIds = gameobj.Select(a => a.GameId).ToList();
                        var deletedGameList = dbgamelist.Where(r => !gameIds.Contains(r.GameId)).ToList();
                        if (deletedGameList != null && deletedGameList.Count() > 0)
                        {
                            foreach(var item in deletedGameList)
                            {
                                item.DeletedFromGeneGame = true;
                                db.SaveChanges();
                            }
                        }
                    }

                }
                return Json(resp);
            }
            catch (Exception ex)
            {
                resp.Msg = ex.Message;
                return Json(resp);
            }
        }
    }
}