using CommunityToolkit.Mvvm.ComponentModel;
using NLog;

namespace SudokuUI.Services;

public partial class SelectionService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public enum Mode { Digits, Hints };

    [ObservableProperty]
    private Mode inputMode = Mode.Digits;

    [ObservableProperty]
    private int digit = 0; // 0 is empty

    public void ToggleInputMode() => InputMode = (InputMode == Mode.Digits ? Mode.Hints : Mode.Digits);

    public void Clear() => Digit = 0;

    public void Next()
    {
        if (Digit == 9)
            Digit = 1;
        else
            Digit++;
    }

    public void Previous()
    {
        if (Digit == 1)
            Digit = 9;
        else
            Digit--;
    }

    partial void OnInputModeChanged(Mode value)
    {
        logger.Debug($"InputMode is now {value}");
    }

    partial void OnDigitChanged(int value)
    {
        logger.Debug($"Selected digit is now {value}");
    }
}
