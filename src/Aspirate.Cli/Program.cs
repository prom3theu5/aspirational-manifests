AspirateCli.WelcomeMessage();

return await new CommandLineBuilder(new AspirateCli())
    .UseDefaults()
    .UseDependencyInjection(services => services.RegisterAspirateEssential())
    .UseHelp(AspirateCli.UseDefaultMasking)
    .Build()
    .InvokeAsync(args);
