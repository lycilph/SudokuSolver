using System;
using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Hidden Triples
 * 
 * Take two rows (the base sets). If you can find two columns, such that all candidates of a specific digit (the fish digit) 
 * in both rows are containd in the columns (the cover sets), all fish candidates in the columns that are not part of the rows can be eliminated. 
 * The result is called an X-Wing in the rows.
 * If you exchange the terms rows and columns in the description above, you get an X-Wing in the columns.
 * (Source: https://hodoku.sourceforge.net/en/tech_fishb.php)
 * (Source: https://www.sudokuwiki.org/X_Wing_Strategy)
 * 
 * Test puzzle(s): 
 * 1.....569492.561.8.561.924...964.8.1.64.1....218.356.4.4.5...169.5.614.2621.....5 (X-wing in rows) (Source: https://www.sudokuwiki.org/Hidden_Candidates#HT)
 * .......9476.91..5..9...2.81.7..5..1....7.9....8..31.6724.1...7..1..9..459.....1.. (X-wing in columns) (Source: https://www.sudokuwiki.org/Hidden_Candidates#HT)
 */

public class XWingStrategy : BaseStrategy<XWingStrategy>
{
    public override string Name => "X-Wing";

    public override ICommand? Plan(Grid grid)
    {
        var command = new XWingCommand(Name);

        FindXWings(grid.Rows, grid.Columns, UnitType.Row, UnitType.Column, command); // Look for x-wing in the rows
        FindXWings(grid.Columns, grid.Rows, UnitType.Column, UnitType.Row, command); // Look for x-wing in the columns

        return command.IsValid() ? command : null;
    }

    private void FindXWings(Unit[] base_sets, Unit[] cover_sets, UnitType base_set_type, UnitType cover_set_type, XWingCommand command)
    {
        //var base_str = base_sets.First().Name;
        //var cover_str = cover_sets.First().Name;

        // Find candidates
        /// Tuple where:
        /// - Item1 is possible value
        /// - Item2 is first cell containing possible value (ie. item1)
        /// - Item3 is second cell containing possible value (ie. item1)
        var candidates = new List<Tuple<int, Cell, Cell>>();

        // Find potential pairs for the x-wing
        foreach (var value in Grid.PossibleValues)
        {
            foreach (var unit in base_sets)
            {
                var pair_candidates = unit.Cells.Where(c => c.Candidates.Contains(value)).ToArray();
                if (pair_candidates.Length == 2)
                    candidates.Add(Tuple.Create(value, pair_candidates[0], pair_candidates[1]));
            }
        }

        for (int i = 0; i < candidates.Count - 1; i++)
        {
            for (int j = i + 1; j < candidates.Count; j++)
            {
                if (IsXWing(candidates[i], candidates[j]))
                {
                    int value = candidates[i].Item1;
                    Unit[] units_to_process = [cover_sets[candidates[i].Item2.GetIndexInUnit(cover_set_type)], cover_sets[candidates[i].Item3.GetIndexInUnit(cover_set_type)]];
                    int[] indices_to_ignore = [candidates[i].Item2.GetIndexInUnit(base_set_type), candidates[j].Item2.GetIndexInUnit(base_set_type)];

                    EliminateCandidates(value, units_to_process, indices_to_ignore, base_set_type, command);
                }
            }
        }
    }

    private void EliminateCandidates(int value, Unit[] cover_sets, int[] indices_to_ignore, UnitType base_set_type, XWingCommand command)
    {
        var xwing_cells = cover_sets
            .SelectMany(u => u.Cells)
            .Where(c => indices_to_ignore.Contains(c.GetIndexInUnit(base_set_type)))
            .OrderBy(c => c.Index)
            .ToList();

        foreach (Unit cover_unit in cover_sets)
        {
            var cells_to_eliminate_value_in = cover_unit.Cells.Where(c => !indices_to_ignore.Contains(c.GetIndexInUnit(base_set_type)) && c.Contains(value));

            if (cells_to_eliminate_value_in.Any())
                command.Add(new CommandElement
                {
                    Description = $"An x-wing on {value} (in {base_set_type}s and cells {string.Join(',', xwing_cells.Select(c => c.Index))}) removes {value} from {cover_unit.FullName} (cells {string.Join(',', cells_to_eliminate_value_in.Select(c => c.Index))})",
                    Numbers = [value],
                    Cells = [.. cells_to_eliminate_value_in],
                    CellsToVisualize = xwing_cells.ToList()
                });
        }
    }

    // Check if two candidates form an x-wing pattern
    // ie. the two candidates has the same value, in two different rows, and in the same columns
    //  or the two candidates has the same value, in two different columns, and in the same rows
    private bool IsXWing(Tuple<int, Cell, Cell> candidate1, Tuple<int, Cell, Cell> candidate2)
    {
        return candidate1.Item1 == candidate2.Item1 &&
            (candidate1.Item2.Row != candidate2.Item2.Row &&
             candidate1.Item2.Column == candidate2.Item2.Column &&
             candidate1.Item3.Column == candidate2.Item3.Column
             ||
             candidate1.Item2.Column != candidate2.Item2.Column &&
             candidate1.Item2.Row == candidate2.Item2.Row &&
             candidate1.Item3.Row == candidate2.Item3.Row);
    }
}
