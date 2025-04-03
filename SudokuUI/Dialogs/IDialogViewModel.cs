namespace SudokuUI.Dialogs;

public interface IDialogViewModel<T>
{
    Task<T> DialogResult { get; }
}
