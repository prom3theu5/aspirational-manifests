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
        IFileSystem? fileSystem = null,
        ISecretProvider? secretProvider = null)
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

        if (secretProvider is not null)
        {
            services.AddSingleton<ISecretProvider>(secretProvider);
        }

        return services.BuildServiceProvider();
    }

    protected AspirateState CreateAspirateStateWithInputs(bool nonInteractive = false, bool generatedInputs = false, bool passwordsSet = false)
    {
        var postgres = CreatePostgresContainerResourceManualInput("postgrescontainer", generatedInputs, passwordsSet);
        var postgresTwo = CreatePostgresContainerResourceManualInput("postgrescontainer2", generatedInputs, passwordsSet);

        var resources = new Dictionary<string, Resource>
        {
            { "postgrescontainer", postgres },
            { "postgrescontainer2", postgresTwo },
        };

        var state = CreateAspirateState(nonInteractive: nonInteractive);
        state.LoadedAspireManifestResources = resources;
        state.AspireComponentsToProcess = resources.Keys.ToList();

        return state;
    }

    private static Container CreatePostgresContainerResourceManualInput(string resourceName, bool generatedInput = false, bool passwordsSet = false)
    {
        var postgres = new Container
        {
            Name = resourceName,
            Type = AspireLiterals.Container,
            Image = "postgres:latest",
            ConnectionString = $"Host={{{resourceName}.bindings.tcp.host}};Port={{{resourceName}.bindings.tcp.port}};Username=postgres;Password={{{resourceName}.inputs.password}}",
            Bindings = new()
            {
                {
                    "tcp", new()
                    {
                        Scheme = "tcp",
                        Protocol = "tcp",
                        Transport = "tcp",
                        ContainerPort = 5432,
                    }
                },
            },
            Inputs = CreateInputs(generatedInput, passwordsSet),
        };
        return postgres;
    }

    private static Dictionary<string, Input> CreateInputs(bool generated = false, bool passwordsSet = false)
    {
        var inputs = new Dictionary<string, Input>();

        if (generated)
        {
            inputs.Add("password", new()
            {
                Type = "string",
                Default = new()
                {
                    Generate = new()
                    {
                        MinLength = 20,
                    },
                },
            });
        }
        else
        {
            inputs.Add("password", new()
            {
                Type = "string",
            });
        }

        if (passwordsSet)
        {
            inputs["password"].Value = "some-password";
        }

        return inputs;
    }

    protected TSystemUnderTest? GetSystemUnderTest(IServiceProvider serviceProvider) =>
        serviceProvider.GetRequiredKeyedService<IAction>(typeof(TSystemUnderTest).Name) as TSystemUnderTest;
}
