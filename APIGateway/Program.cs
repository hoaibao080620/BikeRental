using Ocelot.Middleware;
using WebBFFGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials().SetIsOriginAllowed(_ => true));
});

builder.Services.AddOcelotConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();
app.UseOcelot().Wait();
app.Run();
