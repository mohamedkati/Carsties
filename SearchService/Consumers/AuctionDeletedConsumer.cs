using Contracts.Events;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine($"--> Consume AuctionDeleted event started : {context.Message.Id}");

        var item = await DB.Collection<Item>().DeleteOneAsync(x => x.ID == context.Message.Id);
        Console.WriteLine($"--> Consumed AuctionDeleted, result deleted => : {item.DeletedCount}");
    }
}