using Sandbox.Model;

namespace Sandbox.Strategies;

public interface IStrategy
{
    string Name { get; }

    // Returns true if the strategy made a change to the grid
    bool Step(Grid grid, bool verbose = true);
}
