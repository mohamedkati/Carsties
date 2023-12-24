using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSrvHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSrvHttpClient( HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetDataFromAuctionService()
    {
        var lastUpdateAt = await DB.Find<Item, string>()
            .Sort(x => x.Descending(a => a.UpdateAd))
            .Project(x => x.UpdateAd.ToString())
            .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(
            $"{_config["AuctionServiceUrl"]}/api/auctions?date={lastUpdateAt}");
    }
}