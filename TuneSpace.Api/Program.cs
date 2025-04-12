using Serilog;
using TuneSpace.Api.Infrastructure;
using TuneSpace.Application;
using TuneSpace.Infrastructure;
using TuneSpace.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

//Add logger
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

//Add cors
builder.Services.AddCors(options =>
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.WithOrigins(
                "http://127.0.0.1:5173",
                "http://127.0.0.1:5173/",
                "http://localhost:5173",
                "http://localhost:5173/",
                "http://localhost")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    })
);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors("AllowOrigin");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chats");

app.MapHub<NotificationHub>("/notifications");

app.Run();
