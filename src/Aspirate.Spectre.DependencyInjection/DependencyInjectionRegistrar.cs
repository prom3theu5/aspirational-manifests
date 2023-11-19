namespace Aspirate.Spectre.DependencyInjection;

public class DependencyInjectionRegistrar(IServiceCollection services) : ITypeRegistrar, IDisposable
{
    private IList<IDisposable> BuiltProviders { get; } = new List<IDisposable>();

    public ITypeResolver Build()
    {
        var buildServiceProvider = services.BuildServiceProvider();
        BuiltProviders.Add(buildServiceProvider);
        return new DependencyInjectionResolver(buildServiceProvider);
    }

    public void Register(Type service, Type implementation) => services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) => services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory) => services.AddSingleton(service, _ => factory());

    public void Dispose()
    {
        foreach (var provider in BuiltProviders)
        {
            provider.Dispose();
        }
    }
}
