using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public interface IStrategyVisualizer
{
    void Show(GridViewModel vm, BaseCommand command);
    void Show(GridViewModel vm, CommandElement element);
}

public interface IStrategyVisualizer<T> : IStrategyVisualizer where T : BaseCommand
{
    void Show(GridViewModel vm, T command);

    // Implement the base interface explicitly
    void IStrategyVisualizer.Show(GridViewModel vm, BaseCommand command) => Show(vm, (T)command);
}