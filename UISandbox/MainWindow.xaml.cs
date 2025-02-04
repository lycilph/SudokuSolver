using Core.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UISandbox;

public partial class MainWindow : Window
{
    public ObservableCollection<Cell> Cells { get; private set; }
    public ObservableCollection<Unit> Boxes { get; private set; }

    public MainWindow()
    {
        InitializeComponent();

        var grid = new Core.Model.Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4");
        Cells = new ObservableCollection<Cell>(grid.Cells);
        Boxes = new ObservableCollection<Unit>(grid.Boxes);

        DataContext = this;
    }

    private void TestClick(object sender, RoutedEventArgs e)
    {
        var c1 = Cells[10];
        var c2 = Cells[24];

        var p1 = GetCenterPositionOfCell(c1);
        var p2 = GetCenterPositionOfCell(c2);

        DrawCircle(overlay_canvas, p1, 10, Brushes.Red);
        DrawLine(overlay_canvas, p1, p2, Brushes.Red);
    }

    private Point GetCenterPositionOfCell(Cell c)
    {
        var boxes_ic = FindChild<ItemsControl>(this) ?? throw new Exception("bla bla"); ;
        var box1 = GetBoxContainingCell(c);
        var box_container = boxes_ic.ItemContainerGenerator.ContainerFromItem(box1) ?? throw new Exception("bla bla");

        var cells_ic = FindChild<ItemsControl>(box_container) ?? throw new Exception("bla bla");
        var cell_container = cells_ic.ItemContainerGenerator.ContainerFromItem(c) ?? throw new Exception("bla bla");
        var cell_presenter = cell_container as ContentPresenter ?? throw new Exception("bla bla");

        var pos = cell_presenter.TransformToVisual(overlay_canvas).Transform(new Point(0, 0));
        pos.X += cell_presenter.ActualWidth / 2;
        pos.Y += cell_presenter.ActualHeight / 2;

        return pos;
    }

    private void DrawCircle(Canvas canvas, Point p, double radius, Brush color)
    {
        Ellipse circle = new Ellipse
        {
            Width = radius * 2,
            Height = radius * 2,
            Fill = color,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        // Set position (top-left corner needs to be adjusted)
        Canvas.SetLeft(circle, p.X - radius);
        Canvas.SetTop(circle, p.Y - radius);

        // Add to canvas
        canvas.Children.Add(circle);
    }

    private void DrawLine(Canvas canvas, Point p1, Point p2, Brush color, double thickness = 2)
    {
        Line line = new Line
        {
            X1 = p1.X,
            Y1 = p1.Y,
            X2 = p2.X,
            Y2 = p2.Y,
            Stroke = color,
            StrokeThickness = thickness
        };

        canvas.Children.Add(line);
    }

    private Unit? GetBoxContainingCell(Cell cell)
    {
        return Boxes.FirstOrDefault(u => u.Cells.Contains(cell));
    }

    private T? FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
                return typedChild;

            var descendant = FindChild<T>(child);
            if (descendant != null)
                return descendant;
        }
        return null;
    }

    private void ClearClick(object sender, RoutedEventArgs e)
    {
        overlay_canvas.Children.Clear();
    }
}