namespace Aspirate.Tests.ActionsTests;

public abstract class BaseActionTests<TSystemUnderTest> where TSystemUnderTest : class, IAction
{
    protected const string DefaultProjectPath = "/some-path";
    protected const string DefaultContainerRegistry = "test-registry";
    protected const string DefaultContainerImageTag = "test-tag";
    protected const string DefaultTemplatePath = "/templates";

    protected static AspirateState CreateAspirateState(
        bool nonInteractive = false,
        string? projectPath = DefaultProjectPath,
        string? containerRegistry = DefaultContainerRegistry,
        string? containerImageTag = DefaultContainerImageTag,
        string? templatePath = DefaultTemplatePath,
        string? aspireManifest = null,
        string? inputPath = null,
        string? kubeContext = null)
    {
        var state = new AspirateState
        {
            NonInteractive = nonInteractive,
            ContainerRegistry = containerRegistry,
            ContainerImageTag = containerImageTag,
            TemplatePath = templatePath,
            InputPath = inputPath,
            KubeContext = kubeContext,
        };

        if (!string.IsNullOrEmpty(projectPath))
        {
            state.ProjectPath = projectPath;
        }

        if (!string.IsNullOrEmpty(aspireManifest))
        {
            state.AspireManifest = aspireManifest;
        }

        return state;
    }

    protected static IServiceProvider CreateServiceProvider(
        AspirateState state,
        TestConsole? testConsole = null,
        IFileSystem? fileSystem = null)
    {
        var console = testConsole ?? new TestConsole();
        fileSystem ??= Substitute.For<IFileSystem>();

        var services = new ServiceCollection();
        services.RegisterAspirateEssential();

        services.RemoveAll<IFileSystem>();
        services.RemoveAll<IShellExecutionService>();
        services.RemoveAll<IAnsiConsole>();
        services.RemoveAll<AspirateState>();

        services.AddSingleton<IFileSystem>(fileSystem);
        services.AddSingleton<IAnsiConsole>(console);
        services.AddSingleton(state);
        services.AddSingleton(Substitute.For<IShellExecutionService>());

        return services.BuildServiceProvider();
    }

    protected TSystemUnderTest? GetSystemUnderTest(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredKeyedService<IAction>(typeof(TSystemUnderTest).Name) as TSystemUnderTest;
}
