using AccountService.BackgroundJob;
using AccountService.Extensions;
using AccountService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddScopedServices();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddSingletonServices();
builder.Services.RegisterMessageHandlers();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseGrpcWeb();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<AccountGrpcService>().EnableGrpcWeb();
app.Run();
