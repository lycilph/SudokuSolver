using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuUI.Views.Dialogs;

public partial class SolverHintDialogView : UserControl
{
    public SolverHintDialogView()
    {
        InitializeComponent();

        Loaded += (_, _) => { MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); };
    }
}
