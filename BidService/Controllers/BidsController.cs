using AutoMapper;
using BidService.DTOs;
using BidService.Models;
using BidService.Services.GrpcServices;
using Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BidService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly GrpcAuctionClientService _auctionClient;

    public BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClientService auctionClient)
    {
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _auctionClient = auctionClient;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount)
    {
        var auction = await DB.Find<Auction>().OneAsync(auctionId);

        if (auction is null)
        {
            auction = await _auctionClient.GetAuction(auctionId);
            
            if (auction is null) return BadRequest("Cannot create bids on this action at this time");
            
            await auction.SaveAsync();
        }

        if (auction.Seller == User.Identity.Name)
        {
            return BadRequest("You cannot place a bid on your own car");
        }

        var bid = new Bid()
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity.Name
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.finished;
        }
        else
        {
            var highBid = await DB.Find<Bid>()
                .Match(x => x.AuctionId == auctionId)
                .Sort(x => x.Descending(a => a.Amount))
                .ExecuteFirstAsync();

            if ((highBid is not null && highBid.Amount < amount) || highBid is null)
            {
                bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;
            }
            else if (highBid is not null && highBid.Amount > amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }

        await bid.SaveAsync();
        var bidPlaced = _mapper.Map<BidPlaced>(bid);
        await _publishEndpoint.Publish<BidPlaced>(bidPlaced);

        return Ok(_mapper.Map<BidDto>(bid));
    }

    [HttpGet("{auctionId}")]
    public async Task<ActionResult<List<BidDto>>> GetByAuctionId(string auctionId)
    {
        var bids = await DB.Find<Bid>()
            .Match(x => x.AuctionId == auctionId)
            .Sort(x => x.Descending(a => a.BidTime))
            .ExecuteAsync();
        return _mapper.Map<List<BidDto>>(bids);
    }
}