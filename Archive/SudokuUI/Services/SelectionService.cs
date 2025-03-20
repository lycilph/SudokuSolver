using CommunityToolkit.Mvvm.ComponentModel;
using NLog;

namespace SudokuUI.Services;

public partial class SelectionService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public enum Mode { Digits, Hints };

    [ObservableProperty]
    private Mode _inputMode = Mode.Digits;

    [ObservableProperty]
    private int _digit = 0; // 0 is empty

    public void ClearDigit() => Digit = 0;

    public void ToggleInputMode() => InputMode = (InputMode == Mode.Digits ? Mode.Hints : Mode.Digits);

    public void NextDigit()
    {
        if (Digit == 9)
            Digit = 1;
        else
            Digit++;
    }

    public void PreviousDigit()
    {
        if (Digit == 1)
            Digit = 9;
        else
            Digit--;
    }

    partial void OnInputModeChanged(Mode value)
    {
        logger.Debug($"Input mode is now: {InputMode.ToString()}");
    }
}
