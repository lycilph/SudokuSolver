using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;

namespace SudokuUI.ViewModels;

public partial class CandidateViewModel : ObservableObject
{
    private readonly Brush default_highlight_color;
    private readonly Cell cell;

    [ObservableProperty]
    private int value;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private bool highlight = false;

    [ObservableProperty]
    private Brush highlightColor = Brushes.Black;

    public CandidateViewModel(Cell cell, int value)
    {
        this.cell = cell;

        Value = value;
        IsVisible = true;

        default_highlight_color = App.Current.Resources["cell_highlight_color"] as Brush ?? Brushes.Black;

        ResetVisuals();
    }

    public bool CellHasCandidate() => cell.Contains(Value);
    
    public void ResetVisuals()
    {
        Highlight = false;
        HighlightColor = default_highlight_color;
    }
}
