using Microsoft.OpenApi.Models;
using QuizApp.API.Filters;
using System.Reflection;

namespace QuizApp.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Controllers
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        // Swagger/OpenAPI - Always add in all environments for debugging
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "QuizApp API",
                Version = "v1",
                Description = "A comprehensive Quiz Application API built with Clean Architecture",
                Contact = new OpenApiContact
                {
                    Name = "Quiz App Team",
                    Email = "support@quizapp.com"
                }
            });

            // Add security definition for JWT
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
}