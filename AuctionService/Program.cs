using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    // Configure Entity framework to store sent message to service bus (RabbitMQ) into a database,
    // so if the service bus was down then It will send the messages that are not delivred yet cuz of the unvailability of the service bus
    // that means when the service bus come back up, then all the messages that not delivred previously will be sent to the service bus
    // and we can achieve that by storing all messages into a database, then configure MassTransit to look retry every second.
    x.AddEntityFrameworkOutbox<AuctionDbContext>(config =>
    {
        config.QueryDelay = TimeSpan.FromSeconds(10);
        config.UsePostgres();
        config.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    x.UsingRabbitMq((context, config) =>
        {
            config.ReceiveEndpoint(cnfg =>
            {
                cnfg.UseMessageRetry(cn => cn.Interval(6, 5));
                cnfg.ConfigureConsumers(context);
            });
            config.ConfigureEndpoints(context);
        }
    );
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Authority = builder.Configuration["IdentityServiceUrl"];
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters.ValidateAudience = false;
        opt.TokenValidationParameters.NameClaimType = "username";
    });


var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine("Error while seeding data");
}

app.Run();