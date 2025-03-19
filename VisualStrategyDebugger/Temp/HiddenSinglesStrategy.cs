using Core.Model;

namespace VisualStrategyDebugger.Temp;

public class HiddenSinglesStrategy : IStrategy
{
    public string Name { get; } = "Hidden Singles";

    public IGridCommand? Plan(Grid grid)
    {
        var command = new HiddenSinglesCommand();
        var singles_found = new HashSet<int>(); // This is used to find duplicates (ie. single found in a row, could also be found in a box)

        foreach (var unit in grid.AllUnits)
        {
            var filled_cells = unit.FilledCells();
            var empty_cells = unit.EmptyCells();
            var digits_left_in_unit = Grid.PossibleValues.Except(filled_cells.Select(c => c.Value)).ToArray();

            foreach (var digit in digits_left_in_unit)
            {
                var candidates = empty_cells.Where(c => c.Candidates.Contains(digit)).ToList();
                if (candidates.Count == 1 && !singles_found.Contains(candidates.First().Index))
                {
                    singles_found.Add(candidates.First().Index);
                    command.Elements.Add(new CommandElement()
                    {
                        Description = $"Hidden single found in {unit.FullName}: {digit} can only go in cell {candidates.First().Index}",
                        Number = digit,
                        Cells = candidates
                    });
                }
            }
        }

        command.UpdateDescription(Name);
        return command.Elements.Count > 0 ? command : null;
    }
}
