using Core.Model;

namespace VisualStrategyDebugger.Temp;

public class NakedSinglesStrategy : IStrategy
{
    public string Name { get; } = "Naked Singles";

    public IGridCommand? Plan(Grid grid)
    {
        var command = new NakedSinglesCommand();

        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            command.Elements.Add(new CommandElement()
            {
                Description = $"Naked single {cell.Candidates.First()} found in cell {cell.Index}",
                Number = cell.Candidates.First(),
                Cells = [cell]
            });
        }

        command.UpdateDescription(Name);
        return command.Elements.Count > 0 ? command : null;
    }
}
