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
builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://fbcceeae5f574378be05527dc77f1666@o1326695.ingest.sentry.io/6587240";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
    // We recommend adjusting this value in production.
    o.TracesSampleRate = 1.0;
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseGrpcWeb();
app.UseAuthentication();
app.UseAuthorization();
app.UseSentryTracing();
app.MapControllers();
app.MapGrpcService<AccountGrpcService>().EnableGrpcWeb();
app.Run();
