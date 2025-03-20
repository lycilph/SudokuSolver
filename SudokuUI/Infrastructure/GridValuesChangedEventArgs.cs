namespace SudokuUI.Infrastructure;

public class GridValuesChangedEventArgs(List<int> digitCount) : EventArgs
{
    public List<int> DigitCount { get; set; } = digitCount;
}
