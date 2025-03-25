using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public interface IStrategyVisualizer
{
    void Show(GridViewModel vm, BaseCommand command);
}
