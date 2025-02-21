using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;
using SudokuUI.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SudokuUI;

public partial class MainWindow : Window
{
    private Puzzle puzzle = new();

    public ObservableCollection<DigitSelection> DigitSelections { get; private set; }

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

        DataContext = this;

        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelection(d)), new DigitSelection(0)];

        Grid = new GridViewModel(puzzle.Grid);
    }
}