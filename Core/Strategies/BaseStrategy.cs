using Core.Commands;
using Core.Models;

namespace Core.Strategies;

public abstract class BaseStrategy<T> : IStrategy where T : BaseStrategy<T>, IStrategy, new()
{
    private static readonly Lazy<T> instance = new Lazy<T>(() => new T());
    public static T Instance => instance.Value;

    public abstract string Name { get; }

    public abstract ICommand? Plan(Grid grid);

    public static ICommand? PlanAndExecute(Grid grid)
    {
        var command = Instance.Plan(grid);
        command?.Do();
        return command;
    }
}
