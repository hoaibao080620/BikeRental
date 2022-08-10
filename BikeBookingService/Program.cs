using BikeBookingService.Extensions;
using BikeBookingService.GrpcServices;
using BikeBookingService.MessageQueue.Consumer;
using Hangfire;
using Hangfire.PostgreSql;
using Sentry;

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
builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://fbcceeae5f574378be05527dc77f1666@o1326695.ingest.sentry.io/6587240";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
    // We recommend adjusting this value in production.
    o.TracesSampleRate = 1.0;
    o.MinimumBreadcrumbLevel = LogLevel.Debug;
    o.AttachStacktrace = true;
    o.DiagnosticLevel = SentryLevel.Error;
});
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Hangfire")));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseGrpcWeb();
app.UseHangfireDashboard();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHangfireDashboard();
app.MapGrpcService<BikeBookingGrpcService>().EnableGrpcWeb();
app.UseSentryTracing();
app.RegisterMessageHandler();
app.Run();
