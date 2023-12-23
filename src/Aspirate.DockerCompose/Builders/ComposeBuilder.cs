namespace Aspirate.DockerCompose.Builders;

public class ComposeBuilder : BaseBuilder<ComposeBuilder, Compose>
{
    public ComposeBuilder WithVersion(string version)
    {
        WorkingObject.Version = version;
        return this;
    }

    /// <summary>
    /// Add services to the Compose object
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public ComposeBuilder WithServices(params Service[] services) => WithT(
        () => WorkingObject.Services,
        x => WorkingObject.Services = x,
        services
    );

    /// <summary>
    /// Add networks to the compose object
    /// </summary>
    /// <param name="networks"></param>
    /// <returns></returns>
    public ComposeBuilder WithNetworks(params Network[] networks) => WithT(
        () => WorkingObject.Networks,
        x => WorkingObject.Networks = x,
        networks
    );

    /// <summary>
    /// Add volumes to the compose object
    /// </summary>
    /// <param name="volumes"></param>
    /// <returns></returns>
    public ComposeBuilder WithVolumes(params Volume[] volumes) => WithT(
        () => WorkingObject.Volumes,
        x => WorkingObject.Volumes = x,
        volumes
    );

    /// <summary>
    /// Add secrets to the compose object
    /// </summary>
    /// <param name="secrets"></param>
    /// <returns></returns>
    public ComposeBuilder WithSecrets(params Secret[] secrets) => WithT(
        () => WorkingObject.Secrets,
        x => WorkingObject.Secrets = x,
        secrets
    );

    /// <summary>
    /// Add services to the Compose object
    /// </summary>
    /// <returns></returns>
    private ComposeBuilder WithT<T>(
        Func<IDictionary<string, T>?> getCollection,
        Action<IDictionary<string, T>> setCollection,
        params T[] parameters
    ) where T : IObject
    {
        var collection = getCollection();

        if (collection == null)
        {
            collection = new Dictionary<string, T>();
            setCollection(collection);
        }

        foreach (var parameter in parameters)
        {
            if (collection.ContainsKey(parameter.Name))
            {
                throw new($"{typeof(T).Name} name ('{parameter.Name}') already added to the target collection, please pick a unique one!");
            }

            collection.Add(parameter.Name, parameter);
        }

        return this;
    }
}
