using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuUI.Dialogs.ImageImport;

public partial class ImageImportDialogView : UserControl
{
    public ImageImportDialogView()
    {
        InitializeComponent();

        Loaded += (_, _) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }
}
