using System.Windows;

namespace PaddleOCRUI;

public partial class CaptureImageWindow : Window
{
    public CaptureImageWindow()
    {
        InitializeComponent();
        DataContext = new CaptureImageViewModel();
    }
}
