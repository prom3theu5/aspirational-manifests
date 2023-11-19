var services = new ServiceCollection();

services.RegisterAspirateEssential();

var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);

#pragma warning disable IDE0200 // Remove unnecessary lambda expression, for now - we will have more commands.
app.Configure(config =>
{
    EndToEndCommand.RegisterEndToEndCommand(config);
#pragma warning restore IDE0200 // Remove unnecessary lambda expression
});

return app.Run(args);
