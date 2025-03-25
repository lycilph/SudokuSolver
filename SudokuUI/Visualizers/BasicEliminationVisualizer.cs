using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class BasicEliminationVisualizer : IStrategyVisualizer
{
    private readonly Brush candidate_color;
    private readonly Brush cell_color;

    public BasicEliminationVisualizer()
    {
        candidate_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        cell_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, BaseCommand command)
    {
        foreach (var element in command.Elements)
        {
            foreach (var cell in element.Cells)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = candidate_color;
                candidate_vm.Highlight = true;
            }
        }

        if (command is BasicEliminationCommand cmd)
        {
            foreach (var cell in cmd.CellsToVisualize)
            {
                var cell_vm = vm.Map(cell);
                cell_vm.HighlightColor = cell_color;
                cell_vm.Highlight = true;
            }
        }
    }
}
