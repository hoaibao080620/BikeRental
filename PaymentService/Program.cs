using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

StripeConfiguration.ApiKey = "sk_test_51I9C85F9AODfnpB1KCouR1uyomvh4oWzVjOiWyOfCIWXn5ckSoTNyIUYuocuhG4J2hOJWzvr4gN4bLeqwThDfF9f00rIb64SYv";

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
