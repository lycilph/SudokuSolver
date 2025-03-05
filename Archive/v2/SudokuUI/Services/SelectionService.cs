using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;

namespace SudokuUI.Services;

public partial class SelectionService : ObservableObject
{
    public enum Mode { Digits, Hints };

    [ObservableProperty]
    private Mode inputMode = Mode.Digits;

    [ObservableProperty]
    private int digit = -1; // -1 is no select, 0 is empty, 

    [ObservableProperty]
    private Cell? cell = null;

    public void ClearDigitSelection() => Digit = -1;

    public void ToggleInputMode() => InputMode = (InputMode == Mode.Digits ? Mode.Hints : Mode.Digits);
}
