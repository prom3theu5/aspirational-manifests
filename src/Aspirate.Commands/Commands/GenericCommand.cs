namespace Aspirate.Commands.Commands;

[ExcludeFromCodeCoverage]
public class GenericCommand : Command
{
    public GenericCommand(string name, string description)
        : base(name, description) =>
        Handler = CommandHandler.Create<IServiceCollection>(ExecuteCommand);

    protected virtual Task<int> ExecuteCommand(IServiceCollection services) => Task.FromResult(0);

    protected static Table CreateHelpTable()
    {
        var table = new Table();
        table.AddColumn("Sub Commands");
        table.AddColumn("Description");
        return table;
    }
}
