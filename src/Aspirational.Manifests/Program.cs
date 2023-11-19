var executor = CommandExecutor.For(
    options =>
    {
        options.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
        options.SetAppName(AppLiterals.AppName);
    });

return executor.Execute(args);
