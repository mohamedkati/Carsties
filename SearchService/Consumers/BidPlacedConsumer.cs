using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine($"--> Consume BidPlaced event started From Search service: {context.Message.AuctionId}");
        var auction = await DB.Find<ItemAuction>().OneAsync(context.Message.AuctionId);

        if (context.Message.BidStatus.ToLower().Contains("accepted") && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}