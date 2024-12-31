using FinanceManager.Application;
using FinanceManager.Application.Mapper;
using FinanceManager.Configuration;
using FinanceManager.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Authentication setup
// OAuth 2.0 JWT Token Validation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Audience = builder.Configuration["OAuth:Audience"] ?? throw new ArgumentNullException("Audience is null");
                    options.Authority = builder.Configuration["OAuth:Authority"] ?? throw new ArgumentNullException("Issuer is null");
                });
// ApiKey authentication scheme
builder.Services.AddAuthenticationSchemes();
builder.Services.AddAuthorization();

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration["SQLDbConnectionString"]
    ?? throw new ArgumentNullException("SQL Connection string missing")));

// Configure Dependency Injection
builder.Services.AddObjectMappers();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
