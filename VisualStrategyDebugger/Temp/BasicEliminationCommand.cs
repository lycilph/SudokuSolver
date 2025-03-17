namespace VisualStrategyDebugger.Temp;

public class BasicEliminationCommand : IGridCommand
{
    public string Description => "Basic Elimination Command";

    public List<CommandElement> Elements { get; set; } = [];

    public void Do()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Remove(element.Number);
    }

    public void Undo()
    {
        foreach (var element in Elements)
            foreach (var cell in element.Cells)
                cell.Candidates.Add(element.Number);
    }
}
