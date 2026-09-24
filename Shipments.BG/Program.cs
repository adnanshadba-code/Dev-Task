using DevTeam.Application.Messaging;
using DevTeam.Application.Messaging.ShipmentEventMessage;
using DevTeam.Application.Messaging.ShipmentMessages;
using DevTeam.Services.Identity.Domain.Entities;
using DevTeam.Services.Shipments.Applications.Commands.Shipments;
using DevTeam.Services.Shipments.Applications.Messaging.Publisher;
using DevTeam.Services.Shipments.Applications.Messaging.Queues;
using Microsoft.AspNetCore.Identity;
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
// RabbitMQ Configuration
// ========================================

//builder.Services.Configure<RabbitMQOptions>(
//    configuration.GetSection("RabbitMQ"));


// ========================================
// MediatR
// ========================================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(CreateShipmentCommand).Assembly);
});


// ========================================
// Password Hasher
// ========================================

builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();


// ========================================
// Database
// ========================================

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseNpgsql(
//        configuration.GetConnectionString(
//            "DefaultConnection"));
//});


// ========================================
// Repository
// ========================================

//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<,>));

// ========================================
// Unit OfWork
// ========================================

//builder.Services.AddScoped<
//    IUnitOfWork,
//    UnitOfWork>();


// ========================================
// RabbitMQ Publisher
// ========================================

//builder.Services.AddSingleton<
//    IRabbitMQPublisher,
//    DevTeam.Application.Messaging.RabbitMQPublisher>();


// ========================================
// RabbitMQ Consumer
// ========================================

builder.Services.AddSingleton<
    IRabbitMQConsumer,
    RabbitMQConsumer>();


// ========================================
// Shipment Message Handlers
// ========================================

// Create Shipment

builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    DevTeam.Application.Messaging.ShipmentMessages.CreateShipmentMessageHandler>();


// Update Shipment

builder.Services.AddScoped<
    IRabbitMQMessageHandler,
    DevTeam.Application.Messaging.ShipmentMessages.UpdateShipmentMessageHandler>();


// Delete Shipment

builder.Services.AddScoped<IRabbitMQMessageHandler, DeleteShipmentMessageHandler>();

builder.Services.Configure<RabbitMQOptions>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

builder.Services.AddSingleton<
    IRabbitMQConsumer,
    RabbitMQConsumer>();

builder.Services.AddHostedService<Worker>();


// ========================================
// ShipmentEvent Message Handlers
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
// JWT Service
// ========================================

builder.Services.AddScoped<
    IJwtService,
    JwtService>();

builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
// ========================================
// Background Worker
// ========================================

builder.Services.AddHostedService<Worker>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});


builder.Services.AddScoped<ICache, Cache>();
// ========================================
// Build Host
// ========================================

var host = builder.Build();


// ========================================
// Run Worker
// ========================================

host.Run();
