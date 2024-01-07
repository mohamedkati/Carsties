using AutoMapper;
using BidService.DTOs;
using BidService.Models;
using Contracts.Events;

namespace BidService.RequestHelpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        this.CreateMap<Bid, BidDto>();
        this.CreateMap<Bid, BidPlaced>();

    }
}