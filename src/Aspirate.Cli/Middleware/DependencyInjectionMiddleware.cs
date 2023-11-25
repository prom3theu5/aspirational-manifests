namespace Aspirate.Cli.Middleware;

internal static class DependencyInjectionMiddleware
{
    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<ServiceCollection> configureServices) =>
        UseDependencyInjection(builder, (_, services) => configureServices(services));

    private static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices) =>
        builder.AddMiddleware(async (context, next) =>
        {
            var services = new ServiceCollection();
            configureServices(context, services);
            var uniqueServiceTypes = new HashSet<Type>(services.Select(x => x.ServiceType));

            services.TryAddSingleton(context.Console);

            await using var serviceProvider = services.BuildServiceProvider();

            context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);

            foreach (var serviceType in uniqueServiceTypes)
            {
                context.BindingContext.AddService(serviceType, _ => serviceProvider.GetRequiredService(serviceType));

                // Enable support for "context.BindingContext.GetServices<>()" as in the modern dependency injection
                var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                context.BindingContext.AddService(enumerableServiceType, _ => serviceProvider.GetServices(serviceType));
            }

            await next(context);
        });
}
