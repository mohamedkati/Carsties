using AuctionService;
using BidService.Models;
using Grpc.Net.Client;

namespace BidService.Services.GrpcServices;

public class GrpcAuctionClientService(ILogger<GrpcAuctionClientService> logger, IConfiguration config)
{
    public async Task<Auction> GetAuction(string id)
    {
        logger.LogInformation("Start Calling Auction GrpcServices for id ==> {id}",id);
    
        var channel = GrpcChannel.ForAddress(config["GrpcAuctionServer"] ?? string.Empty);
        var client = new GrpcAuction.GrpcAuctionClient(channel);
        var request = new GetAuctionRequest() { Id = id };
        try
        {
            var response = await client.GetAuctionAsync(request);
            return new Auction()
            {
                ID = response.Auction.Id,
                AuctionEnd = DateTime.Parse(response.Auction.AuctionEnd),
                ReservePrice = response.Auction.ReservePrice,
                Seller = response.Auction.Seller
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex," Could not call Grpc server");
            return null;
        }
    }
}