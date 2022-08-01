using Grpc.Net.Client.Web;
using NotificationService.Extensions;
using NotificationService.Hubs;
using NotificationService.MessageQueue.BackgroundJob;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", innerBuilder => innerBuilder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials().SetIsOriginAllowed(_ => true));
});
builder.Services.AddGrpcClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService", c =>
{
    c.Address = new Uri("https://bike-service-13062022.herokuapp.com");
}).ConfigureChannel(o =>
{
    o.HttpHandler = new GrpcWebHandler(new HttpClientHandler());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMongoDb(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hub");
app.RegisterMessageHandler();

app.Run();
