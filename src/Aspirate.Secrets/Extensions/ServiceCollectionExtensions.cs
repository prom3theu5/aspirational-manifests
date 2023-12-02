namespace Aspirate.Secrets.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAspirateSecretProviders(this IServiceCollection services) =>
        services
            .RegisterSecretProviders<Base64SecretProvider>(AspirateSecretLiterals.Base64SecretsManager)
            .RegisterSecretProviders<PasswordSecretProvider>(AspirateSecretLiterals.PasswordSecretsManager);

    private static IServiceCollection RegisterSecretProviders<TImplementation>(this IServiceCollection services, string key)
        where TImplementation : class, ISecretProvider =>
            services.AddKeyedSingleton<ISecretProvider, TImplementation>(key);
}
