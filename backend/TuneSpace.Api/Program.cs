using Microsoft.AspNetCore.Identity;
using Serilog;
using TuneSpace.Api.Infrastructure;
using TuneSpace.Application;
using TuneSpace.Infrastructure;
using TuneSpace.Infrastructure.Hubs;
using TuneSpace.Infrastructure.Identity;
using TuneSpace.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

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

builder.Services.AddOptions(builder.Configuration);
builder.Services.AddDatabaseServices();
builder.Services.AddRepositoryServices();
builder.Services.AddCachingServices();
builder.Services.AddIdentityServices();
builder.Services.AddHttpClientServices();

builder.Services.AddApplicationServices();

builder.Services.AddSignalR();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var roleSeeder = new RoleSeeder(roleManager);
    await roleSeeder.SeedRolesAsync();
}

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SocketHub>("/socket-hub");

app.Run();
