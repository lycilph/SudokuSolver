using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class NakedPairsVisualizer : IStrategyVisualizer<NakedPairsCommand>
{
    private readonly Brush eliminated_color;
    private readonly Brush pair_color;

    public NakedPairsVisualizer()
    {
        eliminated_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        pair_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, NakedPairsCommand command)
    {
        foreach (var element in command.Elements)
        {
            // These are the pair cells, which eliminate candidates in the other boxes
            foreach (var cell in element.CellsToVisualize)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = pair_color;
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
