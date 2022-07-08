using BikeService.Sonic.Extensions;
using BikeService.Sonic.GrpcServices;
using BikeService.Sonic.Services.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddSingletonServices(builder.Configuration);
builder.Services.AddElasticClient(builder.Configuration);
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpClientToServices();
builder.Services.RunMigrations();


// App pipelines
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BikeLocationHub>("/bikeLocationHub");
app.MapGrpcService<BikeGrpcService>();

app.Run();
