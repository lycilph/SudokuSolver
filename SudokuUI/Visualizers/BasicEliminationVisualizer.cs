using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class BasicEliminationVisualizer : IStrategyVisualizer
{
    public void Show(GridViewModel vm, BaseCommand command)
    {
        foreach (var element in command.Elements)
        {
            foreach (var cell in element.Cells)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = Brushes.IndianRed;
                candidate_vm.Highlight = true;
            }
        }
    }
}
