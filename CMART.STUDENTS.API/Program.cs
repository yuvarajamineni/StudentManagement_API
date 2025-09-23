using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CMART.STUDENTS.SERVICES.Services;
using CMART.STUDENTS.INFRASTRUCTURE.Respositories;
using CMART.STUDENTS.CORE.Models;

var builder = WebApplication.CreateBuilder(args);

// Add console logging and your existing custom file logger
builder.Logging.AddConsole();
builder.Logging.AddProvider(new SimpleFileLoggerProvider("logs/student_api_log.txt"));

// Configure MongoDB settings
builder.Services.Configure<StudentStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(StudentStoreDatabaseSettings)));

builder.Services.AddSingleton<IStudentStoreDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<StudentStoreDatabaseSettings>>().Value);

// MongoDB client
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("StudentStoreDatabaseSettings:ConnectionString")));

// Register repository + service
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - allow all origins, methods, headers (for testing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Force URLs
builder.WebHost.UseUrls("http://localhost:5212", "https://localhost:5213");

var app = builder.Build();

// Log application startup
app.Logger.LogInformation("Application starting up");

// Middleware to log HTTP request start and end
app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next.Invoke();
    app.Logger.LogInformation("Finished handling request");
});

// Enable CORS
app.UseCors("AllowAll");

// Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API");
        c.RoutePrefix = string.Empty; // Opens Swagger at root (http://localhost:5212/)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Log application ready state
app.Logger.LogInformation("Application started and ready to handle requests");

app.Run();
