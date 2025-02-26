using Core.Model;
using Core.Strategies;
using SudokuUI.ViewModels;
using System.Windows;

namespace SudokuUI;

public partial class MainWindow : Window
{
    private Puzzle puzzle = new("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9");

    public SelectionViewModel Selections
    {
        get { return (SelectionViewModel)GetValue(SelectionsProperty); }
        set { SetValue(SelectionsProperty, value); }
    }
    public static readonly DependencyProperty SelectionsProperty =
        DependencyProperty.Register("Selections", typeof(SelectionViewModel), typeof(MainWindow), new PropertyMetadata(null));

    public GridViewModel Grid
    {
        get { return (GridViewModel)GetValue(gridProperty); }
        set { SetValue(gridProperty, value); }
    }
    public static readonly DependencyProperty gridProperty =
        DependencyProperty.Register("Grid", typeof(GridViewModel), typeof(MainWindow), new PropertyMetadata(null));

    public MainWindow() 
    {
        InitializeComponent();

        BasicEliminationStrategy.ExecuteAndApply(puzzle.Grid);

        DataContext = this;

        Selections = new SelectionViewModel();
        Grid = new GridViewModel(puzzle.Grid);
    }
}