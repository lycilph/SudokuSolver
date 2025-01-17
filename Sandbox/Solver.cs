using Sandbox.Model;
using System.Diagnostics;

namespace Sandbox;

/*
 * 1. First start by basic elimination of candidates
 * 2. Then find naked singles
 * 3. Then find hidden singles
 * 4. Bruteforce the rest
 */

public static class Solver
{
    public static void Solve(Grid grid)
    {
        var iterations = 0;
        var last_empty_cells_count = 0;
        var empty_cells_count = grid.EmptyCellsCount();

        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        while (!grid.IsSolved() && empty_cells_count != last_empty_cells_count)
        {
            iterations++;
            Step(grid);
            last_empty_cells_count = empty_cells_count;
            empty_cells_count = grid.EmptyCellsCount();
            Console.WriteLine($"Iteration {iterations}: {grid.EmptyCellsCount()} empty cells left");
        }

        if (grid.IsSolved())
        {
            Console.WriteLine("Sudoku solved!");
        }
        else
        {
            Console.WriteLine("Sudoku not solved. Bruteforcing the rest...");
            Backtrack(grid);
        }

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    public static void Step(Grid grid)
    {
        BasicElimination(grid);

        if (NakedSingles(grid) > 0)
            return;

        HiddenSingles(grid);
    }

    // Eliminate the cell value from the candidates of its peers
    public static void BasicElimination(Grid grid)
    {
        foreach (var cell in grid.Cells.Where(c => c.HasValue))
        {
            foreach (var peer in cell.Peers.Where(p => p.IsEmpty))
            {
                peer.Candidates.Remove(cell.Value);
            }
        }
    }

    // Find cells with only one candidate, and set the value for that cell
    public static int NakedSingles(Grid grid)
    {
        var cells = grid.Cells.Where(c => c.IsEmpty && c.Candidates.Count == 1).ToArray();
        foreach (var cell in cells)
        {
            cell.Value = cell.Candidates.First();
            Console.WriteLine($"Naked single found in cell {cell.Index}");
        }
        return cells.Length;
    }

    // Find cells were 2 cells share the same 2 candidates, and remove these candidates from the other cells in the unit
    public static int NakedPairs(Grid grid)
    {
        int count = 0;
        int unit_count = 0;

        foreach (var row in grid.Rows)
        {
            unit_count = NakedPairsInUnit(row.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Naked pair found in row {row.Index}");
            count += unit_count;
        }

        foreach (var col in grid.Columns)
        {
            unit_count = NakedPairsInUnit(col.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Naked pair found in column {col.Index}");
            count += unit_count;
        }

        foreach (var box in grid.Boxes)
        {
            unit_count = NakedPairsInUnit(box.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Naked pair found in box {box.Index}");
            count += unit_count;
        }

        return count;
    }

    private static int NakedPairsInUnit(Cell[] cells)
    {
        int count = 0;

        var empty_cells = cells.Where(c => c.IsEmpty).ToArray();
        var pair_candidates = cells.Where(c => c.CandidatesCount == 2).ToArray();

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
                            Console.WriteLine($"Naked pair ({string.Join(',',candidate1.Candidates)}) overlaps with cell {cell.Index}");
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }

    // Find cells where a value is only present as a candidate in one cell in a unit
    public static int HiddenSingles(Grid grid)
    {
        int count = 0;
        int unit_count = 0;

        foreach (var row in grid.Rows)
        {
            unit_count = HiddenSinglesInUnit(row.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in row {row.Index}");
            count += unit_count;
        }

        foreach (var col in grid.Columns)
        {
            unit_count = HiddenSinglesInUnit(col.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in column {col.Index}");
            count += unit_count;
        }

        foreach (var box in grid.Boxes)
        {
            unit_count = HiddenSinglesInUnit(box.Cells);
            if (unit_count > 0)
                Console.WriteLine($"Hidden single found in box {box.Index}");
            count += unit_count;
        }

        return count;
    }

    private static int HiddenSinglesInUnit(Cell[] cells)
    {
        int count = 0;

        var emptyCells = cells.Where(c => c.IsEmpty).ToArray();
        var possibleValues = Enumerable.Range(1, 9).Except(cells.Where(c => c.HasValue).Select(c => c.Value)).ToArray();

        foreach (var value in possibleValues)
        {
            var candidates = emptyCells.Where(c => c.Candidates.Contains(value)).ToArray();
            if (candidates.Length == 1)
            {
                candidates[0].Value = value;
                Console.WriteLine($"Found hidden single for cell {candidates[0].Index} with the value {value}");
                count++;
            }
        }

        return count;
    }

    // Solved grid by brute force through backtracking
    public static void Backtrack(Grid grid)
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        BasicElimination(grid);

        if (SolveBacktrack(grid))
            Console.WriteLine("Sudoku solved by backtracking!");
        else
            Console.WriteLine("Sudoku not solved by backtracking!");

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
    }

    private static bool SolveBacktrack(Grid grid)
    {
        if (grid.IsSolved())
            return true;

        var empty_cells = grid.Cells.Where(c => c.IsEmpty).OrderBy(c => c.CandidatesCount).ToArray();

        foreach (var cell in empty_cells)
        {
            var possible_values = cell.Candidates.ToArray();

            foreach (var value in possible_values)
            {
                // Check if value is used in any of the cells peers
                var is_valid = cell.Peers.All(p => p.Value != value);
                if (!is_valid)
                    continue;

                cell.Value = value;
                if (SolveBacktrack(grid))
                {
                    return true;
                }
                else
                {
                    cell.Value = 0;
                    cell.Candidates.UnionWith(possible_values);
                }
            }
            return false;
        }

        return false;
    }
}
