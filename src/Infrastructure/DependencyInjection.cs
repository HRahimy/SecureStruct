using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using SecureStruct.Application.Common.Interfaces;
using SecureStruct.Domain.Constants;
using SecureStruct.Infrastructure.Data;
using SecureStruct.Infrastructure.Data.Interceptors;
using SecureStruct.Infrastructure.Identity;
using SecureStruct.Infrastructure.Identity.Authentication;
using SecureStruct.Infrastructure.Identity.Authentication.Client;
using SecureStruct.Infrastructure.Identity.Authentication.Flow;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(
            connectionString,
            message: "Connection string 'DefaultConnection' not found."
        );

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                options.UseNpgsql(connectionString);
            }
        );

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>()
        );

        services.AddScoped<ApplicationDbContextInitialiser>();

        var keycloakConfig = configuration.GetSection("Keycloak")!;
        var client = AuthenticatedHttpClientFactory.Create(new PasswordGrantFlow
        {
            UserName = keycloakConfig["UserName"]!,
            Password = keycloakConfig["Password"]!,
            KeycloakUrl = keycloakConfig["KeycloakUrl"]!
        });
        services.AddSingleton(client);
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddSingleton(TimeProvider.System);

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator))
        );

        return services;
    }
}
