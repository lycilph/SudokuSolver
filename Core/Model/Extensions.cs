namespace Core.Model;

public static class Extensions
{
    // This returns any naked pairs in a unit, ie. 2 cells that share 2 (and only 2) candidates
    public static IEnumerable<(Cell, Cell)> FindNakedPairs(this Unit unit)
    {
        var result = new List<(Cell, Cell)>();
        var pair_candidates = unit.Cells.Where(c => c.CandidatesCount == 2).ToArray();

        for (int i = 0; i < pair_candidates.Length - 1; i++)
        {
            for (int j = i+1; j < pair_candidates.Length; j++)
            {
                if (pair_candidates[i].Candidates.SetEquals(pair_candidates[j].Candidates))
                    result.Add((pair_candidates[i], pair_candidates[j]));
            }
        }

        return result;
    }

    public static IEnumerable<Cell> EmptyCells(this Unit unit) => unit.Cells.Where(c => c.IsEmpty).ToArray();
}
