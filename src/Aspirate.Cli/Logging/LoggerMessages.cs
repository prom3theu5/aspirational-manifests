namespace Aspirate.Cli.Logging;

public static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Executing '{MethodName}' in service '{ServiceName}'")]
    public static partial void LogExecuteService(this ILogger logger, string methodName, string serviceName);
}
