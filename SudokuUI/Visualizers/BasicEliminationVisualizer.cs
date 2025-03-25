using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class BasicEliminationVisualizer : IStrategyVisualizer
{
    private readonly Brush color;

    public BasicEliminationVisualizer()
    {
        color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, BaseCommand command)
    {
        foreach (var element in command.Elements)
        {
            foreach (var cell in element.Cells)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = color;
                candidate_vm.Highlight = true;
            }
        }
    }
}
