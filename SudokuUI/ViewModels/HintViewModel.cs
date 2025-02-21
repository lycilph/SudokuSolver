using CommunityToolkit.Mvvm.ComponentModel;

namespace SudokuUI.ViewModels;

public partial class HintViewModel : ObservableObject
{
    [ObservableProperty]
    private int _digit;

    public HintViewModel(int digit)
    {
        _digit = digit;
    }
}
