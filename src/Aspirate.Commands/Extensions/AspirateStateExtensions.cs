namespace Aspirate.Commands.Extensions;

/// Provides extension methods for populating an AspirateState object from command options.
public static class AspirateStateExtensions
{
    /// <summary>
    /// Populates the properties of the given <see cref="AspirateState"/> object from the properties of the specified options object.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options object. It must implement <see cref="ICommandOptions"/>.</typeparam>
    /// <param name="state">The <see cref="AspirateState"/> object to populate.</param>
    /// <param name="options">The options object containing the properties to populate the <paramref name="state"/> object with.</param>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="state"/> or <paramref name="options"/> is null.</exception>
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
