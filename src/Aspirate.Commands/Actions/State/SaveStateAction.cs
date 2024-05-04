namespace Aspirate.Commands.Actions.State;

public class SaveStateAction(IServiceProvider serviceProvider, IFileSystem fileSystem)
    : BaseAction(serviceProvider)
{
    public override async Task<bool> ExecuteAsync()
    {
        Logger.WriteRuler("[purple]Saving State[/]");

        var stateFile = fileSystem.Path.Combine(fileSystem.Directory.GetCurrentDirectory(), AspirateLiterals.StateFileName);
        var stateAsJson = JsonSerializer.Serialize(CurrentState);

        await fileSystem.File.WriteAllTextAsync(stateFile, stateAsJson);

        Logger.MarkupLine($"[green]({EmojiLiterals.CheckMark}) Done: [/] State saved successfully to [blue]{stateFile}[/]");

        return true;
    }
}
