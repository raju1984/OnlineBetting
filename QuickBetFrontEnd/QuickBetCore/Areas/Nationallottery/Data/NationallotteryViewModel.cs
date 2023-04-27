using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Nationallottery.Data
{
    public class NationallotteryDahsboardViewModel
    {
        public decimal CurrentMontheCommison { get; set; }
        public decimal CurrentMontheBet { get; set; }

        public decimal LastMontheCommison { get; set; }
        public decimal LastMontheBet { get; set; }
    }

    public class NationallotteryCardsHistoryLogic
    {
        public static List<BetViewModel> GetBetViews(string viewType)
        {
            List<BetViewModel> betViews = new List<BetViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    DateTime filterdate = new DateTime();
                    if (viewType == ViewFilters.Today)
                    {
                        filterdate = DateTime.UtcNow;

                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<BetViewModel> data = new List<BetViewModel>();
                            data = (from _bet in db.PlayerBets
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    where _bet.Insertdate.Date == filterdate.Date
                                    select new BetViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        amount = _bet.Amount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        transactionId = _bet.TransactionId,
                                        Insertdate = _bet.Insertdate
                                    }).OrderByDescending(x => x.Insertdate).ToList();
                            if (data != null && data.Count() > 0)
                            {
                                betViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.Week)
                    {
                        filterdate = DateTime.UtcNow.AddDays(-7);
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<BetViewModel> data = new List<BetViewModel>();
                            data = (from _bet in db.PlayerBets
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    where _bet.Insertdate.Date >= filterdate.Date
                                    select new BetViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        amount = _bet.Amount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        transactionId = _bet.TransactionId,
                                        Insertdate = _bet.Insertdate
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                betViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.Month)
                    {
                        filterdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<BetViewModel> data = new List<BetViewModel>();
                            data = (from _bet in db.PlayerBets
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    where _bet.Insertdate.Date >= filterdate.Date
                                    select new BetViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        amount = _bet.Amount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        transactionId = _bet.TransactionId,
                                        Insertdate = _bet.Insertdate
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                betViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.LifeTime)
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<BetViewModel> data = new List<BetViewModel>();
                            data = (from _bet in db.PlayerBets
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    select new BetViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        amount = _bet.Amount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        transactionId = _bet.TransactionId,
                                        Insertdate = _bet.Insertdate
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                betViews = data;
                            }
                        }
                    }

                    if (betViews != null && betViews.Count() > 0)
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            foreach (var item in betViews)
                            {
                                var gamdata = db.Gamelists.Where(x => x.GameId == item.gameId).FirstOrDefault();
                                item.GameName = gamdata != null ? gamdata.GameName : "NA";
                            }
                        }
                    }

                }

            }
            catch { }
            return betViews;
        }

        public static List<WinViewModel> GetWinViews(string viewType)
        {
            List<WinViewModel> winViews = new List<WinViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(viewType))
                {
                    DateTime filterdate = new DateTime();
                    if (viewType == ViewFilters.Today)
                    {
                        filterdate = DateTime.UtcNow;
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<WinViewModel> data = new List<WinViewModel>();
                            data = (from _bet in db.Playwins
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    join gm in db.Gamelists on _bet.GameId equals gm.GameId
                                    where _bet.InsertAt.Date == filterdate.Date
                                    select new WinViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        betamount = _bet.BetAmount == null ? 0 : _bet.BetAmount.Value,
                                        winamount = _bet.JackpotAmount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        GameName = gm.GameName,
                                        transactionId = _bet.TransactionId,
                                        type = _bet.Type,
                                        freeRoundsRemaining = _bet.FreeRoundsRemaining,
                                        Insertdate = _bet.InsertAt
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                winViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.Week)
                    {
                        filterdate = DateTime.UtcNow.AddDays(-7);
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<WinViewModel> data = new List<WinViewModel>();
                            data = (from _bet in db.Playwins
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    join gm in db.Gamelists on _bet.GameId equals gm.GameId
                                    where _bet.InsertAt.Date >= filterdate.Date
                                    select new WinViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        betamount = _bet.BetAmount == null ? 0 : _bet.BetAmount.Value,
                                        winamount = _bet.JackpotAmount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        GameName = gm.GameName,
                                        transactionId = _bet.TransactionId,
                                        type = _bet.Type,
                                        freeRoundsRemaining = _bet.FreeRoundsRemaining,
                                        Insertdate = _bet.InsertAt
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                winViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.Month)
                    {
                        filterdate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<WinViewModel> data = new List<WinViewModel>();
                            data = (from _bet in db.Playwins
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    join gm in db.Gamelists on _bet.GameId equals gm.GameId
                                    where _bet.InsertAt.Date >= filterdate.Date
                                    select new WinViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        betamount = _bet.BetAmount == null ? 0 : _bet.BetAmount.Value,
                                        winamount = _bet.JackpotAmount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        GameName = gm.GameName,
                                        transactionId = _bet.TransactionId,
                                        type = _bet.Type,
                                        freeRoundsRemaining = _bet.FreeRoundsRemaining,
                                        Insertdate = _bet.InsertAt
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                winViews = data;
                            }
                        }
                    }
                    else if (viewType == ViewFilters.LifeTime)
                    {
                        using (QuickbetDbEntities db = new QuickbetDbEntities())
                        {
                            List<WinViewModel> data = new List<WinViewModel>();
                            data = (from _bet in db.Playwins
                                    join uu in db.Users on _bet.ExternalPlayerIdUserId equals uu.Id
                                    join gm in db.Gamelists on _bet.GameId equals gm.GameId
                                    select new WinViewModel
                                    {
                                        Id = _bet.Id,
                                        Player_UserId = _bet.ExternalPlayerIdUserId,
                                        PlayerName = uu.Name,
                                        PlayerEmail = uu.Email,
                                        betamount = _bet.BetAmount == null ? 0 : _bet.BetAmount.Value,
                                        winamount = _bet.JackpotAmount,
                                        currency = _bet.Currency,
                                        gameId = _bet.GameId,
                                        GameName = gm.GameName,
                                        transactionId = _bet.TransactionId,
                                        type = _bet.Type,
                                        freeRoundsRemaining = _bet.FreeRoundsRemaining,
                                        Insertdate = _bet.InsertAt
                                    }).OrderByDescending(x => x.Insertdate).ToList();

                            if (data != null && data.Count() > 0)
                            {
                                winViews = data;
                            }
                        }
                    }



                }

            }
            catch { }
            return winViews;
        }
    }
}
