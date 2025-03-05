using Core.Model;

namespace Core.Strategies;

// This implementation comes from the AI "Claude"

public abstract class BaseStrategy<T> : IStrategy where T : BaseStrategy<T>, IStrategy, new()
{
    private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
    
    public static T Instance => _instance.Value;

    public abstract string Name { get; }

    public abstract ISolveAction? Execute(Grid grid);

    public static ISolveAction? ExecuteAndApply(Grid grid)
    {
        var actions = Instance.Execute(grid);
        actions?.Apply();
        return actions;
    }
}