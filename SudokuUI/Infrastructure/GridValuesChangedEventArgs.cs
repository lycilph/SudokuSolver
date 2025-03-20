namespace SudokuUI.Infrastructure;

public class GridValuesChangedEventArgs : EventArgs
{
    public List<int> DigitCount { get; set; } = [];
}
