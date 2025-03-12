using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class CandidateViewModel : ObservableObject
{
    [ObservableProperty]
    private int value;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private bool highlightNumber = false;

    public CandidateViewModel(int value)
    {
        Value = value;
        IsVisible = true;
    }
}
