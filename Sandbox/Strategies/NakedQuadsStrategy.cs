using Sandbox.Model;

namespace Sandbox.Strategies;

public class NakedQuadsStrategy : IStrategy
{
    public string Name => "Naked Quads";

    public static readonly NakedQuadsStrategy Instance = new();

    public bool Step(Grid grid)
    {
        var found_quads = false;
        foreach (var unit in grid.AllUnits)
        {
            if (FindNakedQuads(unit))
                found_quads = true;
        }
        return found_quads;
    }

    private bool FindNakedQuads(Unit unit)
    {
        var found_quads = false;
        
        var empty_cells = unit.Cells.Where(c => c.IsEmpty).ToArray();
        if (empty_cells.Length <= 4)
            return false;

        var candidates = empty_cells.Where(c => c.CandidatesCount >= 2 && c.CandidatesCount <= 4).ToArray();
        if (candidates.Length < 4)
            return false;

        for (int i = 0; i < candidates.Length - 3; i++)
        {
            for (int j = i + 1; j < candidates.Length - 2; j++)
            {
                for (int k = j + 1; k < candidates.Length - 1; k++)
                {
                    for (int m = k + 1; m < candidates.Length; m++)
                    {
                        var quad = new Cell[] { candidates[i], candidates[j], candidates[k], candidates[m] };
                        var quad_candidates = quad.SelectMany(c => c.Candidates).Distinct().ToArray();

                        if (quad_candidates.Length == 4)
                        {
                            foreach (var cell in empty_cells)
                            {
                                if (cell == candidates[i] || cell == candidates[j] || cell == candidates[k] || cell == candidates[m])
                                    continue;

                                if (cell.Candidates.Overlaps(quad_candidates))
                                {
                                    cell.Candidates.ExceptWith(quad_candidates);
                                    Console.WriteLine($" * Found naked quad ({string.Join(',', quad_candidates)}) in {unit.FullName} removes candidates from {cell.Index}");
                                    found_quads = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        return found_quads;
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
