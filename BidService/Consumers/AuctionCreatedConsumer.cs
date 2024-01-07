using BidService.Models;
using Contracts.Events;
using MassTransit;
using MongoDB.Entities;

namespace BidService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"Consuming AuctionCreated Event From BidService auctionid=>= {context.Message.Id}");
        var auction = new Auction()
        {
            ID = context.Message.Id.ToString(),
            AuctionEnd = context.Message.AuctionEnd,
            ReservePrice = context.Message.ReservePrice,
            Seller = context.Message.Seller,
        };
        await auction.SaveAsync();
    }
}