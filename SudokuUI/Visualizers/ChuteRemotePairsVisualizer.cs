using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class ChuteRemotePairsVisualizer : IStrategyVisualizer<ChuteRemotePairsCommand>
{
    private readonly Brush remote_pair_color;
    private readonly Brush eliminated_candidates_color;
    private readonly Brush box_color;

    public ChuteRemotePairsVisualizer()
    {
        remote_pair_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        box_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, ChuteRemotePairsCommand command)
    {
        foreach (var element in command.Elements)
            Show(vm, element);
    }

    public void Show(GridViewModel vm, CommandElement element)
    {
        // Mark the pair here
        foreach (var cell in element.CellsToVisualize.Take(2))
        {
            var cell_vm = vm.Map(cell);
            foreach (var candidate_vm in cell_vm.Candidates)
            {
                if (candidate_vm.IsVisible && element.NumbersToVisualize.Contains(candidate_vm.Value))
                {
                    candidate_vm.HighlightColor = remote_pair_color;
                    candidate_vm.Highlight = true;
                }
            }
        }

        // Mark the last box cells here
        foreach (var cell in element.CellsToVisualize.Skip(2))
        {
            var cell_vm = vm.Map(cell);
            cell_vm.BackgroundColor = box_color;
        }

        // Mark eliminations here
        foreach (var cell in element.Cells)
        {
            var cell_vm = vm.Map(cell);
            var candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = eliminated_candidates_color;
            candidate_vm.Highlight = true;
        }
    }
}
