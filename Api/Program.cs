using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddResponseCaching();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseReDoc(
    options =>
    {
        options.DocumentTitle = "Api Documentation";
        options.SpecUrl = "/swagger/v1/swagger.json";
        options.RoutePrefix = string.Empty;
    }
);

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseCors();

app.MapControllers();

app.Run();