using Microsoft.Xaml.Behaviors;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Views;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuUI.Behaviors;

public class DragSelectBehavior : Behavior<GridView>
{
    private bool is_dragging = false;
    private readonly List<CellViewModel> selected_cells = [];
    private readonly Brush drag_selection_color;

    public SelectionService.Mode Mode
    {
        get { return (SelectionService.Mode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }
    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register("Mode", typeof(SelectionService.Mode), typeof(DragSelectBehavior), new PropertyMetadata(SelectionService.Mode.Hints));

    public DragSelectBehavior()
    {
        drag_selection_color = App.Current.Resources["drag_selection_color"] as Brush ?? Brushes.Black;
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        AssociatedObject.MouseMove += AssociatedObject_MouseMove;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
        AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        base.OnDetaching();
    }

    private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Mode == SelectionService.Mode.Hints)
        {
            is_dragging = true;
            selected_cells.ForEach(vm => vm.ResetVisuals());
            selected_cells.Clear();

            if (sender is IInputElement elem && sender is Visual visual)
                SelectCell(visual, e.GetPosition(elem));
        }
        else
        {
            if (sender is IInputElement elem && sender is Visual visual)
            {
                var vm = GetCell(visual, e.GetPosition(elem));
                vm?.Set();
            }
        }
    }

    private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (Mode == SelectionService.Mode.Digits)
            return;

        is_dragging = false;
        selected_cells.ForEach(vm =>
        {
            vm.Set();
            vm.ResetVisuals();
        });
    }

    private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
    {
        if (is_dragging && sender is IInputElement elem && sender is Visual visual)
            SelectCell(visual, e.GetPosition(elem));
    }

    private CellViewModel? GetCell(Visual visual, Point position)
    {
        var result = VisualTreeHelper.HitTest(visual, position);
        if (result != null)
        {
            var view = FindParent<CellView>(result.VisualHit);
            if (view != null && view.DataContext is CellViewModel vm)
                return vm;
        }
        return null;
    }

    private void SelectCell(Visual visual, Point position)
    {
        var result = VisualTreeHelper.HitTest(visual, position);
        if (result != null)
        {
            var view = FindParent<CellView>(result.VisualHit);
            if (view != null && view.DataContext is CellViewModel vm && !selected_cells.Contains(vm))
            {
                if (vm.WrappedObject.IsEmpty)
                {
                    selected_cells.Add(vm);
                    vm.BackgroundColor = drag_selection_color;
                }
            }
        }
    }

    private T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parent_object = VisualTreeHelper.GetParent(child);
        if (parent_object == null) return null;

        if (parent_object is T parent)
            return parent;
        else
            return FindParent<T>(parent_object);
    }
}
