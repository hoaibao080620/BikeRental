using BikeBookingService.Extensions;
using BikeBookingService.GrpcServices;
using BikeBookingService.MessageQueue.Consumer;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddHttpClientToServices();
builder.Services.RunMigrations();
builder.Services.RegisterGrpcClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseGrpcWeb();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<BikeBookingGrpcService>().EnableGrpcWeb();

app.RegisterMessageHandler();
app.Run();
