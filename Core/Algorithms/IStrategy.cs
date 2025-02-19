using Core.Model;

namespace Core.Algorithms;

public interface IStrategy
{
    string Name { get; }

    ISolveAction? Execute(Grid grid);
}
