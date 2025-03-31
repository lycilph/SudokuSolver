using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using SudokuUI.Infrastructure;

namespace SudokuUI.Behaviors;
public class DisableKeyboardInputBehavior : Behavior<Window>
{
    private InputBindingCollection original_input_bindings = new InputBindingCollection();

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

    public bool IsInputBindingsDisabled
    {
        get { return (bool)GetValue(IsInputBindingsDisabledProperty); }
        set { SetValue(IsInputBindingsDisabledProperty, value); }
    }

    public static readonly DependencyProperty IsInputBindingsDisabledProperty =
        DependencyProperty.Register(
            nameof(IsInputBindingsDisabled),
            typeof(bool),
            typeof(DisableKeyboardInputBehavior), 
            new PropertyMetadata(false, new PropertyChangedCallback(OnInputBindingsDisabledChanged)));

    private static void OnInputBindingsDisabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is DisableKeyboardInputBehavior behavior)
        {
            var str = behavior.IsInputBindingsDisabled ? "disabled" : "enabled";
            System.Diagnostics.Debug.WriteLine($"Input bindings is now {str}");

            if (behavior.IsInputBindingsDisabled)
                behavior.AssociatedObject.InputBindings.Clear();
            else
                behavior.AssociatedObject.InputBindings.AddRange(behavior.original_input_bindings);
        }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        
        AssociatedObject.PreviewKeyDown += Window_PreviewKeyDown;
        original_input_bindings = new InputBindingCollection(AssociatedObject.InputBindings);
    }

    protected override void OnDetaching()
    {
        original_input_bindings.Clear();
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