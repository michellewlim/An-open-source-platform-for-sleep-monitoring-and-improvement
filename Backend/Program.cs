using Backend.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDatabaseController, DatabaseController>();

builder.WebHost.ConfigureKestrel(serverOptions => {
    serverOptions.ConfigureEndpointDefaults(listenOptions =>{
        listenOptions.UseHttps();
    });
    serverOptions.ConfigureHttpsDefaults(listenOptions => {
    });
});

builder.WebHost.UseUrls("https://0.0.0.0");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();