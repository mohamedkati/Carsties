using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.GrpcServices;

public class GrpcAuctionService(AuctionDbContext auctionDbContext, ILogger<GrpcAuctionService> logger)
    : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _auctionDbContext = auctionDbContext;

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        logger.LogInformation("==> Receiving Request from Auction Grpc Service Server : for auction id = {id}",
            request.Id);
        var auction = await _auctionDbContext.Auctions.FindAsync(Guid.Parse(request.Id)) ??
                      throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));

        var auctionResponse = new GrpcAuctionResponse()
        {
            Auction = new()
            {
                Id = auction.Id.ToString(),
                AuctionEnd = auction.AuctionEnd.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller
            }
        };
        return auctionResponse;
    }
}