namespace Aspirate.Commands.Actions.BindMounts;
public sealed class SaveBindMountsAction(
    IServiceProvider serviceProvider,
    IFileSystem fileSystem) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");

        if (CurrentState.DisableMinikubeMountAction.Equals(true))
        {
            Logger.WriteLine("Minikube bindmounts actions disabled. Not saving to state.");
            return Task.FromResult(true);
        }

        if (!CurrentState.ActiveKubernetesContextIsSet)
        {
            Logger.WriteLine("KubeContext has not been set. Not saving to state. In order to use minikube bind mounts, KubeContext must be minikube.");
            return Task.FromResult(true);
        }

        if (!CurrentState.KubeContext.Equals(MinikubeLiterals.Path, StringComparison.OrdinalIgnoreCase))
        {
            Logger.WriteLine("KubeContext was not minikube. Not saving to state. In order to use minikube bind mounts, KubeContext must be minikube.");
            return Task.FromResult(true);
        }

        var values = new Dictionary<string, Dictionary<string, int>>();
        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            var resourceWithBindMounts = resource.Value as IResourceWithBindMounts;

            if (resourceWithBindMounts?.BindMounts.Count > 0)
            {
                foreach (var bindMount in resourceWithBindMounts.BindMounts)
                {
                    bindMount.Source = fileSystem.GetFullPath(bindMount.Source);
                    if (!values.ContainsKey(bindMount.Source))
                    {
                        values[bindMount.Source] = [];
                    }
                    values[bindMount.Source].TryAdd(bindMount.Target, 0);
                }
            }
        }

        if (values.Count > 0)
        {
            CurrentState.BindMounts = values;
        }
        else
        {
            Logger.WriteLine("No bindmounts to save.");
        }

        return Task.FromResult(true);
    }
}
