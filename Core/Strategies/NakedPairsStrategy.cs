using Core.Model;

namespace Core.Strategies;

public class NakedPairsStrategy : IStrategy
{
    public string Name => "Naked Pairs";

    // Find pairs where 2 cells share the same 2 candidates, and remove these candidates from the other cells in the unit
    public bool Step(Grid grid, bool verbose = true)
    {
        var found_pairs = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindNakedPairs(unit, verbose))
                found_pairs = true;
        }
        return found_pairs;
    }

    private bool FindNakedPairs(Unit unit, bool verbose)
    {
        var found_pairs = false;

        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();
        var pair_candidates = unit.Cells.Where(c => c.CandidatesCount == 2).ToArray();

        foreach (var candidate1 in pair_candidates)
        {
            foreach (var candidate2 in pair_candidates)
            {
                if (candidate1 == candidate2)
                    continue;

                if (candidate1.Candidates.SetEquals(candidate2.Candidates))
                {
                    foreach (var cell in empty_cells)
                    {
                        if (cell == candidate1 || cell == candidate2)
                            continue;
                        if (cell.Candidates.Overlaps(candidate1.Candidates))
                        {
                            cell.Candidates.ExceptWith(candidate1.Candidates);
                            found_pairs = true;
                            if (verbose)
                                Console.WriteLine($" * Naked pair ({string.Join(',', candidate1.Candidates)}) overlaps with cell {cell.Index} (in {unit.FullName})");
                        }
                    }
                }
            }
        }

        return found_pairs;
    }
}
