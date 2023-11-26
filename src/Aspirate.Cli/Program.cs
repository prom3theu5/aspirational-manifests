AspirateCli.WelcomeMessage();

return await new CommandLineBuilder(new AspirateCli())
    .UseDefaults()
    .UseDependencyInjection(services => services.RegisterAspirateEssential())
    .Build()
    .InvokeAsync(args);

