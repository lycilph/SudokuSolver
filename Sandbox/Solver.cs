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
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        int count = 0;
        int iterations = 0;
        while (!grid.IsSolved())
        {
            BasicElimination(grid);
            count = NakedSingles(grid);
            Console.WriteLine($"Found {count} naked singles");
            iterations++;

            if (count == 0)
                break;
        }

        stopwatch.Stop(); // Stop the stopwatch
        Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"{iterations} iterations run");
    }

    // Eliminate the cell value from the candidates of its peers
    public static void BasicElimination(Grid grid)
    {
        for (int cell = 0; cell < 81; cell++)
        {
            var p = grid.Peers[cell];
            var v = grid.Values[cell];
            for (int i = 0; i < p.Length; i++)
            {
                grid.Candidates[p[i]] &= ~(1 << (v - 1));
            }
        }
    }

    // Find cells with only one candidate, and set the value for that cell
    public static int NakedSingles(Grid grid)
    {
        int count = 0;
        for (int cell = 0; cell < 81; cell++)
        {
            if (grid.Values[cell] == 0)
            {
                var c = grid.Candidates[cell];
                if (grid.CandidateCount(cell) == 1)
                {   
                    grid.SetValue(cell, Grid.CandidateToValue(c));
                    count++;
                }
            }
        }
        return count;
    }
}
