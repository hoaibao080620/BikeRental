using AccountService.BackgroundJob;
using AccountService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<MessageQueueConsumer>();
builder.Services.AddScopedServices();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddSingletonServices();
// builder.Services.RegisterMessageHandlers();
// builder.Services.RunMigrations();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
