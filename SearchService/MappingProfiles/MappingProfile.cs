using AutoMapper;
using Contracts.Events;
using SearchService.Models;

namespace SearchService.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();

    }
}