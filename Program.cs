using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Pagination.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

{
	//Read Configuration from appSettings  
	var config = new ConfigurationBuilder()
		.AddJsonFile("appsettings.json")
		.Build();
	//Initialize Logger 
	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(config)
		.CreateLogger();
}

builder. Host.UseSerilog();  //UseSerilog method is called on Host instead of WebApplication

  //Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("cnStr")));

  // CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "MyCors", builder =>
	{
		builder.WithOrigins("http://localhost:3000")
			.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

 //Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
app.UseRouting();
app.UseAuthorization();
app.UseCors("MyCors");
app.MapControllers();

app.Run();
