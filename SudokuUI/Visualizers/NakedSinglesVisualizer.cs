using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class NakedSinglesVisualizer : IStrategyVisualizer
{
    public void Show(GridViewModel vm, BaseCommand command)
    {
        foreach (var element in command.Elements)
        {
            var cell_vm = vm.Map(element.Cell);
            var candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = Brushes.ForestGreen;
            candidate_vm.Highlight = true;
        }
    }
}
