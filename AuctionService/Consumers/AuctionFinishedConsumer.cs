using AuctionService.Data;
using AuctionService.Entities;
using Contracts.Events;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer(AuctionDbContext dbContext) : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _dbContext = dbContext;
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine($"Consuming Auction Finished Event From auction service:  {context.Message.AuctionId}");
        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));
        if (auction == null)
        {
            Console.WriteLine($"No Auction found with this ID :  {context.Message.AuctionId}");
            return;
        }
        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;

        await _dbContext.SaveChangesAsync();

    }
}