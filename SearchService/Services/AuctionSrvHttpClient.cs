using System.Globalization;
using System.Text;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSrvHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSrvHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<ItemAuction>> GetDataFromAuctionService()
    {
        var lastUpdateAt = await DB.Find<ItemAuction, string>()
            .Sort(x => x.Descending(a => a.UpdateAd))
            .Project(x => x.UpdateAd.ToString())
            .ExecuteFirstAsync();

        var url = new StringBuilder().Append($"{_config["AuctionServiceUrl"]}/api/auctions");
        if (!string.IsNullOrEmpty(lastUpdateAt))
            url.Append($"?date={lastUpdateAt}");

        return await _httpClient.GetFromJsonAsync<List<ItemAuction>>(
            url.ToString());
    }
}