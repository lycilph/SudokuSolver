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

    public CandidateViewModel(Cell cell, int value)
    {
        this.cell = cell;

        Value = value;
        IsVisible = true;
    }

    public bool CellHasCandidate() => cell.Contains(Value);
}
