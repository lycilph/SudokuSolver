using System.Text;

namespace VisualStrategyDebugger.Temp;

public class NakedSinglesCommand : IGridCommand
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public List<CommandElement> Elements { get; set; } = [];

    public void Do()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
            {
                cell.Value = element.Number;
                cell.Candidates.Clear();
            }
    }

    public void Undo()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
            {
                cell.Value = 0;
                cell.Candidates.Clear();
                cell.Candidates.Add(element.Number);
            }
    }

    public IVisualizer GetVisualizer()
    {
        return new NakedSinglesVisualizer(this);
    }

    public void UpdateDescription(string name)
    {
        Name = name;

        var sb = new StringBuilder();
        sb.AppendLine(name);
        foreach (var e in Elements)
            sb.AppendLine($" * {e.Description}");
        Description = sb.ToString();
    }
}
