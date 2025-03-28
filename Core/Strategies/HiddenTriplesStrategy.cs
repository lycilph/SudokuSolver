using System;
using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Hidden Triples
 * 
 * All Hidden Subsets work the same way, the only thing that changes is the number of cells and candidates affected by the move.
 * Take Hidden Pair: If you can find two cells within a house such as that two candidates appear nowhere outside 
 * those cells in that house, those two candidates must be placed in the two cells. All other candidates can therefore be eliminated.
 * (Source: https://hodoku.sourceforge.net/en/tech_hidden.php)
 * (Source: https://www.sudokuwiki.org/Hidden_Candidates)
 * 
 * Test puzzle(s): 
 * .....1.3.231.9.....65..31..6789243..1.3.5...6...1367....936.57...6.198433........ (Source: https://www.sudokuwiki.org/Hidden_Candidates#HT)
 */

public class HiddenTriplesStrategy : BaseStrategy<HiddenTriplesStrategy>
{
    public override string Name => "Hidden Triples";
    public override int Difficulty => 7;

    public override ICommand? Plan(Grid grid)
    {
        var command = new HiddenTriplesCommand(this);
        var triples_found = new HashSet<(Cell, Cell, Cell)>(); // This is used to find duplicates (ie. a triple found in a row, could also be found in a box)

        foreach (var unit in grid.AllUnits)
            FindHiddenTriples(unit, command, triples_found);

        return command.IsValid() ? command : null;
    }

    private void FindHiddenTriples(Unit unit, HiddenTriplesCommand command, HashSet<(Cell, Cell, Cell)> triples_found)
    {
        // Make a dictionary of digit to cells that contain that digit (but only if there are exactly 2 or 3 cells with that value)
        var digit_to_cells = new Dictionary<int, Cell[]>();
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = unit.EmptyCells().Where(c => c.Candidates.Contains(digit)).ToArray();
            if (cells_with_digit.Length >= 2)
                digit_to_cells[digit] = cells_with_digit;
        }

        // Go through all possible triples of digits and see if they are present in the dictionary from above
        for (int i = 1; i <= 7; i++)
        {
            for (int j = i + 1; j <= 8; j++)
            {
                for (var k = j + 1; k <= 9; k++)
                {
                    // Check if this triple of digits (i,j,k) is actually present in the unit
                    if (digit_to_cells.ContainsKey(i) &&
                        digit_to_cells.ContainsKey(j) &&
                        digit_to_cells.ContainsKey(k))
                    {
                        var union = digit_to_cells[i]
                            .Union(digit_to_cells[j])
                            .Union(digit_to_cells[k])
                            .Distinct()
                            .ToArray();

                        // Check if this is amounts to an actual triple
                        if (union.Length == 3)
                        {
                            // Check if this triple has already been found
                            triples_found.Add((union[0], union[1], union[2]));

                            // We have now found a triple, mark all other candidates in the cells for elimination
                            foreach (var cell in union)
                            {
                                foreach (var candidate_to_eliminate in cell.Candidates.Except([i, j, k]))
                                    command.Add(new CommandElement
                                    {
                                        Description = $"Hidden triple ({i},{j},{k}) in {unit.FullName}: Eliminates {candidate_to_eliminate} in cell {cell.Index}",
                                        Numbers = [candidate_to_eliminate],
                                        Cells = [cell],
                                        NumbersToVisualize = [i, j, k],
                                        CellsToVisualize = [union[0], union[1], union[2]]
                                    });
                            }
                        }
                    }
                }
            }
        }
    }
}
