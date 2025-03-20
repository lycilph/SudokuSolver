using System.Text;

namespace Core.Model.Actions;

/// <summary>
/// This is a base class for the ValueSolvePuzzleAction and EliminationSolvePuzzleAction used by the solver
/// </summary>
public abstract class BaseSolveAction : IPuzzleAction
{
    public string Description { get; set; } = string.Empty;
    public List<SolveActionElement> Elements { get; set; } = [];

    public virtual void Do() {}
    public virtual void Undo() {}

    public void Add(SolveActionElement element) => Elements.Add(element);
    public bool HasElements() => Elements.Count > 0;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Description);
        foreach (var e in Elements)
            sb.AppendLine($" * {e.Description}");
        return sb.ToString();
    }
}
