using Core.Model;

namespace Core.Strategies;

public class NakedTriplesStrategy : IStrategy
{
    public string Name => "Naked Triples";

    public static readonly NakedTriplesStrategy Instance = new();

    public bool Step(Grid grid, bool verbose = true)
    {
        var found_triples = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindNakedTriples(unit, verbose))
                found_triples = true;
        }
        return found_triples;
    }

    private bool FindNakedTriples(Unit unit, bool verbose)
    {
        var found_triples = false;

        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();
        if (empty_cells.Length <= 3)
            return false;

        var candidates = empty_cells.Where(c => c.CandidatesCount == 2 || c.CandidatesCount == 3).ToArray();
        if (candidates.Length < 3)
            return false;

        for (int i = 0; i < candidates.Length - 2; i++)
        {
            for (int j = i + 1; j < candidates.Length - 1; j++)
            {
                for (int k = j + 1; k < candidates.Length; k++)
                {
                    var triple = new Cell[] { candidates[i], candidates[j], candidates[k] };
                    var triple_candidates = triple.SelectMany(c => c.Candidates).Distinct().ToArray();

                    if (triple_candidates.Length == 3)
                    {
                        foreach (var cell in empty_cells)
                        {
                            if (cell == candidates[i] || cell == candidates[j] || cell == candidates[k])
                                continue;

                            if (cell.Candidates.Overlaps(triple_candidates))
                            {
                                cell.Candidates.ExceptWith(triple_candidates);
                                found_triples = true;
                                if (verbose)
                                    Console.WriteLine($" * Found naked triple ({string.Join(',', triple_candidates)}) in {unit.FullName} removes candidates from {cell.Index}");
                            }
                        }
                    }
                }
            }
        }

        return found_triples;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
