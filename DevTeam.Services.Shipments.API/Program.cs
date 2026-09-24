using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using DevTeam.Services.Shipments.Applications.Messaging.Queues;
using DevTeam.Services.Shipments.Domain.Entities;
using DevTeam.Services.Shipments.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharedKernel.Helpers;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;
using SharedKernel.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =====================================================
// 1. Controllers
// =====================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


// =====================================================
// 2. Authentication - JWT
// =====================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        ))
            };
    });


// =====================================================
// 3. Authorization - Dynamic Permissions
// =====================================================

builder.Services.AddAuthorization();

builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

builder.Services.AddSingleton<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();


// =====================================================
// 4. MediatR - CQRS
// =====================================================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateShipmentHandler).Assembly);
});


// =====================================================
// 5. Swagger - JWT Bearer
// =====================================================

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "Bearer",
                    document)
            ] = new List<string>()
        });
});


// =====================================================
// 6. Database - PostgreSQL
// =====================================================

builder.Services.AddDbContext<ConnectAppDbContext>(
    options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(
                "DefaultConnection")));


// =====================================================
// 7. Unit Of Work
// =====================================================

builder.Services.AddScoped<
    IUnitOfWork<ConnectAppDbContext>,
    UnitOfWork<ConnectAppDbContext>>();


// =====================================================
// 8. Repository
// =====================================================

builder.Services.AddScoped<
    IRepository<Shipment>,
    Repository<Shipment, ConnectAppDbContext>>();

builder.Services.AddScoped<
    IRepository<ShipmentEvent>,
    Repository<ShipmentEvent, ConnectAppDbContext>>();

builder.Services.AddScoped<
    IRepository<Carrier>,
    Repository<Carrier, ConnectAppDbContext>>();

builder.Services.AddScoped<
    IRepository<Location>,
    Repository<Location, ConnectAppDbContext>>();



// =====================================================
// 9. RabbitMQ
// =====================================================

builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<
    IRabbitMQPublisher,
    RabbitMQPublisher>();


// =====================================================
// 10. Redis Cache
// =====================================================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});

builder.Services.AddScoped<ICache, Cache>();


// =====================================================
// 11. Build Application
// =====================================================

var app = builder.Build();


// =====================================================
// 12. Middleware
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();