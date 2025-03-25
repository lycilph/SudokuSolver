using Core.Commands;
using Core.Models;
using Core.Strategies;
using SudokuUI.Visualizers;

namespace Sandbox;

internal class Program
{
    static void Main()
    {
        var g = new Grid();

        Console.WriteLine("Assemblies loaded:");
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
            Console.WriteLine(assembly.FullName);
        Console.WriteLine();

        Console.WriteLine("Strategies found:");
        var strategy_interface = typeof(IStrategy);
        var strategies = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                strategy_interface.IsAssignableFrom(type))
            .ToList();
        foreach (var strategy in strategies)
            Console.WriteLine(strategy.FullName);
        Console.WriteLine();

        Console.WriteLine("Visualizers found:");
        var visualizer_interface = typeof(IStrategyVisualizer);
        var visualizers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                visualizer_interface.IsAssignableFrom(type))
            .ToList();
        foreach (var visualizer in visualizers)
            Console.WriteLine(visualizer.FullName);
        Console.WriteLine();

        var known_strategies = new List<IStrategy>();
        foreach (var strategy in strategies)
        {
            if (Activator.CreateInstance(strategy) is IStrategy strat)
                known_strategies.Add(strat);
        }
        known_strategies.ForEach(s => Console.WriteLine(s.Name));

        BaseCommand cmd = new BasicEliminationCommand("Basic Elimination");
        IStrategyVisualizer vis = new BasicEliminationVisualizer();
        vis.Show(null!, cmd);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
