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
        var values = new Dictionary<string, List<BindMount>>();
        foreach (var resource in CurrentState.AllSelectedSupportedComponents)
        {
            var resourceWithBindMounts = resource.Value as IResourceWithBindMounts;

            if (resourceWithBindMounts?.BindMounts.Count > 0)
            {
                values.Add(resourceWithBindMounts.Name, resourceWithBindMounts.BindMounts);
            }
        }

        if (values.Count > 0)
        {
            CurrentState.BindMounts = values;
        }

        return Task.FromResult(true);
    }
}
