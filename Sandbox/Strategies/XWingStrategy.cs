using Sandbox.Model;

namespace Sandbox.Strategies;

public class XWingStrategy : IStrategy
{
    public string Name => "X-Wing";

    public static readonly XWingStrategy Instance = new();

    public bool Step(Grid grid, bool verbose = true)
    {
        var found_xwing = false;

        if (FindXWingsRowMajor(grid))
            found_xwing = true;

        if (FindXWingsColumnMajor(grid))
            found_xwing = true;

        return found_xwing;
    }

    private bool FindXWingsRowMajor(Grid grid)
    {
        var found_xwing = false;

        /// Tuple where:
        /// - Item1 is possible value
        /// - Item2 is first cell containing possible value (ie. item1)
        /// - Item3 is second cell containing possible value (ie. item1)
        var candidates = new List<Tuple<int, Cell, Cell>>();

        foreach (var value in Grid.PossibleValues)
        {
            foreach (var row in grid.Rows)
            {
                var pair_candidates = row.Cells.Where(c => c.Candidates.Contains(value)).ToArray();
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
                    Console.WriteLine("Found x-wing (row-major)");
                    Console.WriteLine($"Found a pair-candidate on {candidates[i].Item1} in row {candidates[i].Item2.Row} (columns: {candidates[i].Item2.Column} and {candidates[i].Item3.Column})");
                    Console.WriteLine($"Found a pair-candidate on {candidates[j].Item1} in row {candidates[j].Item2.Row} (columns: {candidates[j].Item2.Column} and {candidates[j].Item3.Column})");

                    if (EliminateCandidatesFromColumns(candidates[i].Item1,
                        [grid.Columns[candidates[i].Item2.Column], grid.Columns[candidates[i].Item3.Column]],
                        [candidates[i].Item2.Row, candidates[j].Item2.Row]))
                        found_xwing = true;
                }
            }
        }

        return found_xwing;
    }

    private bool FindXWingsColumnMajor(Grid grid)
    {
        var found_xwing = false;

        /// Tuple where:
        /// - Item1 is possible value
        /// - Item2 is first cell containing possible value (ie. item1)
        /// - Item3 is second cell containing possible value (ie. item1)
        var candidates = new List<Tuple<int, Cell, Cell>>();

        foreach (var value in Grid.PossibleValues)
        {
            foreach (var col in grid.Columns)
            {
                var pair_candidates = col.Cells.Where(c => c.Candidates.Contains(value)).ToArray();
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
                    Console.WriteLine("Found x-wing (column-major)");
                    Console.WriteLine($"Found a pair-candidate on {candidates[i].Item1} in column {candidates[i].Item2.Column} (rows: {candidates[i].Item2.Row} and {candidates[i].Item3.Row})");
                    Console.WriteLine($"Found a pair-candidate on {candidates[j].Item1} in column {candidates[j].Item2.Column} (rows: {candidates[j].Item2.Row} and {candidates[j].Item3.Row})");

                    if (EliminateCandidatesFromRows(candidates[i].Item1,
                        [grid.Columns[candidates[i].Item2.Row], grid.Columns[candidates[i].Item3.Row]],
                        [candidates[i].Item2.Column, candidates[j].Item2.Column]))
                        found_xwing = true;
                }
            }
        }

        return found_xwing;
    }

    private bool EliminateCandidatesFromColumns(int value, Unit[] columns, int[] rows_to_exclude)
    {
        var removed_candidates = false;

        foreach (var cell in columns.SelectMany(c => c.Cells))
        {
            if (!rows_to_exclude.Contains(cell.Row) && cell.Candidates.Contains(value))
            {
                cell.Candidates.Remove(value);
                removed_candidates = true;
                Console.WriteLine($"Removing {value} from cell {cell.Index} ({cell.Row}, {cell.Column})");
            }
        }

        return removed_candidates;
    }

    private bool EliminateCandidatesFromRows(int value, Unit[] rows, int[] columns_to_exclude)
    {
        var removed_candidates = false;

        foreach (var cell in rows.SelectMany(c => c.Cells))
        {
            if (!columns_to_exclude.Contains(cell.Column) && cell.Candidates.Contains(value))
            {
                cell.Candidates.Remove(value);
                removed_candidates = true;
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
            ((candidate1.Item2.Row != candidate2.Item2.Row &&
             candidate1.Item2.Column == candidate2.Item2.Column &&
             candidate1.Item3.Column == candidate2.Item3.Column)
             ||
             (candidate1.Item2.Column != candidate2.Item2.Column &&
             candidate1.Item2.Row == candidate2.Item2.Row &&
             candidate1.Item3.Row == candidate2.Item3.Row));
    }

    public static bool Execute(Grid grid)
    {
        return Instance.Step(grid);
    }
}
