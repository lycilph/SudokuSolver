using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuUI.Views;

public partial class ImportDialogView : UserControl
{
    public ImportDialogView()
    {
        InitializeComponent();

        this.Loaded += (_, _) => { this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); };
    }
}
