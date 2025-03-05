using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class CandidateViewModel : ObservableObject
{
    [ObservableProperty]
    private int value;

    [ObservableProperty]
    private bool isVisible;

    public CandidateViewModel(int value)
    {
        Value = value;
        IsVisible = true;
    }
}
