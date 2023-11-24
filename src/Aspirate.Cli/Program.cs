var services = new ServiceCollection();

services.RegisterAspirateEssential();

var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.AddCommand<InitCommand>(InitCommand.CommandName)
            .WithDescription(InitCommand.CommandDescription)
            .WithExample(["init"])
            .WithExample(["init", "-p", "/path/to/appHost"]);

        config.AddCommand<GenerateCommand>(GenerateCommand.CommandName)
            .WithDescription(GenerateCommand.CommandDescription)
            .WithAlias("gen")
            .WithExample(["generate"])
            .WithExample(["generate", "-p", "/path/to/appHost", "-o", "./output"]);

        config.AddCommand<ApplyCommand>(ApplyCommand.CommandName)
            .WithDescription(ApplyCommand.CommandDescription)
            .WithExample(["apply"])
            .WithExample(["apply", "-k", "./output"]);

        config.AddCommand<DestroyCommand>(DestroyCommand.CommandName)
            .WithDescription(DestroyCommand.CommandDescription)
            .WithExample(["destroy"])
            .WithExample(["destroy", "-k", "./output"]);

        config.SetApplicationName("aspirate");
    });

return app.Run(args);
