using AuctionService.Data;
using Contracts.Events;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext dbContext) : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbContext = dbContext;

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine($"Consuming Bid Placed Event From auction service :  {context.Message.AuctionId}");

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse((context.Message.AuctionId)));

        if (auction == null)
        {
            Console.WriteLine($"No Auction found with this ID :  {context.Message.AuctionId}");
            return;
        }

        if (auction.CurrentHighBid is null || (context.Message.Amount > auction.CurrentHighBid &&
                                               context.Message.BidStatus.ToLower().Contains("accepted")))
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _dbContext.SaveChangesAsync();
        }
    }
}