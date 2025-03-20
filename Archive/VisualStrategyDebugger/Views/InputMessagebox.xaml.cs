using System.Windows;

namespace VisualStrategyDebugger.Views;

public partial class InputMessagebox
{
    public string Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }
    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(InputMessagebox), new PropertyMetadata(string.Empty));
    
    public string Input
    {
        get { return (string)GetValue(InputProperty); }
        set { SetValue(InputProperty, value); }
    }
    public static readonly DependencyProperty InputProperty =
        DependencyProperty.Register("Input", typeof(string), typeof(InputMessagebox), new PropertyMetadata(string.Empty));

    public InputMessagebox()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OkClick(object sender, RoutedEventArgs e) => DialogResult = true;

    private void CancelClick(object sender, RoutedEventArgs e) => DialogResult = false;
}