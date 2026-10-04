using AutoMapper;
using FluentValidation;
using BikeConsulting.Api.Mapper;
using BikeConsulting.Api.Validation.User;
using BikeConsulting.Business.Contract;
using BikeConsulting.Business.Implementation;
using BikeConsulting.Data.Contract;
using BikeConsulting.Data.Implementation;
using BikeConsulting.Entities.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;


// ============================================================
// SERILOG
// ============================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .Enrich.FromLogContext()
    .WriteTo.File(
        "logs/errors.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();


// ============================================================
// CREATE APPLICATION BUILDER
// ============================================================

var builder = WebApplication.CreateBuilder(args);


// Use Serilog as the ASP.NET Core logging provider
builder.Host.UseSerilog();


// ============================================================
// CONTROLLERS
// ============================================================

builder.Services.AddControllers();


// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// ============================================================
// JWT AUTHENTICATION
// ============================================================

var jwtSecret = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException(
        "JWT Key is missing from configuration.");
}

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,

        IssuerSigningKey =
            new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Error(
                context.Exception,
                "JWT token validation failed.");

            return Task.CompletedTask;
        }
    };
});


// ============================================================
// AUTHORIZATION
// ============================================================

builder.Services.AddAuthorization();


// ============================================================
// DATABASE / ENTITY FRAMEWORK CORE
// ============================================================

builder.Services.AddDbContext<BikeConsultingContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BikeConsultingContext")));


// ============================================================
// FLUENT VALIDATION
// ============================================================

// Automatically discovers all validators in the assembly
builder.Services.AddValidatorsFromAssemblyContaining<UserModelValidator>();


// ============================================================
// BUSINESS LAYER
// ============================================================

builder.Services.AddScoped<IUserComponent, UserComponent>();


// ============================================================
// DATA / REPOSITORY LAYER
// ============================================================

builder.Services.AddScoped<IUserRepository, UserRepository>();


// ============================================================
// AUTOMAPPER
// ============================================================

var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
}, NullLoggerFactory.Instance);

IMapper mapper = mapperConfiguration.CreateMapper();

builder.Services.AddSingleton(mapper);


// ============================================================
// SWAGGER
// ============================================================

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "",
        Description = ""
    });

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,

            // HTTP Bearer is the correct semantic type for JWT
            Type = SecuritySchemeType.Http,

            Scheme = "bearer",
            BearerFormat = "JWT",

            Description =
                "Enter: Bearer {your JWT token}"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});


// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();


// ============================================================
// HTTP REQUEST PIPELINE
// ============================================================

// Swagger
app.UseSwagger();
app.UseSwaggerUI();


// CORS
app.UseCors("AllowAll");


// HTTPS
app.UseHttpsRedirection();


// Authentication
app.UseAuthentication();


// Authorization
app.UseAuthorization();


// Controller routing
app.MapControllers();


// ============================================================
// START APPLICATION
// ============================================================

app.Run();