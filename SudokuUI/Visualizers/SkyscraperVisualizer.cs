using System.Windows.Media;
using Core.Commands;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class SkyscraperVisualizer : IStrategyVisualizer<SkyscraperCommand>
{
    private readonly Brush skyscraper_color;
    private readonly Brush eliminated_candidates_color;
    private readonly Brush overlap_color;

    public SkyscraperVisualizer()
    {
        skyscraper_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        overlap_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, SkyscraperCommand command)
    {
        foreach (var element in command.Elements)
        {
            // Mark skyscraper here
            foreach (var cell in element.CellsToVisualize.Take(4))
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];
                
                candidate_vm.HighlightColor = skyscraper_color;
                candidate_vm.Highlight = true;
            }

            // Mark the overlap cells here
            foreach (var cell in element.CellsToVisualize.Skip(4))
            {
                var cell_vm = vm.Map(cell);
                cell_vm.BackgroundColor = overlap_color;
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
}
