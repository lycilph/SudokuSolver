using System;
using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Hidden Quads
 * 
 * All Hidden Subsets work the same way, the only thing that changes is the number of cells and candidates affected by the move.
 * Take Hidden Pair: If you can find two cells within a house such as that two candidates appear nowhere outside 
 * those cells in that house, those two candidates must be placed in the two cells. All other candidates can therefore be eliminated.
 * (Source: https://hodoku.sourceforge.net/en/tech_hidden.php)
 * (Source: https://www.sudokuwiki.org/Hidden_Candidates)
 * 
 * Test puzzle(s): 
 * 65..87.24...649.5..4..25...57.438.61...5.1...31.9.2.85...89..1....213...13.75..98 (Source: https://www.sudokuwiki.org/Hidden_Candidates#HT)
 * 9.15...46425.9..8186..1..2.5.2.......19...46.6.......2196.4.2532...6.817.....1694 (Source: https://www.sudokuwiki.org/Hidden_Candidates#HT)
 */

public class HiddenQuadsStrategy : BaseStrategy<HiddenQuadsStrategy>    
{
    public override string Name => "Hidden Quads";

    public override ICommand? Plan(Grid grid)
    {
        var command = new HiddenQuadsCommand(Name);
        var quads_found = new HashSet<(Cell, Cell, Cell, Cell)>(); // This is used to find duplicates (ie. a triple found in a row, could also be found in a box)

        foreach (var unit in grid.AllUnits)
            FindHiddenQuads(unit, command, quads_found);

        return command.IsValid() ? command : null;
    }

    private void FindHiddenQuads(Unit unit, HiddenQuadsCommand command, HashSet<(Cell, Cell, Cell, Cell)> quads_found)
    {
        // Make a dictionary of digit to cells that contain that digit (but only if there are exactly 2, 3, or 4 cells with that value)
        var digit_to_cells = new Dictionary<int, Cell[]>();
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = unit.EmptyCells().Where(c => c.Candidates.Contains(digit)).ToArray();
            if (cells_with_digit.Length >= 2)
                digit_to_cells[digit] = cells_with_digit;
        }

        // Go through all possible quads of digits and see if they are present in the dictionary from above
        for (int i = 1; i <= 6; i++)
        {
            for (int j = i + 1; j <= 7; j++)
            {
                for (var k = j + 1; k <= 8; k++)
                {
                    for (var m = k + 1; m <= 9; m++)
                    {
                        // Check if this quad of digits (i,j,k,m) is actually present in the unit
                        if (digit_to_cells.ContainsKey(i) &&
                            digit_to_cells.ContainsKey(j) &&
                            digit_to_cells.ContainsKey(k) &&
                            digit_to_cells.ContainsKey(m))
                        {
                            var union = digit_to_cells[i]
                                .Union(digit_to_cells[j])
                                .Union(digit_to_cells[k])
                                .Union(digit_to_cells[m])
                                .Distinct()
                                .ToArray();

                            // Check if this is amounts to an actual quad
                            if (union.Length == 4)
                            {
                                // Check if this triple has already been found
                                quads_found.Add((union[0], union[1], union[2], union[3]));

                                // We have now found a quad, mark all other candidates in the cells for elimination
                                foreach (var cell in union)
                                {
                                    foreach (var candidate_to_eliminate in cell.Candidates.Except([i, j, k, m]))
                                        command.Add(new CommandElement
                                        {
                                            Description = $"Hidden quad ({i},{j},{k},{m}) in {unit.FullName}: Eliminates {candidate_to_eliminate} in cell {cell.Index}",
                                            Numbers = [candidate_to_eliminate],
                                            Cells = [cell],
                                            NumbersToVisualize = [i, j, k, m],
                                            CellsToVisualize = [union[0], union[1], union[2], union[3]]
                                        });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
