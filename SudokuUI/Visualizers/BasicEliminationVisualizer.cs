using System.Windows.Media;
using Core.Commands;
using Core.Models;
using SudokuUI.ViewModels;

namespace SudokuUI.Visualizers;

public class BasicEliminationVisualizer : IStrategyVisualizer<BasicEliminationCommand>
{
    private readonly Brush candidate_color;
    private readonly Brush cell_color;
    private readonly Brush cell_transparent_color;
    private readonly Brush peer_background_color;

    private List<Cell> cells_to_highlight = [];

    public BasicEliminationVisualizer()
    {
        candidate_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        cell_color = App.Current.Resources["cell_information_color"] as Brush ?? Brushes.Black;
        cell_transparent_color = App.Current.Resources["cell_transparent_color"] as Brush ?? Brushes.Black;
        peer_background_color = App.Current.Resources["cell_peers_background_color"] as Brush ?? Brushes.Black;
    }

    public void Show(GridViewModel vm, BasicEliminationCommand command)
    {
        cells_to_highlight = command.Elements.SelectMany(e => e.CellsToVisualize).ToList();

        foreach (var element in command.Elements)
            Show(vm, element);

        cells_to_highlight.Clear();
    }

    public void Show(GridViewModel vm, CommandElement element)
    {
        // These cells are eliminating candidates in its peers
        foreach (var cell in element.CellsToVisualize)
        {
            var cell_vm = vm.Map(cell);
            cell_vm.HighlightColor = cell_color;
            cell_vm.Highlight = true;

            foreach (var peer in cell.Peers.Except(cells_to_highlight))
            {
                cell_vm = vm.Map(peer);
                cell_vm.BackgroundColor = peer_background_color;
                cell_vm.HighlightColor = cell_transparent_color;
                cell_vm.Highlight = true;
            }
        }

        // These are the candidates eliminated by the cells above
        foreach (var cell in element.Cells)
        {
            var cell_vm = vm.Map(cell);
            var candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = candidate_color;
            candidate_vm.Highlight = true;
        }
    }
}
