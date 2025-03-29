using Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers.Misc;
using System.Windows.Media;

namespace SudokuUI.Visualizers;

public class TwoStringKiteVisualizer : IStrategyVisualizer<TwoStringKiteCommand>
{
    private readonly Brush candidate_color;
    private readonly Brush kite_color;
    private readonly VisualizationService visualizer;

    public TwoStringKiteVisualizer()
    {
        candidate_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        kite_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;

        visualizer = App.Current.Services.GetRequiredService<VisualizationService>();
    }

    public void Show(GridViewModel vm, TwoStringKiteCommand command)
    {
        // These are the candidates eliminated by the kite
        foreach (var element in command.Elements)
        {
            var cell_vm = vm.Map(element.Cell);
            var candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = candidate_color;
            candidate_vm.Highlight = true;

            foreach (var cell in element.CellsToVisualize)
            {
                cell_vm = vm.Map(cell);
                candidate_vm = cell_vm.Candidates[element.Number - 1];
                
                candidate_vm.HighlightColor = kite_color;
                candidate_vm.Highlight = true;
            }

            visualizer.Add(element.LinksToVisualize[0], Brushes.Blue);
            visualizer.Add(element.LinksToVisualize[1], Brushes.Blue);
            visualizer.Add(element.LinksToVisualize[2], Brushes.Red, LinkVisualizer.LineType.Dotted);
        }
    }
}
