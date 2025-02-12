using Core.Archive.Model;

namespace Core.Archive.Strategies;

/// <summary>
/// Take two rows (the base sets). If you can find two columns, 
/// such that all candidates of a specific digit (the fish digit) in both rows are containd in the columns (the cover sets), 
/// all fish candidates in the columns that are not part of the rows can be eliminated. 
/// The result is called an X-Wing in the rows. The base and cover sets are reversed for X-Wings in the columns
/// </summary>
/// <remarks>
/// See also:
/// - http://www.sudokuwiki.org/X_Wing_Strategy
/// </remarks>
public class XWingStrategy : IStrategy
{
    public string Name => "X-Wing";

    public static readonly XWingStrategy Instance = new();

    public bool Step(Grid grid, bool verbose = true)
    {
        var found_xwing = false;

        found_xwing |= FindXWings(grid.Rows, grid.Columns, UnitType.Row, UnitType.Column, verbose);
        found_xwing |= FindXWings(grid.Columns, grid.Rows, UnitType.Column, UnitType.Row, verbose);

        return found_xwing;
    }

    private bool FindXWings(Unit[] base_sets, Unit[] cover_sets, UnitType base_set_type, UnitType cover_set_type, bool verbose)
    {
        var found_xwing = false;
        var base_str = base_sets.First().Name;
        var cover_str = cover_sets.First().Name;

        // Find candidates
        /// Tuple where:
        /// - Item1 is possible value
        /// - Item2 is first cell containing possible value (ie. item1)
        /// - Item3 is second cell containing possible value (ie. item1)
        var candidates = new List<Tuple<int, Cell, Cell>>();

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
                    Unit[] units_to_process = [cover_sets[candidates[i].Item2.GetUnitIndex(cover_set_type)], cover_sets[candidates[i].Item3.GetUnitIndex(cover_set_type)]];
                    int[] indices_to_ignore = [candidates[i].Item2.GetUnitIndex(base_set_type), candidates[j].Item2.GetUnitIndex(base_set_type)];

                    if (verbose)
                    {
                        Console.WriteLine($"Found x-wing (base set is {base_str}s)");
                        Console.WriteLine($"Found a pair-candidate on {candidates[i].Item1} in {base_str} {candidates[i].Item2.GetUnitIndex(base_set_type)} ({cover_str}s: {candidates[i].Item2.GetUnitIndex(cover_set_type)} and {candidates[i].Item3.GetUnitIndex(cover_set_type)})");
                        Console.WriteLine($"Found a pair-candidate on {candidates[j].Item1} in {base_str} {candidates[j].Item2.GetUnitIndex(base_set_type)} ({cover_str}s: {candidates[j].Item2.GetUnitIndex(cover_set_type)} and {candidates[j].Item3.GetUnitIndex(cover_set_type)})");
                    }

                    found_xwing |= EliminateCandidates(value, units_to_process, indices_to_ignore, base_set_type, verbose);
                }
            }
        }

        return found_xwing;
    }

    private bool EliminateCandidates(int value, Unit[] cover_sets, int[] indices_to_ignore, UnitType base_set_type, bool verbose)
    {
        var removed_candidates = false;

        foreach (var cell in cover_sets.SelectMany(c => c.Cells))
        {
            if (!indices_to_ignore.Contains(cell.GetUnitIndex(base_set_type)) && cell.Candidates.Contains(value))
            {
                cell.Candidates.Remove(value);
                removed_candidates = true;

                if (verbose)
                    Console.WriteLine($"Removing {value} from cell {cell.Index} ({cell.Row}, {cell.Column})");
            }
        }

        return removed_candidates;
    }

    // Check if two candidates form an x-wing pattern
    // ie. the two candidates has the same value, in two different rows, and in the same columns
    // or the two candidates has the same value, in two different columns, and in the same rows
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

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
