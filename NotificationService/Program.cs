using NotificationService.Extensions;
using NotificationService.Hub;
using NotificationService.MessageQueue.BackgroundJob;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices();
builder.Services.AddSignalR();
builder.Services.RegisterMessageHandlers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", innerBuilder => innerBuilder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials().SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BikeLocationHub>("/bikeLocationHub");

app.Run();
