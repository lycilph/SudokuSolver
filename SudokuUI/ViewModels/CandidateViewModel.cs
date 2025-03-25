using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;

namespace SudokuUI.ViewModels;

public partial class CandidateViewModel : ObservableObject
{
    private readonly Cell cell;

    [ObservableProperty]
    private int value;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private bool highlight = false;

    [ObservableProperty]
    private Brush highlightColor = Brushes.CornflowerBlue;

    public CandidateViewModel(Cell cell, int value)
    {
        this.cell = cell;

        Value = value;
        IsVisible = true;

        HighlightColor = App.Current.Resources["cell_highlight_color"] as Brush ?? Brushes.Black;
    }

    public bool CellHasCandidate() => cell.Contains(Value);
}
