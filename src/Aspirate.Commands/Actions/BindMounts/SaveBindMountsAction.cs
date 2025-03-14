using Aspirate.Shared.Models.AspireManifests.Components.Common;
using Aspirate.Shared.Models.AspireManifests.Components.Common.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Aspirate.Commands.Actions.BindMounts;
public sealed class SaveBindMountsAction(
    IServiceProvider serviceProvider) : BaseAction(serviceProvider)
{
    public override Task<bool> ExecuteAsync()
    {
        var values = new Dictionary<string, List<BindMount>>();
        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            var container = resource.Value as ContainerResourceBase;

            if (container.BindMounts.Count > 0)
            {
                values.Add(resource.Value.Name, container.BindMounts);
            }
        }
        CurrentState.BindMounts = values;

        return Task.FromResult(true);
    }
}
