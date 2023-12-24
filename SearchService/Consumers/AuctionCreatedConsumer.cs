using AutoMapper;
using Contracts.Events;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"--> Consume AuctionCreated event started : {context.Message.Id}");

        var item = _mapper.Map<ItemAuction>(context.Message);

        await item.SaveAsync();
        
        Console.WriteLine($"--> Consumed AuctionCreated,item created");
    }
}