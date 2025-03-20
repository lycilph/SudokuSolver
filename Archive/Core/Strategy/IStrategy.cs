using Core.Model;
using Core.Model.Actions;

namespace Core.Strategy;

public interface IStrategy
{
    string Name { get; }

    IPuzzleAction? Execute(Grid grid);
}
