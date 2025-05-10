using Core.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));


// Scoped lifetime: one per request
builder.Services.AddScoped<IStudentRepository, AppDbContext>();
// Service layer
builder.Services.AddScoped<StudentHandlers>();
// If using MediatR
builder.Services.AddMediatR(typeof(StudentHandlers).Assembly);


// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allows all origins (relaxed for development)
              .AllowAnyMethod() // Allows all HTTP methods (GET, POST, etc.)
              .AllowAnyHeader(); // Allows all headers
    });
    // For production, use a more restrictive policy, e.g.:
    // options.AddPolicy("AllowSpecific", policy =>
    // {
    //     policy.WithOrigins("http://localhost:3000", "https://your-frontend.com")
    //           .AllowAnyMethod()
    //           .AllowAnyHeader();
    // });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Use "AllowSpecific" for production
app.UseRouting();
app.Run();
