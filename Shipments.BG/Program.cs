using DevTeam.Application.Messaging;
using DevTeam.Application.Messaging.ShipmentEventMessage;
using DevTeam.Application.Messaging.ShipmentMessages;

using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using DevTeam.Services.Shipments.Applications.Messaging.Queues;
using DevTeam.Services.Shipments.Domain.Entities;
using DevTeam.Services.Shipments.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

using SharedKernel.Interfaces;
using SharedKernel.Repositories;
using SharedKernel.Services;

using Shipments.BG.Consumer;
using ShipmentWorker;


// ========================================
// Create Host Builder
// ========================================

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;


// ========================================
// 1. Database
// ========================================

builder.Services.AddDbContext<ConnectAppDbContext>(options =>
{
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"));
});


// ========================================
// 2. Unit Of Work
// ========================================

builder.Services.AddScoped<
    IUnitOfWork<ConnectAppDbContext>,
    UnitOfWork<ConnectAppDbContext>>();


// ========================================
// 3. Repository
// ========================================

// Shipment
builder.Services.AddScoped<
    IRepository<Shipment>,
    Repository<Shipment, ConnectAppDbContext>>();

// ShipmentEvent
builder.Services.AddScoped<
    IRepository<ShipmentEvent>,
    Repository<ShipmentEvent, ConnectAppDbContext>>();

// Carrier
builder.Services.AddScoped<
    IRepository<Carrier>,
    Repository<Carrier, ConnectAppDbContext>>();

// Location
builder.Services.AddScoped<
    IRepository<Location>,
    Repository<Location, ConnectAppDbContext>>();


// ========================================
// 4. MediatR
// ========================================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateShipmentCommand).Assembly);
});


// ========================================
// 5. RabbitMQ Configuration
// ========================================

builder.Services.Configure<RabbitMQOptions>(
    configuration.GetSection("RabbitMQ"));


// ========================================
// 6. RabbitMQ Publisher
// ========================================

builder.Services.AddSingleton<
    IRabbitMQPublisher,
    RabbitMQPublisher>();


// ========================================
// 7. RabbitMQ Consumer
// ========================================

builder.Services.AddSingleton<
    IRabbitMQConsumer,
    RabbitMQConsumer>();


// ========================================
// 8. Shipment Message Handlers
// ========================================

// Create Shipment
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    CreateShipmentMessageHandler>();

// Update Shipment
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    UpdateShipmentMessageHandler>();

// Delete Shipment
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    DeleteShipmentMessageHandler>();


// ========================================
// 9. ShipmentEvent Message Handlers
// ========================================

// Create ShipmentEvent
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    CreateShipmentEventMessageHandler>();

// Update ShipmentEvent
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    UpdateShipmentEventMessageHandler>();

// Delete ShipmentEvent
builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    DeleteShipmentEventMessageHandler>();


// ========================================
// 10. JWT Service
// ========================================

builder.Services.AddScoped<
    IJwtService,
    JwtService>();


// ========================================
// 11. Redis
// ========================================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        configuration["Redis:ConnectionString"];
});

builder.Services.AddScoped<
    ICache,
    Cache>();


// ========================================
// 12. Background Worker
// ========================================

builder.Services.AddHostedService<Worker>();


// ========================================
// 13. Build Host
// ========================================

var host = builder.Build();


// ========================================
// 14. Run Worker
// ========================================

host.Run();