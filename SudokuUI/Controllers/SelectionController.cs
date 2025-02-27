using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;

namespace SudokuUI.Controllers;

public partial class SelectionController : ObservableObject
{
    public enum Mode { Digits, Hints };

    [ObservableProperty]
    private Mode inputMode = Mode.Digits;

    [ObservableProperty]
    private int digitSelected = 0;

    [ObservableProperty]
    private Cell? cellSelected = null;

    public void ClearDigitSelection() => DigitSelected = 0;

    public void ToggleInputMode() => InputMode = (InputMode == Mode.Digits ? Mode.Hints : Mode.Digits);
}
