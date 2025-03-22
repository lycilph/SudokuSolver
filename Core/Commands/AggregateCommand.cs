using Core.Extensions;
using System.Text;

namespace Core.Commands;

/// <summary>
/// This action can aggregate puzzle and execute them 1 at a time. This is used
/// when multiple actions needs to the done/undone at once
/// </summary>

public class AggregateCommand : ICommand
{
    public string Name { get; } = "Aggregate Command";
    public string Description => ToString();

    private readonly List<ICommand> commands = [];

    public void Add(ICommand command) => commands.Add(command);
    public void Do() => commands.ForEach(c => c.Do());
    public void Undo() => commands.AsEnumerable().Reverse().ForEach(c => c.Undo());
    public bool IsValid() => true;

    public override string ToString()
    {
        var sb = new StringBuilder();
        commands.ForEach(c => sb.AppendLine(c.ToString()));
        return sb.ToString();
    }
}