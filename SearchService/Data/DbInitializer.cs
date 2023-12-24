using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDbAsync(WebApplication app)
    {
        await DB.InitAsync("SearchAuctionsDb",
            MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Seller, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .Key(x => x.Status, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();
        Console.WriteLine(count + " found in mongo db");
        using var scope = app.Services.CreateScope();

        var httpClientService = scope.ServiceProvider.GetRequiredService<AuctionSrvHttpClient>();

        var items = await httpClientService.GetDataFromAuctionService();
        if (items.Any())
        {
            Console.WriteLine(items.Count + " found to update");
            await DB.SaveAsync(items);
        }
    }
}