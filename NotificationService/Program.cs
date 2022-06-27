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
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", innerBuilder => innerBuilder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials().SetIsOriginAllowed(_ => true));
});
builder.Services.AddSignalR();
// builder.Services.RegisterMessageHandlers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BikeLocationHub>("/hub");
app.RegisterMessageHandler();

app.Run();
