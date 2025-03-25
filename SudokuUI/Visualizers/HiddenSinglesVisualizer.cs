using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class HiddenSinglesVisualizer : IStrategyVisualizer<HiddenSinglesCommand>
{
    private readonly Brush hidden_single_color;
    private readonly Brush eliminated_candidates_color;

    public HiddenSinglesVisualizer()
    {
        hidden_single_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;

    }

    public void Show(GridViewModel vm, HiddenSinglesCommand command)
    {
        foreach (var element in command.Elements)
        {
            var cell_vm = vm.Map(element.Cell);
            foreach (var candidate_vm in cell_vm.Candidates)
            {
                if (candidate_vm.IsVisible)
                {
                    candidate_vm.HighlightColor = candidate_vm.Value == element.Number ? hidden_single_color : eliminated_candidates_color;
                    candidate_vm.Highlight = true;
                }
            }
        }
    }
}
