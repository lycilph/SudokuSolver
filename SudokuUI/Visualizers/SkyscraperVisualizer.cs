using System.Windows.Media;
using Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers.Misc;

namespace SudokuUI.Visualizers;

public class SkyscraperVisualizer : IStrategyVisualizer<SkyscraperCommand>
{
    private readonly Brush skyscraper_color;
    private readonly Brush eliminated_candidates_color;
    private readonly Brush overlap_color;
    private readonly Brush strong_link_color;
    private readonly Brush weak_link_color;
    private readonly VisualizationService visualizer;

    public SkyscraperVisualizer()
    {
        skyscraper_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        eliminated_candidates_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        overlap_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
        strong_link_color = App.Current.Resources["strong_link_color"] as Brush ?? Brushes.Black;
        weak_link_color = App.Current.Resources["weak_link_color"] as Brush ?? Brushes.Black;

        visualizer = App.Current.Services.GetRequiredService<VisualizationService>();
    }

    public void Show(GridViewModel vm, SkyscraperCommand command)
    {
        foreach (var element in command.Elements)
        {
            // Skyscraper links
            var link1 = element.LinksToVisualize[0];
            var link2 = element.LinksToVisualize[1];
            var base_link = element.LinksToVisualize[2];
            visualizer.Add(link1, strong_link_color);
            visualizer.Add(link2, strong_link_color);
            visualizer.Add(base_link, weak_link_color, LinkVisualizer.LineType.Dotted);

            // Mark skyscraper here
            foreach (var cell in link1.Cells.Concat(link2.Cells))
            {
                var cell_vm = vm.Map(cell);
                var candidate_vm = cell_vm.Candidates[element.Number - 1];

                candidate_vm.HighlightColor = skyscraper_color;
                candidate_vm.Highlight = true;
            }

            // Mark the overlap cells here
            foreach (var cell in element.CellsToVisualize)
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
