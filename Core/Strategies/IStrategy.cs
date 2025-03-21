using Core.Commands;
using Core.Models;

namespace Core.Strategies;

public interface IStrategy
{
    public string Name { get; }

    public ICommand? Plan(Grid grid);
}
