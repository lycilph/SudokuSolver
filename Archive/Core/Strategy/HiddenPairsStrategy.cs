using Core.Model;
using Core.Model.Actions;

namespace Core.Strategy;

/* Strategy name: Hidden Pairs
 * 
 * All Hidden Subsets work the same way, the only thing that changes is the number of cells and candidates affected by the move.
 * Take Hidden Pair: If you can find two cells within a house such as that two candidates appear nowhere outside 
 * those cells in that house, those two candidates must be placed in the two cells. All other candidates can therefore be eliminated.
 * (Source: https://hodoku.sourceforge.net/en/tech_hidden.php)
 * (Source: https://www.sudokuwiki.org/Hidden_Candidates)
 * 
 * Test puzzle(s): 
 * 72.4.8.3..8.....474.1.768.281.739......851......264.8.2.968.41334......8168943275 (Source: https://www.sudokuwiki.org/Hidden_Candidates)
 * .49132....81479...327685914.96.518...75.28....38.46..5853267...712894563964513... (Source: https://hodoku.sourceforge.net/en/tech_hidden.php)
 */

public class HiddenPairsStrategy : BaseStrategy<HiddenPairsStrategy>
{
    public override string Name => "Hidden Pairs";

    public override IPuzzleAction? Execute(Grid grid)
    {
        var action = new EliminationSolvePuzzleAction() { Description = Name };
        var pairs_found = new HashSet<(Cell, Cell)>(); // This is used to find duplicates (ie. a pair found in a row, could also be found in a box)

        foreach (var unit in grid.AllUnits)
            FindHiddenPairs(unit, action, pairs_found);

        if (action.HasElements())
            return action;
        else
            return null;
    }

    private void FindHiddenPairs(Unit unit, EliminationSolvePuzzleAction action, HashSet<(Cell, Cell)> pairs_found)
    {
        // Make a dictionary of digit to cells that contain that digit (but only if there are exactly 2 cells with that digit)
        var digit_to_cells = new Dictionary<int, Cell[]>();
        foreach (var digit in Grid.PossibleValues)
        {
            var cells_with_digit = unit.EmptyCells().Where(c => c.Candidates.Contains(digit)).ToArray();
            if (cells_with_digit.Length == 2)
                digit_to_cells[digit] = cells_with_digit;
        }

        // Go through all possible pairs of digits and see if they are present in the dictionary from above
        for (int i = 1; i <= 8; i++)
        {
            for (int j = i + 1; j <= 9; j++)
            {
                // Check if this pair of digits (i,j) is actually present in the unit
                if (digit_to_cells.ContainsKey(i) &&
                    digit_to_cells.ContainsKey(j) &&
                    digit_to_cells[i].SequenceEqual(digit_to_cells[j]))
                {
                    // Since the the entries for i and j should be the same, either can be used to make the pair
                    var cell_pair = (digit_to_cells[i][0], digit_to_cells[i][0]);

                    // Check if this pair has already been found
                    if (!pairs_found.Contains(cell_pair))
                    {
                        pairs_found.Add(cell_pair);

                        // We have now found a pair, mark all other candidates in the cells for elimination
                        foreach (var cell in digit_to_cells[i])
                        {
                            foreach (var candidates_to_eliminate in cell.Candidates.Except([i, j]))
                                action.Add(new SolveActionElement()
                                {
                                    Description = $"Hidden pair ({i},{j}) in {unit.FullName}: Eliminates {candidates_to_eliminate} in cell {cell.Index}",
                                    Number = candidates_to_eliminate,
                                    Cells = [cell]
                                });
                        }
                    }
                }
            }
        }
    }
}
