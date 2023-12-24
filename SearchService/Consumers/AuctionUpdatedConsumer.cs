using AutoMapper;
using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine($"--> Consume AuctionUpdated event started : {context.Message.Id}");

        var updatedItem = _mapper.Map<ItemAuction>(context.Message);

        var auctionInDb = await updatedItem.SaveOnlyAsync(new List<string>()
            { "Model", "Make", "Color", "Year", "Mileage" });
        Console.WriteLine($"--> Consumed AuctionUpdated, result updated => : {auctionInDb.MatchedCount}");
    }
}