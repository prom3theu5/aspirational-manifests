namespace Aspirate.Commands.Options;

public abstract class BaseOption<T>(
    string[] aliases,
    string envName,
    T defaultValue) : Option<T>(aliases,
    getDefaultValue: GetOptionDefault(envName,
        defaultValue))
{
    public abstract bool IsSecret { get; }

    public T GetOptionDefault() => GetOptionDefault(envName, defaultValue)();

    private static Func<TReturnValue> GetOptionDefault<TReturnValue>(string envVarName, TReturnValue defaultValue) =>
        () =>
        {
            var envValue = Environment.GetEnvironmentVariable(envVarName);
            if (envValue == null)
            {
                return defaultValue;
            }

            try
            {
                return (TReturnValue) Convert.ChangeType(envValue, typeof(TReturnValue));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        };
}
