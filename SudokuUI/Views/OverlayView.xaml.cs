using System.Windows.Controls;
using SudokuUI.ViewModels;

namespace SudokuUI.Views;

public partial class OverlayView : UserControl
{
    public OverlayView()
    {
        InitializeComponent();
    }

    private void OpenAnimationCompleted(object sender, EventArgs e)
    {
        if (DataContext is OverlayViewModel vm)
            vm.OnOpenAnimationCompleted();
    }

    private void CloseAnimationCompleted(object sender, EventArgs e)
    {
        if (DataContext is OverlayViewModel vm)
            vm.OnCloseAnimationCompleted();
    }
}
