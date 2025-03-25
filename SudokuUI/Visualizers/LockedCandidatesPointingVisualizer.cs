using Core.Commands;
using SudokuUI.ViewModels;
using System.Windows.Media;

namespace SudokuUI.Visualizers;

public class LockedCandidatesPointingVisualizer : IStrategyVisualizer<LockedCandidatesPointingCommand>
{
    private readonly Brush eliminated_color;
    private readonly Brush pointing_color;

    public LockedCandidatesPointingVisualizer()
    {
        eliminated_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        pointing_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, LockedCandidatesPointingCommand command)
    {
        foreach (var element in command.Elements)
        {
            // These are the pointing cells, which eliminate candidates in the other boxes
            foreach (var cell in element.CellsToVisualize)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = pointing_color;
                candidate_vm.Highlight = true;
            }

            foreach (var cell in element.Cells)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = eliminated_color;
                candidate_vm.Highlight = true;
            }
        }
    }
}
