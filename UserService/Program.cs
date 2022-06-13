using UserService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddMessageQueueServices();
builder.Services.AddDbContextService(builder.Configuration);
builder.Services.AddScopedServices();
builder.Services.AddHttpClientToServices();
// await builder.Services.SyncOktaUsers();


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
