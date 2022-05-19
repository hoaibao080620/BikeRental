using Ocelot.Middleware;
using WebBFFGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelotConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseOcelot().Wait();
app.Run();