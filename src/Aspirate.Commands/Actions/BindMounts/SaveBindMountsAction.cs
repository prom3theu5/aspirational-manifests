namespace Aspirate.Commands.Actions.BindMounts;
public sealed class SaveBindMountsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");

        if (!CurrentState.DisableMinikubeMountAction.Equals(true))
        {
            var values = new Dictionary<string, Dictionary<string, int>>();
            foreach (var resource in CurrentState.AllSelectedSupportedComponents)
            {
                var resourceWithBindMounts = resource.Value as IResourceWithBindMounts;

                if (resourceWithBindMounts?.BindMounts.Count > 0)
                {
                    foreach (var bindMount in resourceWithBindMounts.BindMounts)
                    {
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
        }
        else
        {
            Logger.WriteLine("Minikube bindmounts actions disabled. Not saving in state.");
        }

        if (CurrentState.KubeContext == null || !CurrentState.KubeContext.Equals("minikube", StringComparison.OrdinalIgnoreCase))
        {
            Logger.WriteLine("Minikube was not set as the kubecontext to use. Will not add default minikube mount prefix to volumes hostpath in deployment files.");
        }


        return Task.FromResult(true);
    }
}
