using BikeTrackingService.Extensions;
using BikeTrackingService.MessageQueue.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddDbContextService(builder.Configuration); 
builder.Services.AddHttpClientToServices();
builder.Services.RunMigrations();
builder.Services.AddGrpcClient<BikeServiceGrpc.BikeServiceGrpcClient>(c =>
{
    c.Address = new Uri("https://bike-service-13062022.herokuapp.com");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.RegisterMessageHandler();
app.Run();
