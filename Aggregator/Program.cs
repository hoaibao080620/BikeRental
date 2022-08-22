using Aggregator.Controllers;
using Aggregator.Extensions;
using Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
});
builder.Services.AddOktaAuthenticationService(builder.Configuration);
builder.Services.RegisterGrpcClient();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IViewRender, ViewRender>();
builder.Services.AddHttpClient<SampleImportController>("s3", option =>
{
    option.BaseAddress = new Uri("https://bike-rental-fe.s3.amazonaws.com");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
