using Aspirate.Shared.Commands;

namespace Aspirate.Shared.Extensions;

public static class AspirateStateExtensions
{
    public static void PopulateStateFromOptions<TOptions>(this AspirateState state, TOptions options)
        where TOptions : ICommandOptions
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(options);

        var properties = typeof(TOptions).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(options);
            if (propertyValue is null)
            {
                continue;
            }

            var stateProperty = state.GetType().GetProperty(property.Name);

            stateProperty?.SetValue(state, propertyValue);
        }
    }
}
