using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts.Events;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdateAd.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        var auctions = await query
            .AsTracking()
            .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return auctions;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction is null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        
        auction.Seller = User.Identity!.Name;
        await _context.Auctions.AddAsync(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        var createdAuctionContract = _mapper.Map<AuctionCreated>(newAuction);

        await _publishEndpoint.Publish(createdAuctionContract);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not create auction in the DB");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null) return NotFound("Could not found the auction");
        
        if (auction.Seller != User.Identity!.Name) return Forbid();
        
        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;

        var auctionUpdatedContract = _mapper.Map<AuctionUpdated>(auction);

        await _publishEndpoint.Publish(auctionUpdatedContract);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not update the auction in the DB");

        return Ok();
    }
    
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction is null) return NotFound();

        if (auction.Seller != User.Identity!.Name) return Forbid();
        
        _context.Auctions.Remove(auction);
        await _publishEndpoint.Publish(new AuctionDeleted() { Id = auction.Id.ToString() });
        var result = await _context.SaveChangesAsync() > 0;

        if (!result) BadRequest("Could not detele the auction");

        return Ok();
    }
}