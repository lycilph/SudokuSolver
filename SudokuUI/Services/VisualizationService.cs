using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;
using SudokuUI.Visualizers.Misc;

namespace SudokuUI.Services;

public partial class VisualizationService : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<LinkVisualizer> links = [];

    public void Add(Cell start, Cell end, Brush color, LinkVisualizer.LineType line_type = LinkVisualizer.LineType.Solid)
    {
        Links.Add(new LinkVisualizer(start, end, color, line_type));
    }

    public void Add(Link link, Brush color, LinkVisualizer.LineType line_type = LinkVisualizer.LineType.Solid)
    {
        Links.Add(new LinkVisualizer(link.Start, link.End, color, line_type));
    }

    public void Clear() => Links.Clear();
}
