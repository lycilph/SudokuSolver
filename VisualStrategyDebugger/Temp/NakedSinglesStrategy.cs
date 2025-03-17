using Core.Model;

namespace VisualStrategyDebugger.Temp;

public class NakedSinglesStrategy : IStrategy
{
    public string Name { get; } = "Naked Singles";

    public IGridCommand? Plan(Grid grid)
    {
        return null;
    }
}
