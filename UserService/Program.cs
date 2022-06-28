using UserService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddMessageQueueServices();
builder.Services.AddScopedServices();
builder.Services.AddHttpClientToServices();
await builder.Services.SyncOktaUsers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
