namespace Aspirate.Secrets.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecretProtectionStrategies(this IServiceCollection services) =>
        services
            .AddSingleton<ISecretProtectionStrategy, ConnectionStringProtector>()
            .AddSingleton<ISecretProtectionStrategy, PostgresPasswordProtector>()
            .AddSingleton<ISecretProtectionStrategy, MsSqlPasswordProtector>();

    public static IServiceCollection RegisterAspirateSecretProvider(this IServiceCollection services, ProviderType providerType) =>
        providerType switch
        {
            ProviderType.Base64 => services.RegisterSecretProvider<Base64SecretProvider>(),
            ProviderType.Password => services.RegisterSecretProvider<PasswordSecretProvider>(),
            _ => throw new ArgumentOutOfRangeException(nameof(providerType), providerType, null)
        };

    private static IServiceCollection RegisterSecretProvider<TImplementation>(this IServiceCollection services)
        where TImplementation : class, ISecretProvider =>
            services.AddSingleton<ISecretProvider, TImplementation>();
}
