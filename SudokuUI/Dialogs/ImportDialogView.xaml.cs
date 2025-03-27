using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuUI.Dialogs;

public partial class ImportDialogView : UserControl
{
    public ImportDialogView()
    {
        InitializeComponent();

        Loaded += (_, _) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }
}
