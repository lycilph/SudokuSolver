using System.Windows.Controls;

namespace SudokuUI.Dialogs.ImageImport;

public partial class CaptureImageImportDialogView : UserControl
{
    public CaptureImageImportDialogView()
    {
        InitializeComponent();

        LayoutUpdated += (s, e) =>
        {
            if (DataContext is CaptureImageImportDialogViewModel vm)
            {
                vm.Initialize();
            }
        };
    }
}
