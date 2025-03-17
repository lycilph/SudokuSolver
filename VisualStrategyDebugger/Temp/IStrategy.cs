using Core.Model;

namespace VisualStrategyDebugger.Temp;

public interface IStrategy
{
    string Name { get; }

    IGridCommand? Plan(Grid grid);
}
