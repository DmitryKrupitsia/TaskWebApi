using Microsoft.EntityFrameworkCore;
using TaskWebApi.Data;
using TaskWebApi.Models;
using TaskWebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// Add services to the container
// ----------------------------

// Add controllers
builder.Services.AddControllers();

// Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// DB context (connection from appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HysTestTaskDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// ----------------------------
// Build the app
// ----------------------------
var app = builder.Build();

// ----------------------------
// Configure middleware
// ----------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
