using System.Reflection.Metadata.Ecma335;
using Aspirate.Shared.Models.AspireManifests.Components.Common;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Aspirate.Commands.Actions.BindMounts;
public sealed class SaveBindMountsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        if (CurrentState.DisableMinikubeMountAction.Equals(false) || !CurrentState.DisableMinikubeMountAction.HasValue)
        {
            var values = new Dictionary<string, Dictionary<string, int?>>();
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
                        values[bindMount.Source].TryAdd(bindMount.Target, null);
                    }

                    //values.Add(resourceWithBindMounts.Name, resourceWithBindMounts.BindMounts);
                }
            }

            if (values.Count > 0)
            {
                CurrentState.BindMounts = values;
            }
        }

        return Task.FromResult(true);
    }
}
