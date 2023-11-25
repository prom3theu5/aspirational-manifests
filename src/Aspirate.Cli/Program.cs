AspirateCli.WelcomeMessage();

return new CommandLineBuilder(new AspirateCli())
    .UseDefaults()
    .UseDependencyInjection(services => services.RegisterAspirateEssential())
    .Build()
    .Invoke(args);
