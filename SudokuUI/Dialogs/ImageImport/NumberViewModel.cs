using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SudokuUI.Dialogs.ImageImport;

public partial class NumberViewModel : ObservableObject
{
    private VerifyImageImportDialogViewModel parent;

    [ObservableProperty]
    private int number;

    [ObservableProperty]
    private bool selected = false;

    public NumberViewModel(VerifyImageImportDialogViewModel parent, int number)
    {
        this.parent = parent;
        Number = number;
    }

    [RelayCommand]
    private void Select()
    {
        parent.Update(this);
    }
}
