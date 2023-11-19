
namespace Aspirate.Spectre.DependencyInjection;

internal class DependencyInjectionResolver : ITypeResolver, IDisposable
{
    private ServiceProvider ServiceProvider { get; }

    internal DependencyInjectionResolver(ServiceProvider serviceProvider) =>
        ServiceProvider = serviceProvider;


    public void Dispose() =>
        ServiceProvider.Dispose();

    public object Resolve(Type? type) =>
        ServiceProvider.GetService(type) ?? Activator.CreateInstance(type);
}
