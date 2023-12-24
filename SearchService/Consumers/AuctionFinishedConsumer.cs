using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine($"--> Consume AuctionFinished event started From Search service: {context.Message.AuctionId}");
        var auction = await DB.Find<ItemAuction>().OneAsync(context.Message.AuctionId);

        if (context.Message.ItemSold)
        {
            auction.SoldAmount = context.Message.Amount;
            auction.Winner = context.Message.Winner;
        }

        auction.Status = auction.SoldAmount >= auction.ReservePrice
            ? "Finished"
            : "ReserveNotMet";

        await auction.SaveAsync();
    }
}