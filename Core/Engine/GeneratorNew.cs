using Core.DancingLinks;
using Core.Extensions;
using Core.Models;
using Core.Strategies;
using System.Diagnostics;

namespace Core.Engine;

public static class GeneratorNew
{
    public static (Grid?, Grade) Generate(int min_difficult = 0, int max_difficulty = 11, int final_clues = 25, int max_grid_generation_tries = 25, int max_cell_removal_tries = 25)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        int grid_generation_tries = 0;
        Grid? grid = null;
        Grade grade = null!;
        while (grid_generation_tries < max_grid_generation_tries)
        {
            grid_generation_tries++;
            (grid, grade) = GenerateGrid(max_difficulty, final_clues, max_cell_removal_tries);

            if (grade.Difficulty >= min_difficult && grade.Difficulty <= max_cell_removal_tries)
                break;
        }

        if (grid_generation_tries >= max_grid_generation_tries &&
            (grade.Difficulty < min_difficult || grade.Difficulty > max_difficulty))
        {
            Console.WriteLine("Couldn't generate a grid with the requested difficulty");
            grid = null;
        }

        stopwatch.Stop();
        Console.WriteLine($"Total elapsed time (after {grid_generation_tries} tries): {stopwatch.ElapsedMilliseconds} ms");
        
        return (grid, grade);
    }

    private static (Grid, Grade) GenerateGrid(int max_difficulty = 11, int final_clues = 25, int max_cell_removal_tries = 25)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var initial_grid_tries = 0;
        Grid? grid = null;
        while (true)
        {
            initial_grid_tries++;
            grid = CreateInitialGrid();
            if (grid != null)
            {
                Console.WriteLine(grid);
                break;
            }
        }
        Console.WriteLine($"Initial grid created after {initial_grid_tries} tries");

        var cells_to_remove = Grid.Size() - final_clues;
        for (int i = 0; i < cells_to_remove; i++)
        {
            var grade = RemoveRandomCell(grid, max_difficulty, max_cell_removal_tries);

            Console.WriteLine($"Current grid has {grid.FilledCells().Count()} clues with a {grade.Difficulty} difficulty");
        }

        stopwatch.Stop();
        Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");

        var final_grade = Grader.Grade(grid);
        return (grid, final_grade);
    }

    private static Grade RemoveRandomCell(Grid grid, int max_difficulty, int max_cell_removal_tries)
    {
        int cell_removal_tries = 0;
        var grade = new Grade(0,0);
        while (cell_removal_tries < max_cell_removal_tries)
        {
            cell_removal_tries++;

            // Clear a random cell (remember the value if it needs to be restored)
            var cell = grid.FilledCells().OrderBy(x => Random.Shared.Next()).First();
            var value = cell.Value;
            cell.Value = 0;

            // The empty cells must also be reset again (otherwise it "remembers" the full solution)
            grid.EmptyCells().ForEach(c =>
            {
                c.Reset();
                c.FillCandidates();
            });

            (var results, _) = DancingLinksSolver.Solve(grid, true, 10);

                //  There is still only 1 solution, so lets check its difficulty
            if (results.Count == 1)
            {
                grade = Grader.Grade(grid);
                if (grade.Difficulty <= max_difficulty)
                    return grade;
            }

            // Restore the cell, since it left the grid invalid or too difficult
            cell.Value = value;
            cell.Clear();
            Console.WriteLine($"Oops, grid is no longer valid OR too difficult - {results.Count} solutions found - difficulty is {grade.Difficulty} (iteration {cell_removal_tries})");
        }

        return grade;
    }

    private static Grid? CreateInitialGrid()
    {
        var grid = new Grid();
        grid.FillCandidates();

        foreach (var b in grid.Boxes.OrderBy(x => Random.Shared.Next()).ToList())
        {
            var cell = b.Cells.OrderBy(x => Random.Shared.Next()).First();
            var candidate = cell.Candidates.OrderBy(x => Random.Shared.Next()).First();

            cell.Value = candidate;
            cell.Candidates.Clear();
            BasicEliminationStrategy.PlanAndExecute(grid);
        }

        (var solutions, _) = DancingLinksSolver.Solve(grid.Copy(true), true, 10);
        if (solutions.Count > 0)
            return solutions[Random.Shared.Next(0, solutions.Count)];
        else
            return null;
    }
}
