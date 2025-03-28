using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SudokuUI.Visualizers.Graphics;

namespace SudokuUI.Services;

public partial class VisualizationService : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Link> links = [];

    public void Clear() => Links.Clear();
}
