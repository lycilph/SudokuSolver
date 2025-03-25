using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class NakedSinglesVisualizer : IStrategyVisualizer<NakedSinglesCommand>
{
    private readonly Brush color;

    public NakedSinglesVisualizer()
    {
        color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, NakedSinglesCommand command)
    {
        foreach (var element in command.Elements)
        {
            var cell_vm = vm.Map(element.Cell);
            var candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = color;
            candidate_vm.Highlight = true;
        }
    }
}
