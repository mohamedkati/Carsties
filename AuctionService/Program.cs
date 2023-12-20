using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
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