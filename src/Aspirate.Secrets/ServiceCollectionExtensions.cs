namespace Aspirate.Secrets;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecretProtectionStrategies(this IServiceCollection services) =>
        services
            .AddSingleton<ISecretProtectionStrategy, ConnectionStringProtector>()
            .AddSingleton<ISecretProtectionStrategy, PostgresPasswordProtector>()
            .AddSingleton<ISecretProtectionStrategy, MsSqlPasswordProtector>();

    public static IServiceCollection AddAspirateSecretProvider(this IServiceCollection services) =>
        services.AddSingleton<ISecretProvider, SecretProvider>();
}
