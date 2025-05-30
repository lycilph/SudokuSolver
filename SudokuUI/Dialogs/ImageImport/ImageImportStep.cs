using System.Windows.Controls;

namespace SudokuUI.Dialogs.ImageImport;

public class ImageImportStep(IDialogViewModel<ImportStepOutput> vm, UserControl view, string title)
{
    public IDialogViewModel<ImportStepOutput> ViewModel { get; set; } = vm;
    public UserControl View { get; set; } = view;
    public string Title { get; set; } = title;
}
