﻿using Core.Commands;
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
    private readonly Brush strong_link_color;
    private readonly Brush weak_link_color;
    private readonly HighlightService service;

    public TwoStringKiteVisualizer()
    {
        candidate_color = App.Current.Resources["cell_negative_color"] as Brush ?? Brushes.Black;
        kite_color = App.Current.Resources["cell_positive_color"] as Brush ?? Brushes.Black;
        strong_link_color = App.Current.Resources["strong_link_color"] as Brush ?? Brushes.Black;
        weak_link_color = App.Current.Resources["weak_link_color"] as Brush ?? Brushes.Black;

        service = App.Current.Services.GetRequiredService<HighlightService>();
    }

    public void Show(GridViewModel vm, TwoStringKiteCommand command)
    {
        
        foreach (var element in command.Elements)
            Show(vm, element);
    }

    public void Show(GridViewModel vm, CommandElement element)
    {
        var cell_vm = vm.Map(element.Cell);
        var candidate_vm = cell_vm.Candidates[element.Number - 1];

        // These are the candidates eliminated by the kite
        candidate_vm.HighlightColor = candidate_color;
        candidate_vm.Highlight = true;

        foreach (var cell in element.CellsToVisualize)
        {
            cell_vm = vm.Map(cell);
            candidate_vm = cell_vm.Candidates[element.Number - 1];

            candidate_vm.HighlightColor = kite_color;
            candidate_vm.Highlight = true;
        }

        service.Add(element.LinksToVisualize[0], strong_link_color);
        service.Add(element.LinksToVisualize[1], strong_link_color);
        service.Add(element.LinksToVisualize[2], weak_link_color, LinkVisualizer.LineType.Dotted);
    }
}
