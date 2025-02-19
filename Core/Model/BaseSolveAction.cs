using System.Text;

namespace Core.Model;

public class BaseSolveAction : ISolveAction
{
    public string Description { get; set; } = string.Empty;
    public List<SolveActionElement> Elements { get; set; } = [];

    public virtual void Apply(Grid grid) {}

    public virtual void Undo(Grid grid) {}

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Description);
        foreach (var e in Elements)
            sb.AppendLine(e.Description);
        return sb.ToString();
    }
}
