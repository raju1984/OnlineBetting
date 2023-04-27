using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuickBetCore
{
    public class TaskRunner : IHostedService, IDisposable
    {
        private readonly ILogger<TaskRunner> logger;
        private Timer timer;
        private Timer walltetTimer;
        private IHostEnvironment _hostEnv;
        private readonly IServiceScopeFactory _scopeFactory;
        private static bool PaymentStartLock = false;
        public TaskRunner(ILogger<TaskRunner> logger,
            IHostEnvironment hostEnv,
            IServiceScopeFactory scopeFactory
            )
        {
            this.logger = logger;
            _hostEnv = hostEnv;
            _scopeFactory = scopeFactory;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timer Started");
            //Timer - 900000=15min ,130000=2min
            walltetTimer = new Timer(o =>
            {
                CheckInComingWalletAmount();
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(130000));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timer Stopped...");

            return Task.CompletedTask;
        }

        private async void CheckInComingWalletAmount()
        {
            if (PaymentStartLock == false)
            {
                PaymentStartLock = true;
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        using (QuickbetDbEntities dbEntities = new QuickbetDbEntities())
                        {
                            var cashBackOffers = dbEntities.CashBackOffersTransactions.Where(a => a.IsCreditToWallet == false
                            && a.AmountSpendToUnlock >= a.AmountToUnlockCashback).ToList();
                            if(cashBackOffers!=null)
                            {
                                foreach(var item in cashBackOffers)
                                {
                                    try
                                    {
                                        item.IsCreditToWallet = true;
                                        PaymentDb.CreditCashBackAmount(item.CashBackAmount,item.UserId,item.Id);
                                        dbEntities.SaveChanges();
                                    }
                                    catch(Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error);
                        return;
                    }

                    PaymentStartLock = false;
                }
            }
        }
    }
}
