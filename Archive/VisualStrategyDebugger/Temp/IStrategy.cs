using Core.Model;

namespace VisualStrategyDebugger.Temp;

public interface IStrategy
{
    public string Name { get; }

    public IGridCommand? Plan(Grid grid);
}
