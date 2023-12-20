using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        this.CreateMap<Auction, AuctionDto>()
            .IncludeMembers(x => x.Item);
        this.CreateMap<Item, AuctionDto>();

        this.CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, opt => opt.MapFrom(s => s));
        this.CreateMap<CreateAuctionDto, Item>();
    }
}