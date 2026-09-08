using Microsoft.OpenApi.Models;

namespace EventLensAI.API.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddEventLensSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(SchemaId);
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EventLens AI API",
                Version = "v1",
                Description = "Production API foundation for the EventLens AI multi-tenant event platform."
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Enter a JWT access token."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                }] = []
            });
        });
        return services;
    }

    private static string SchemaId(Type type)
    {
        if (type.IsArray) return $"{SchemaId(type.GetElementType()!)}Array";
        if (!type.IsGenericType) return (type.FullName ?? type.Name).Replace('+', '.');

        var definition = (type.GetGenericTypeDefinition().FullName ?? type.Name)
            .Split('`')[0]
            .Replace('+', '.');
        return $"{definition}_{string.Join("_", type.GetGenericArguments().Select(SchemaId))}";
    }
}
