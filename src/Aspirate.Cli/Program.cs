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
            .WithExample(["e2e", "-m", "./Example/aspire-manifest.json", "-o", "./output"]);
    });

return app.Run(args);
