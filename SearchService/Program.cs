using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSrvHttpClient>()
    .AddPolicyHandler(Retry());
builder.Services.AddMassTransit(x =>
{
    // registring consumers.
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    //this is to name the consumer like search-auction-created in queue.
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cnfg) =>
    {
        cnfg.Host(builder.Configuration["RabbitMq:Host"],"/", host =>
        {
            host.Username(builder.Configuration.GetValue<string>("RabbitMq:Username","guest"));
            host.Password(builder.Configuration.GetValue<string>("RabbitMq:Password","guest"));
        });
        // cnfg.ReceiveEndpoint("search-auction-created", e =>
        // {
        //     e.UseMessageRetry(r => r.Interval(6, 5));
        //     e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        // });
        cnfg.ReceiveEndpoint(cnfg =>
        {
            cnfg.UseMessageRetry(cn=> cn.Interval(6,5));
            cnfg.ConfigureConsumers(context);
        });
        
        cnfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(TryInitDb);

app.Run();

static IAsyncPolicy<HttpResponseMessage> Retry() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

async void TryInitDb()
{
    try
    {
        await DbInitializer.InitDbAsync(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}