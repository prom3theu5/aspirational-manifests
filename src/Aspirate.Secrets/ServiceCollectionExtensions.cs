using Aspirate.Shared.Enums;
using Aspirate.Shared.Interfaces.Secrets;
using Aspirate.Shared.Interfaces.Services;

namespace Aspirate.Secrets;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecretProtectionStrategies(this IServiceCollection services) =>
        services
            .AddSingleton<ISecretProtectionStrategy, ConnectionStringProtector>()
            .AddSingleton<ISecretProtectionStrategy, PostgresPasswordProtector>()
            .AddSingleton<ISecretProtectionStrategy, MsSqlPasswordProtector>();

    public static IServiceCollection RegisterAspirateSecretProvider(this IServiceCollection services, SecretProviderType providerType) =>
        providerType switch
        {
            SecretProviderType.Base64 => services.RegisterSecretProvider<Base64SecretProvider>(),
            SecretProviderType.Password => services.RegisterSecretProvider<PasswordSecretProvider>(),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
        };

    private static IServiceCollection RegisterSecretProvider<TImplementation>(this IServiceCollection services)
        where TImplementation : class, ISecretProvider =>
            services.AddSingleton<ISecretProvider, TImplementation>();
}
