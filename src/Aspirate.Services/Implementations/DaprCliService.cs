namespace Aspirate.Services.Implementations;

public class DaprCliService(IShellExecutionService shellExecutionService, IAnsiConsole console) : IDaprCliService
{
    private string _daprPath = "dapr";

    public bool IsDaprCliInstalledOnMachine()
    {
        var result = shellExecutionService.IsCommandAvailable("dapr");

        if (!result.IsAvailable)
        {
            return false;
        }

        _daprPath = result.FullPath;
        return true;
    }

    public async Task<bool> IsDaprInstalledInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("status", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await shellExecutionService.ExecuteCommandWithEnvironmentNoOutput(_daprPath, argumentsBuilder, new Dictionary<string, string?>());
    }

    public async Task<ShellCommandResult> InstallDaprInCluster()
    {
        var argumentsBuilder = ArgumentsBuilder
            .Create()
            .AppendArgument("init", string.Empty, quoteValue: false)
            .AppendArgument("-k", string.Empty, quoteValue: false);

        return await shellExecutionService.ExecuteCommand(new()
        {
            Command = _daprPath,
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
            Command = _daprPath,
            ArgumentsBuilder = argumentsBuilder,
            ShowOutput = true,
        });
    }
}
