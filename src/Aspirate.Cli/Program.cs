var services = new ServiceCollection();

services.RegisterAspirateEssential();

var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(EndToEndCommand.RegisterEndToEndCommand);

return app.Run(args);
