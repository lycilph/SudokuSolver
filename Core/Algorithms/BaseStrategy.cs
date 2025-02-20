using Core.Model;

namespace Core.Algorithms;

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

/*
 This implementation comes from the AI "Claude"
  
  
 // Your existing interface
public interface IStrategy
{
    void Execute(/ parameters /);
}

// Generic singleton base class
public abstract class SingletonStrategy<T> : IStrategy where T : SingletonStrategy<T>, new()
{
    private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

    public static T Instance => _instance.Value;

    public static void Execute(/ parameters /)
    {
        Instance.Execute(/ parameters /);
    }

    // Implementation of IStrategy.Execute
    public abstract void Execute(/ parameters /);
}


public class ConcreteStrategyA : SingletonStrategy<ConcreteStrategyA>
{
    public override void Execute(/ parameters /)
    {
        // Implementation for strategy A
    }
}

public class ConcreteStrategyB : SingletonStrategy<ConcreteStrategyB>
{
    public override void Execute(/ parameters /)
    {
        // Implementation for strategy B
    }
}

// Call static method (uses singleton internally)
ConcreteStrategyA.Execute(/ parameters /);

// Or use the instance directly if needed
var instance = ConcreteStrategyA.Instance;
instance.Execute(/ parameters /);
*/