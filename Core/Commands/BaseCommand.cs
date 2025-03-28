using Core.Strategies;
using System.Text;

namespace Core.Commands;

/// <summary>
/// Base class for the commands used by the strategy classes
/// </summary>

public abstract class BaseCommand(IStrategy strategy) : ICommand
{
    public IStrategy Strategy { get; private set; } = strategy;
    public string Name => Strategy.Name;
    public int Difficulty => Strategy.Difficulty;
    public string Description => ToString();

    public List<CommandElement> Elements { get; set; } = [];

    public virtual void Do() => throw new NotImplementedException();
    public virtual void Undo() => throw new NotImplementedException();

    public void Add(CommandElement element) => Elements.Add(element);
    public bool IsValid() => Elements.Count > 0;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Name);
        foreach (var e in Elements)
            sb.AppendLine($" * {e.Description}");
        return sb.ToString();
    }
}
