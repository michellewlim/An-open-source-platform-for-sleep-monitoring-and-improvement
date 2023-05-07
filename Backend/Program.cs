using Backend.Controllers;
using Backend.Helpers;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("appsettings.Production.json");
builder.Configuration.AddEnvironmentVariables();
//builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddSingleton<IDatabaseController, DatabaseController>();
builder.Services.AddSingleton<IOptimizationScheduler, OptimizationScheduler>();
builder.Services.AddSingleton<IFitbitController, FitbitController>();
builder.Services.AddSingleton<IFitbitAuthenticator, FitbitAuthenticator>();
builder.Services.AddSingleton<INestController, NestController>();
builder.Services.AddSingleton<INestAuthenticator, NestAuthenticator>();
builder.Services.AddTransient<IOptimizer, Optimizer>();
// builder.WebHost.ConfigureKestrel(serverOptions => {
//     serverOptions.ConfigureEndpointDefaults(listenOptions =>{
//         listenOptions.UseHttps();
//     });
//     serverOptions.ConfigureHttpsDefaults(listenOptions => {
//     });
// });

//builder.WebHost.UseUrls("http://127.0.0.1:5000");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseAuthorization();

app.MapControllers();

app.Run();
