using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class HiddenPairsVisualizer : IStrategyVisualizer<HiddenPairsCommand>
{
    private readonly Brush hidden_single_color;
    private readonly Brush eliminated_candidates_color;

    public HiddenPairsVisualizer()
    {
        hidden_single_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, HiddenPairsCommand command)
    {
        foreach (var element in command.Elements)
        {
            var cell_vm = vm.Map(element.Cell);
            foreach (var candidate_vm in cell_vm.Candidates)
            {
                if (candidate_vm.IsVisible)
                {
                    candidate_vm.HighlightColor = element.NumbersToVisualize.Contains(candidate_vm.Value) ? hidden_single_color : eliminated_candidates_color;
                    candidate_vm.Highlight = true;
                }
            }
        }
    }
}
