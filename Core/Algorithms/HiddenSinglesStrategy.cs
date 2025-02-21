using Core.Model;

namespace Core.Algorithms;

/* Strategy name: Hidden Singles
 * 
 * Find cells where a value is only present as a candidate in one cell in a unit
 * (Source: https://hodoku.sourceforge.net/en/tech_singles.php)
 * 
 * Test puzzle(s): 
 * .28..7....16.83.7.....2.85113729.......73........463.729..7.......86.14....3..7.. (Source: https://hodoku.sourceforge.net/en/tech_singles.php)
 */

public class HiddenSinglesStrategy : BaseStrategy<HiddenSinglesStrategy>
{
    public override string Name => "Hidden Singles";

    public override ISolveAction? Execute(Grid grid)
    {
        var action = new ValueSolveAction() { Description = Name };
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
                    action.Add(new SolveActionElement()
                    {
                        Description = $"Hidden single found in {unit.FullName}: {digit} can only go in cell {candidates.First().Index}",
                        Number = digit,
                        Cells = candidates
                    });
                }
            }
        }

        if (action.HasElements())
            return action;
        else
            return null;
    }
}
