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

        // Ensure we have a default output format.
        if (string.IsNullOrEmpty(state.OutputFormat))
        {
            state.OutputFormat = OutputFormat.Kustomize.Value;
        }
    }

    public static void ReplaceCurrentStateWithPreviousState(this AspirateState currentState, AspirateState previousState, bool restoreAllRestorable)
    {
        ArgumentNullException.ThrowIfNull(currentState);
        ArgumentNullException.ThrowIfNull(previousState);

        if (restoreAllRestorable)
        {
            var properties = typeof(AspirateState).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => Attribute.IsDefined(p, typeof(RestorableStatePropertyAttribute)));

            foreach (var property in properties)
            {
                var previousStatePropertyValue = property.GetValue(previousState);
                if (previousStatePropertyValue is null)
                {
                    continue;
                }

                property.SetValue(currentState, previousStatePropertyValue);
            }

            currentState.StateWasLoadedFromPrevious = true;
            return;
        }

        currentState.SecretState = previousState.SecretState;
    }
}
