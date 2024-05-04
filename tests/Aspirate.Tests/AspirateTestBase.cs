namespace Aspirate.Tests;

public abstract class AspirateTestBase
{
    protected const string DefaultProjectPath = "/some-path";
    protected const string DefaultContainerRegistry = "test-registry";
    protected const string DefaultContainerPrefix = "test-prefix";
    protected const string DefaultContainerBuilder = "docker";
    protected const string DefaultContainerImageTag = "test-tag";
    protected const string DefaultTemplatePath = "/templates";
    protected const string DefaultOutputFormat = "kustomize";

    protected static AspirateState CreateAspirateState(bool nonInteractive = false,
        string? projectPath = DefaultProjectPath,
        string? containerRegistry = DefaultContainerRegistry,
        string? containerPrefix = null,
        string? containerBuilder = null,
        string? containerImageTag = DefaultContainerImageTag,
        string? templatePath = DefaultTemplatePath,
        string? aspireManifest = null,
        string? inputPath = null,
        string? kubeContext = null,
        string? password = null,
        string? outputFormat = DefaultOutputFormat)
    {
        var state = new AspirateState
        {
            NonInteractive = nonInteractive,
            ContainerRegistry = containerRegistry,
            ContainerImageTag = containerImageTag,
            ContainerBuilder = containerBuilder,
            ContainerRepositoryPrefix = containerPrefix,
            TemplatePath = templatePath,
            InputPath = inputPath,
            KubeContext = kubeContext,
            SecretPassword = password,
            OutputFormat = outputFormat,
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
        secretProvider ??= new Base64SecretProvider(fileSystem);

        var services = new ServiceCollection();
        services.RegisterAspirateEssential();

        services.RemoveAll<IFileSystem>();
        services.RemoveAll<IShellExecutionService>();
        services.RemoveAll<IAnsiConsole>();
        services.RemoveAll<AspirateState>();

        services.AddSingleton<IFileSystem>(fileSystem);
        services.AddSingleton<ISecretProvider>(secretProvider);
        services.AddSingleton<IAnsiConsole>(console);
        services.AddSingleton(state);
        services.AddSingleton(Substitute.For<IShellExecutionService>());

        return services.BuildServiceProvider();
    }

    protected AspirateState CreateAspirateStateWithInputs(bool nonInteractive = false, bool generatedInputs = false, bool passwordsSet = false)
    {
        var (postgres, params1) = CreatePostgresContainerResourceManualInput("postgrescontainer", generatedInputs, passwordsSet);
        var (postgresTwo, params2) = CreatePostgresContainerResourceManualInput("postgrescontainer2", generatedInputs, passwordsSet);

        var resources = new Dictionary<string, Resource>
        {
            { "postgrescontainer", postgres },
            { "postgrescontainer2", postgresTwo },
            { "postgresparams1", params1 },
            { "postgresparams2", params2 },
        };

        var state = CreateAspirateState(nonInteractive: nonInteractive);
        state.LoadedAspireManifestResources = resources;
        state.AspireComponentsToProcess = resources.Keys.ToList();

        return state;
    }

    protected AspirateState CreateAspirateStateWithConnectionStrings(bool nonInteractive = false, string? password = null)
    {
        var (postgres, params1) = CreatePostgresContainerResourceManualInput("postgrescontainer");
        var (postgresTwo, params2) = CreatePostgresContainerResourceManualInput("postgrescontainer2");

        var resources = new Dictionary<string, Resource>
        {
            { "postgrescontainer", postgres },
            { "postgrescontainer2", postgresTwo },
            { "postgresparams1", params1 },
            { "postgresparams2", params2 },
        };

        (resources["postgrescontainer"] as IResourceWithEnvironmentalVariables).Env = new Dictionary<string, string>
        {
            ["ConnectionString_Test"] = "some_secret_value",
        };

        (resources["postgrescontainer2"] as IResourceWithEnvironmentalVariables).Env = new Dictionary<string, string>
        {
            ["ConnectionString_Test"] = "some_secret_value",
        };

        var state = CreateAspirateState(nonInteractive: nonInteractive, password: password);
        state.LoadedAspireManifestResources = resources;
        state.AspireComponentsToProcess = resources.Keys.ToList();

        return state;
    }

    private static (ContainerResource container, ParameterResource parameters) CreatePostgresContainerResourceManualInput(string resourceName, bool generatedInput = false, bool passwordsSet = false)
    {
        var postgres = new ContainerResource
        {
            Name = resourceName,
            Type = AspireComponentLiterals.Container,
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
                        TargetPort = 5432,
                    }
                },
            },
        };

        var parameters = CreateInputs(generatedInput, passwordsSet);

        return (postgres, parameters);
    }

    private static ParameterResource CreateInputs(bool generated = false, bool passwordsSet = false)
    {
        var parameters = new ParameterResource
        {
            Name = "password",
            Type = "string"
        };

        if (generated)
        {
            parameters.Inputs = new Dictionary<string, ParameterInput>
            {
                ["value"] = new()
                {
                    Default = new ParameterDefault
                    {
                        Generate = new Generate
                        {
                            MinLength = 22,
                        },
                    }
                }
            };
        }
        else
        {
            parameters.Inputs = new Dictionary<string, ParameterInput>
            {
                ["value"] = new(),
            };
        }

        if (passwordsSet)
        {
            parameters.Value = "some-password";
        }

        return parameters;
    }
}
