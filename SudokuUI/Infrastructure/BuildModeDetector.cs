namespace SudokuUI.Infrastructure;

public static class BuildModeDetector
{
    public static bool IsDebug
    {
        get
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }

    public static bool IsRelease => !IsDebug;
}
