using Core.Model;

namespace Core.Strategies;

public interface IStrategy
{
    string Name { get; }

    ISolveAction? Execute(Grid grid);
}
