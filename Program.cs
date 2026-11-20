using StudentManagementAPI.Database;

var builder = WebApplication.CreateBuilder(args);

// Ensure Development environment by default if running standalone
if (builder.Environment.IsProduction() &&
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null)
{
    builder.Environment.EnvironmentName = "Development";
}

// Add logging configuration
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
});

// Add CORS policy to allow cross-origin requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-initialize / verify database and table schema on startup
var connString = app.Configuration.GetConnectionString("DefaultConnection");
var logger = app.Services.GetRequiredService<ILogger<Program>>();
DatabaseInitializer.Initialize(connString, logger);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentManagementAPI v1");
    c.RoutePrefix = "swagger";
});
Console.WriteLine("Swagger UI is available at /swagger/index.html");

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
