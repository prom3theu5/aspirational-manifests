namespace Aspirate.Services.Implementations;

public class DaprCliService(IShellExecutionService shellExecutionService, IAnsiConsole console) : IDaprCliService
{
    public Task<bool> IsDaprCliInstalledOnMachine() =>
        shellExecutionService.IsCommandAvailable("dapr");

    public async Task<bool> IsDaprInstalledInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("status", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await shellExecutionService.ExecuteCommandWithEnvironmentNoOutput("dapr", argumentsBuilder, new Dictionary<string, string?>());
    }

    public async Task<ShellCommandResult> InstallDaprInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("init", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await shellExecutionService.ExecuteCommand(new()
        {
            Command = "dapr",
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true,
        });
    }

    public async Task<ShellCommandResult> RemoveDaprFromCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("uninstall", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await shellExecutionService.ExecuteCommand(new()
        {
            Command = "dapr",
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true,
        });
    }
}
