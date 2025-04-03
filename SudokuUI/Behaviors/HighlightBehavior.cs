using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;
using NLog;
using SudokuUI.Services;
using SudokuUI.ViewModels;
using SudokuUI.Visualizers.Misc;

namespace SudokuUI.Behaviors;

public class HighlightBehavior : Behavior<Canvas>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public HighlightService Service
    {
        get { return (HighlightService)GetValue(ServiceProperty); }
        set { SetValue(ServiceProperty, value); }
    }
    public static readonly DependencyProperty ServiceProperty =
        DependencyProperty.Register(nameof(Service), typeof(HighlightService), typeof(HighlightBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnServiceChanged)));

    public GridViewModel ViewModel
    {
        get { return (GridViewModel)GetValue(ViewModelProperty); }
        set { SetValue(ViewModelProperty, value); }
    }
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(GridViewModel), typeof(HighlightBehavior), new PropertyMetadata(null));

    private static void OnServiceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is HighlightBehavior behavior)
        {
            logger.Debug("Service was changed");

            behavior.Service.Links.CollectionChanged += (s,e) => 
                UpdateLines(behavior.ViewModel, behavior.Service, behavior.AssociatedObject);
        }
    }

    private static void UpdateLines(GridViewModel vm, HighlightService service, Canvas canvas)
    {
        logger.Debug("Lines changed");
        canvas.Children.Clear();

        foreach (var link in service.Links) 
        { 
            var link_start_vm = vm.Map(link.Start);
            var link_end_vm = vm.Map(link.End);

            var parent = canvas.Parent as Visual;
            if (parent != null)
            {
                var control_for_link_start = FindContentControlForViewModel(canvas.Parent, link_start_vm);
                var control_for_link_end = FindContentControlForViewModel(canvas.Parent, link_end_vm);
                if (control_for_link_start != null && control_for_link_end != null)
                {
                    var point_for_link_start = GetCoordinatesRelativeToVisual(control_for_link_start, parent);
                    var point_for_link_end = GetCoordinatesRelativeToVisual(control_for_link_end, parent);

                    var line = new Line
                    {
                        X1 = point_for_link_start.X + control_for_link_start.ActualWidth / 2,
                        Y1 = point_for_link_start.Y + control_for_link_start.ActualHeight / 2,
                        X2 = point_for_link_end.X + control_for_link_end.ActualWidth / 2,
                        Y2 = point_for_link_end.Y + control_for_link_end.ActualHeight / 2,
                        Stroke = link.Color,
                        StrokeThickness = 4
                    };

                    if (link.Style == LinkVisualizer.LineType.Dotted)
                        line.StrokeDashArray = [4, 2];

                    canvas.Children.Add(line);
                }
            }
        }
    }

    // Find the ContentControl hosting a specific ViewModel
    public static ContentControl? FindContentControlForViewModel(DependencyObject parent, object viewModel)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is ContentControl contentControl && contentControl.DataContext == viewModel)
            {
                return contentControl;
            }

            var result = FindContentControlForViewModel(child, viewModel);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    // Get coordinates of a control relative to its Canvas parent
    public static Point GetCoordinatesRelativeToVisual(UIElement element, Visual visual)
    {
        var transform = element.TransformToAncestor(visual);
        return transform.Transform(new Point(0, 0)); // Transform the origin (top-left) of the element
    }
}
