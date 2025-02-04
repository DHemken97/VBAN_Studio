
using VBAN_Studio.WebServer;

VbanStudioContext.VbanStudioEnvironment = new VBAN_Studio.Common.VbanStudioEnvironment(@"C:\Users\Dominic\source\repos\VBAN_Studio\VBAN_Studio.WebServer\bin\Debug\net8.0\Plugins");

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
