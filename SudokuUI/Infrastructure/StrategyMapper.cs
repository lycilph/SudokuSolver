using Core.Commands;
using Core.Strategies;
using SudokuUI.Visualizers;

namespace SudokuUI.Infrastructure;

public static class StrategyMapper
{
    public static Dictionary<Type, IStrategyVisualizer> GetVisualizerMap(IEnumerable<IStrategy> strategies)
    {
        var commands = GetClassesAssignableFrom(typeof(BaseCommand));
        var visualizers = GetClassesAssignableFrom(typeof(IStrategyVisualizer));
        var map = new Dictionary<Type, IStrategyVisualizer>();

        foreach (var strategy in strategies)
        {
            var base_name = strategy.GetType().Name.Replace("Strategy","");
            var command_type = commands.Where(c => c.Name.Contains(base_name)).Single();
            var visualizer_type = visualizers.Where(v => v.Name.Contains(base_name)).Single();
            var visualizer = Activator.CreateInstance(visualizer_type) as IStrategyVisualizer;

            if (visualizer == null)
                throw new InvalidOperationException($"Type {visualizer_type} couldn't be created");

            map.Add(command_type, visualizer);
        }
        return map;
    }

    private static List<Type> GetClassesAssignableFrom(Type interface_type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                interface_type.IsAssignableFrom(type))
            .ToList();
    }
}
