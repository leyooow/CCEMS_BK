using Application.Contracts.Services;
using Application.MappingProfile;
using Application.Services;
using Infrastructure.DependencyInjection;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger => {

    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "A SP.NET 8 Web API",
        Description = ""
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
    });
});

// Register DbContext with dependency injection
builder.Services.AddDbContext<CcemQatContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register DbContext with dependency injection
builder.Services.AddDbContext<SitcbsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Finacle")));


// Add AutoMapper with mapping profiles.
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.InfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("DefaultPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors(policy =>
//    policy.WithOrigins("http://localhost:5173")
//          .AllowAnyMethod()
//          .AllowAnyHeader());



// Add authentication middleware
app.UseAuthentication(); // Add this line

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
