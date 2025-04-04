using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class HiddenTriplesVisualizer : IStrategyVisualizer<HiddenTriplesCommand>
{
    private readonly Brush hidden_triple_color;
    private readonly Brush eliminated_candidates_color;

    public HiddenTriplesVisualizer()
    {
        hidden_triple_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, HiddenTriplesCommand command)
    {
        foreach (var element in command.Elements)
            Show(vm, element);
    }

    public void Show(GridViewModel vm, CommandElement element)
    {
        // Mark candidates to eliminate
        var cell_vm = vm.Map(element.Cell);
        foreach (var candidate_vm in cell_vm.Candidates)
        {
            if (candidate_vm.IsVisible && candidate_vm.Value == element.Number)
            {
                candidate_vm.HighlightColor = eliminated_candidates_color;
                candidate_vm.Highlight = true;
            }
        }

        // Mark the triple here
        foreach (var cell in element.CellsToVisualize)
        {
            cell_vm = vm.Map(cell);
            foreach (var candidate_vm in cell_vm.Candidates)
            {
                if (candidate_vm.IsVisible && element.NumbersToVisualize.Contains(candidate_vm.Value))
                {
                    candidate_vm.HighlightColor = hidden_triple_color;
                    candidate_vm.Highlight = true;
                }
            }
        }
    }
}
