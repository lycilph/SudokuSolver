using Core.Commands;
using Core.Models;

namespace Core.Strategies;

/* Strategy name: Chute Remote Pairs
 * 
 * The first example gives the classic pattern. We're looking for two bi-value cells with the same candidates in the same Chute,
 * which is a horizontal or vertical set of the three boxes. There are six chutes in a 9x9 Sudoku.
 * 
 * Given the remote pair cells. The next step is to check they can't 'see' each other, 
 * ie they are not a Naked Pair but instead a Remote Pair.
 * 
 * Given that pattern there will exist exactly three cells in the unused box in the same chute.
 * 
 * The rule says,
 * If the yellow cells have only ONE of the candidates in the remote pair cells then that candidate can be eliminated from all cells 
 * seen by both remote pair cells.
 * 
 * (Source: https://www.sudokuwiki.org/Chute_Remote_Pairs)
 * 
 * Test puzzle(s): 
 * .2.9.56.39.5.3..12.631...5939..5..6.5.63.19.4.4.769385.3.5...9.859.13...614297538  (Source: https://www.sudokuwiki.org/Chute_Remote_Pairs)
 * 35.682.171.6573...7289413656..25.....1.43..5.5..168..4265814793...3965...3.725.8.  (Source: https://www.sudokuwiki.org/Chute_Remote_Pairs)
 */

public class ChuteRemotePairsStrategy : BaseStrategy<ChuteRemotePairsStrategy>
{
    public override string Name => "Chute Remote Pairs";

    public override ICommand? Plan(Grid grid)
    {
        var command = new ChuteRemotePairsCommand(Name);

        foreach (var chute in grid.AllChutes)
            FindRemotePairs(chute, command);

        return command.IsValid() ? command : null;
    }

    private void FindRemotePairs(Chute chute, ChuteRemotePairsCommand command)
    {
        // Find pairs in chute
        var pairs = new HashSet<(Cell, Cell)>();
        var pair_candidates = chute.Cells.Where(c => c.Count() == 2).ToArray();

        for (int i = 0; i < pair_candidates.Length - 1; i++)
        {
            for (int j = i + 1; j < pair_candidates.Length; j++)
            {
                // Is there a pair in [i,j]
                if (pair_candidates[i].Candidates.SetEquals(pair_candidates[j].Candidates))
                {
                    var pair = (pair_candidates[i], pair_candidates[j]);

                    // Check if they are proper remote pairs ie. they cannot "see" each other (cell 1 must not be in the peers of cell 2)
                    if (!pair.Item2.Peers.Contains(pair.Item1))
                        pairs.Add(pair);
                }
            }
        }

        // Check the last box in the chute (the one that doesn't contain any of the pair cells)
        foreach (var pair in pairs)
        {
            var box_with_cell1 = chute.Boxes.Single(b => b.Cells.Contains(pair.Item1));
            var box_with_cell2 = chute.Boxes.Single(b => b.Cells.Contains(pair.Item2));
            var last_box = chute.Boxes.Single(b => b.Index != box_with_cell1.Index && b.Index != box_with_cell2.Index);
            var last_box_row = last_box.Cells.Except(pair.Item1.Peers).Except(pair.Item2.Peers).ToArray();

            // Get the overlap in candidates between the remote pair and the last_box_row
            var candidates_overlap = last_box_row
                .SelectMany(c => c.Candidates)
                .Distinct()
                .Intersect(pair.Item1.Candidates);

            // If 1 and only 1 of the pair candidates are in the last box row, then we can continue with eliminations
            if (candidates_overlap.Count() == 1)
            {
                var candidate_to_eliminate = candidates_overlap.Single();

                // Finding cells that potentially could contain candidates to be eliminated
                var cells_in_box1 = box_with_cell1.Cells.Intersect(pair.Item2.Peers);
                var cells_in_box2 = box_with_cell2.Cells.Intersect(pair.Item1.Peers);
                var potential_cells = cells_in_box1.Concat(cells_in_box2).Where(c => c.Contains(candidate_to_eliminate)).ToList();

                if (potential_cells.Count > 0)
                    command.Add(new CommandElement
                    {
                        Description = $"{chute.FullName}: Value {candidate_to_eliminate} cannot be in cell(s) {string.Join(',',potential_cells.Select(c => c.Index))} due to chute remote pairs ({pair.Item1.Index},{pair.Item2.Index}) in [{pair.Item1.GetCandidatesAsShortString()}]",
                        Numbers = [candidate_to_eliminate],
                        Cells = potential_cells,
                        CellsToVisualize = [pair.Item1, pair.Item2, .. last_box_row],
                        NumbersToVisualize = pair.Item1.Candidates.ToList()
                    });
            }
        }
    }
}
