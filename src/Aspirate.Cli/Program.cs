var services = new ServiceCollection();

services.RegisterAspirateEssential();

var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(
    config =>
    {
        config.AddCommand<EndToEndCommand>(EndToEndCommand.EndToEndCommandName)
            .WithDescription(EndToEndCommand.EndToEndDescription)
            .WithAlias("e2e")
            .WithExample(["e2e", "-p", ".", "-o", "./output"]);

        config.AddCommand<InitCommand>(InitCommand.InitCommandName)
            .WithDescription(InitCommand.InitDescription)
            .WithExample(["init", "-p", "."]);
    });

return app.Run(args);
