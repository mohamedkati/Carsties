using BidService.Models;
using Contracts.Events;
using MassTransit;
using MongoDB.Entities;

namespace BidService.Services.Background;

public class CheckAuctionFinishedService(ILogger<CheckAuctionFinishedService> logger, IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly ILogger<CheckAuctionFinishedService> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting checkAuctionFinishedService");

        stoppingToken.Register(() => _logger.LogInformation("Stopping checkAuctionFinishedService"));

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckFinishedAuctions(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task CheckFinishedAuctions(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var endpointPublisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var auctions = await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished)
            .ExecuteAsync(stoppingToken);

        _logger.LogInformation("==> Found {count} auctions completed", auctions.Count);

        async void CheckAuction(Auction auction)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);
            
            var winnerBid = await DB.Find<Bid>()
                .Match(x => x.AuctionId == auction.ID)
                .Match(x => x.BidStatus == BidStatus.Accepted)
                .Sort(x => x.Descending(b => b.Amount))
                .ExecuteFirstAsync(stoppingToken);

            var auctionFinished = new AuctionFinished()
            {
                ItemSold = winnerBid is not null,
                AuctionId = auction.ID,
                Amount = winnerBid?.Amount,
                Seller = auction.Seller,
                Winner = winnerBid?.Bidder
            };
            await endpointPublisher.Publish(auctionFinished, stoppingToken);
        }

        auctions.AsParallel().ForAll(CheckAuction);
    }
}