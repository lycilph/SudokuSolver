using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;

namespace SudokuUI.Dialogs.ImageImport;

public class ImageImportStep(IDialogViewModel<string?> vm, UserControl view, string title)
{
    public IDialogViewModel<string?> ViewModel { get; set; } = vm;
    public UserControl View { get; set; } = view;
    public string Title { get; set; } = title;
}
