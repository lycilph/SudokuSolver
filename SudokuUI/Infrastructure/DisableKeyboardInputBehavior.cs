using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace SudokuUI.Infrastructure;

public class DisableKeyboardInputBehavior : Behavior<Window>
{
    public static readonly DependencyProperty IsDisabledProperty =
        DependencyProperty.Register(
            nameof(IsDisabled),
            typeof(bool),
            typeof(DisableKeyboardInputBehavior),
            new PropertyMetadata(false));

    public bool IsDisabled
    {
        get => (bool)GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    public static readonly DependencyProperty IgnoredKeysProperty =
        DependencyProperty.Register(
            nameof(IgnoredKeys),
            typeof(IList<Key>),
            typeof(DisableKeyboardInputBehavior),
            new PropertyMetadata(new KeyCollection()));

    public IList<Key> IgnoredKeys
    {
        get => (IList<Key>)GetValue(IgnoredKeysProperty);
        set => SetValue(IgnoredKeysProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewKeyDown += Window_PreviewKeyDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewKeyDown -= Window_PreviewKeyDown;
        base.OnDetaching();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (IsDisabled && !IgnoredKeys.Contains(e.Key))
        {
            e.Handled = true;
        }
    }
}
