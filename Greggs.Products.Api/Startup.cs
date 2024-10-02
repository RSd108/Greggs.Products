using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Greggs.Products.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Register the data access service
        services.AddScoped<IDataAccess<Product>, ProductAccess>();
        services.AddScoped<IDataAccess<ExchangeRate>, ExchangeRateAccess>();

        // Add API Versioning
        services.AddApiVersioning( options =>
        {
            options.ReportApiVersions = true; // Optional, to show API versions in the response headers
            options.AssumeDefaultVersionWhenUnspecified = true; // Use default version when none is specified
            options.DefaultApiVersion = new ApiVersion( 2, 0 ); // Set the default version
        } );

        // Add Versioned API Explorer (for Swagger support)
        services.AddVersionedApiExplorer( options =>
        {
            options.GroupNameFormat = "'v'VVV"; // API version format: v1, v1.1, etc.
            options.SubstituteApiVersionInUrl = true;
        } );

        services.AddSwaggerGen( options =>
        {
            // Get all API version descriptions from ApiExplorer and create a Swagger document for each
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            foreach ( var description in provider.ApiVersionDescriptions )
            {
                options.SwaggerDoc( description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = $"Greggs Products API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString()
                } );
            }
        } );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider )
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        // Get all API version descriptions from ApiExplorer and create a Swagger document for each
        app.UseSwaggerUI( options =>
        {
            // Create a Swagger UI endpoint for each API version
            foreach ( var description in provider.ApiVersionDescriptions )
            {
                options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
            }
        } );

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers();});
    }
}