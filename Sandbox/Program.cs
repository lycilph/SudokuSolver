using Core.Engine;
using Core.Extensions;
using Core.Models;
using Core.Serialization;
using Core.Strategies;
using System.Text.Json;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        //var grid = new Grid().Load(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4", true);
        //BasicEliminationStrategy.PlanAndExecute(grid);
        //grid.Serialize("test.txt");

        var grid = new Grid().Load("test.txt");
        Console.WriteLine(grid.ToString());
        Console.WriteLine(grid.ToSimpleString());
        Console.WriteLine(grid.CandidatesToString());
        Console.WriteLine($"Candidates count: {grid.EmptyCells().SelectMany(x => x.Candidates).Count()}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private static void GridSerialization()
    {
        var grid = new Grid().Load(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4", true);
        BasicEliminationStrategy.PlanAndExecute(grid);

        Console.WriteLine(grid.ToString());
        Console.WriteLine(grid.ToSimpleString());
        Console.WriteLine(grid.CandidatesToString());
        Console.WriteLine($"Candidates count: {grid.EmptyCells().SelectMany(x => x.Candidates).Count()}");

        var list = grid.Cells.Select(c => new CellDTO(c)).ToList();
        var json = JsonSerializer.Serialize(list);
        Console.WriteLine(json);

        grid.Reset();
        Console.WriteLine(grid.ToSimpleString());
        Console.WriteLine(grid.CandidatesToString());
        Console.WriteLine($"Candidates count: {grid.EmptyCells().SelectMany(x => x.Candidates).Count()}");

        var list2 = JsonSerializer.Deserialize<List<CellDTO>>(json) ?? [];
        for (int i = 0; i < list2.Count; i++)
        {
            grid[i].Value = list2[i].Value;
            grid[i].IsClue = list[i].IsClue;
            grid[i].Candidates.AddRange(list2[i].Candidates);
        }
        Console.WriteLine(grid.ToSimpleString());
        Console.WriteLine(grid.CandidatesToString());
        Console.WriteLine($"Candidates count: {grid.EmptyCells().SelectMany(x => x.Candidates).Count()}");
    }
}
