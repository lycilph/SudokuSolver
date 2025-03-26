using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class NakedQuadsVisualizer : IStrategyVisualizer<NakedQuadsCommand>
{
    private readonly Brush eliminated_color;
    private readonly Brush quad_color;

    public NakedQuadsVisualizer()
    {
        eliminated_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        quad_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, NakedQuadsCommand command)
    {
        foreach (var element in command.Elements)
        {
            // These are the triple cells, which eliminate candidates in the other boxes
            foreach (var cell in element.CellsToVisualize)
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];
                if (candidate_vm.IsVisible)
                {
                    candidate_vm.HighlightColor = quad_color;
                    candidate_vm.Highlight = true;
                }
                
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
