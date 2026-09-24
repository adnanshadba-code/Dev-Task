using DevTeam.Services.Identity.Application.Commands.Auth;
using DevTeam.Services.Identity.Domain.Entities;
using DevTeam.Services.Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharedKernel.Helpers;
using SharedKernel.Interfaces;
using SharedKernel.Repositories;
using SharedKernel.Services;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// =============================
// Controllers
// =============================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


// =============================
// Authentication - JWT
// =============================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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


// =============================
// Authorization
// =============================

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
// =============================
// MediatR
// =============================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(LoginCommand).Assembly);
});


// =============================
// Swagger
// =============================

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


// =============================
// Database
// =============================

builder.Services.AddDbContext<IdentityAppDbContext>(
    options =>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// =============================
// Unit Of Work
// =============================

builder.Services.AddScoped<IUnitOfWork<IdentityAppDbContext>,UnitOfWork<IdentityAppDbContext>>();


// =============================
// Repository
// =============================

builder.Services.AddScoped<
    IRepository<User>,
    Repository<User, IdentityAppDbContext>>();

builder.Services.AddScoped<IRepository<Permission>,Repository<Permission, IdentityAppDbContext>>();

builder.Services.AddScoped<IRepository<UserPermission>
    ,Repository<UserPermission, IdentityAppDbContext>>();


// =============================
// Password Hasher
// =============================

builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();


// =============================
// Redis
// =============================

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});


// =============================
// Cache
// =============================

builder.Services.AddScoped<ICache, Cache>();

builder.Services.AddScoped<IJwtService, JwtService>();
// =============================
// Build Application
// =============================

var app = builder.Build();


// =============================
// Middleware
// =============================

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