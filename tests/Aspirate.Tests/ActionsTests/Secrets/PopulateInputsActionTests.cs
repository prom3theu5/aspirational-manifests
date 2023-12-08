namespace Aspirate.Tests.ActionsTests.Secrets;

public class PopulateInputsActionTests : BaseActionTests<PopulateInputsAction>
{
    [Fact]
    public async Task ExecuteAsync_InInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;
        EnterPasswordInput(console, "secret_password"); // postgrescontainer
        EnterPasswordInput(console, "other_secret_password"); // postgresContainer2
        var state = CreateAspirateStateWithInputs();
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        var postgres = state.LoadedAspireManifestResources["postgrescontainer"] as IResourceWithInput;
        var postgres2 = state.LoadedAspireManifestResources["postgrescontainer2"] as IResourceWithInput;

        postgres.Inputs["password"].Value.Should().Be("secret_password");
        postgres2.Inputs["password"].Value.Should().Be("other_secret_password");
    }

    [Fact]
    public async Task ExecuteAsync_NonInteractiveMode_ReturnsCorrectResult()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true, generatedInputs: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var result = await action.ExecuteAsync();

        // Assert
        result.Should().BeTrue();

        var postgres = state.LoadedAspireManifestResources["postgrescontainer"] as IResourceWithInput;
        var postgres2 = state.LoadedAspireManifestResources["postgrescontainer2"] as IResourceWithInput;

        postgres.Inputs["password"].Value.Should().HaveLength(20);
        postgres2.Inputs["password"].Value.Should().HaveLength(20);
        postgres.Inputs["password"].Value.Should().NotBe(postgres2.Inputs["password"].Value);
    }

    [Fact]
    public void Validate_NonInteractiveContextSet_ThrowsWhenInputValues()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ValidateNonInteractiveState();

        // Assert
        act.Should().Throw<ActionCausesExitException>();
    }

    [Fact]
    public void Validate_NonInteractiveContextSet_DoesNotThrowWhenOnlyGeneratedSecrets()
    {
        // Arrange
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;
        var state = CreateAspirateStateWithInputs(nonInteractive: true, generatedInputs: true);
        var serviceProvider = CreateServiceProvider(state, console);
        var action = GetSystemUnderTest(serviceProvider);

        // Act
        var act = () => action.ValidateNonInteractiveState();

        // Assert
        act.Should().NotThrow();
    }

    private AspirateState CreateAspirateStateWithInputs(bool nonInteractive = false, bool generatedInputs = false)
    {
        var postgres = CreatePostgresContainerResourceManualInput("postgrescontainer", generatedInputs);
        var postgresTwo = CreatePostgresContainerResourceManualInput("postgrescontainer2", generatedInputs);

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

    private static Container CreatePostgresContainerResourceManualInput(string resourceName, bool generatedInput = false)
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
            Inputs = CreateInputs(generatedInput),
        };
        return postgres;
    }

    private static Dictionary<string, Input> CreateInputs(bool generated = false)
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

        return inputs;
    }

    private static void EnterPasswordInput(TestConsole console, string password)
    {
        // first entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);

        // confirmation entry
        console.Input.PushTextWithEnter(password);
        console.Input.PushKey(ConsoleKey.Enter);
    }
}
