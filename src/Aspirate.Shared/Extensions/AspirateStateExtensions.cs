namespace Aspirate.Shared.Extensions;

/// Provides extension methods for populating an AspirateState object from command options.
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
