using PaymentService.Extensions;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.AddSingletonService();
builder.Services.AddScopeService();

var app = builder.Build();


StripeConfiguration.ApiKey = builder.Configuration["Stripe:APIKey"];

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
