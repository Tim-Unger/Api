using Microsoft.EntityFrameworkCore;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseReDoc(
    options =>
    {
        options.DocumentTitle = "Api Documentation";
        options.SpecUrl = "/swagger/v1/swagger.json";
    }
);

app.UseHttpsRedirection();

app.UseResponseCaching();

//app.UseCors();

app.MapControllers();

app.Run();
