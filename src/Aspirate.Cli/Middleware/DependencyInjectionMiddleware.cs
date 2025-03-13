namespace Aspirate.Cli.Middleware;

internal static class DependencyInjectionMiddleware
{
    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<ServiceCollection> configureServices) =>
        UseDependencyInjection(builder, (_, services) => configureServices(services));

    private static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices) =>
        builder.AddMiddleware((context, next) =>
        {
            var services = new ServiceCollection();
            configureServices(context, services);

            services.TryAddSingleton(context.Console);

            context.BindingContext.AddService<IServiceCollection>(_ => services);

            return next(context);
        });
}
