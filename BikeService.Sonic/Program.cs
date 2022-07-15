using BikeService.Sonic.BackgroundJob;
using BikeService.Sonic.Extensions;
using BikeService.Sonic.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices(builder.Configuration);
builder.Services.AddElasticClient(builder.Configuration);
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpClientToServices();
builder.Services.RunMigrations();
builder.Services.RegisterMessageHandlers();
builder.Services.AddHostedService<MessageQueueConsumer>();


// App pipelines
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseGrpcWeb();
app.MapControllers();
app.MapGrpcService<BikeGrpcService>().EnableGrpcWeb();

app.Run();
