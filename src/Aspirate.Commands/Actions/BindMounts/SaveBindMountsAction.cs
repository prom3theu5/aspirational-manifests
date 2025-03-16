namespace Aspirate.Commands.Actions.BindMounts;
public sealed class SaveBindMountsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Handling minikube mounts[/]");

        if (CurrentState.DisableMinikubeMountAction.Equals(false) || !CurrentState.DisableMinikubeMountAction.HasValue)
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

        return Task.FromResult(true);
    }
}
