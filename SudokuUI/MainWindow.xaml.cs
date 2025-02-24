using CommunityToolkit.Mvvm.ComponentModel;
using Core.Model;
using SudokuUI.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SudokuUI;

public partial class MainWindow : Window
{
    private Puzzle puzzle = new("4......38.32.941...953..24.37.6.9..4.29..16.36.47.3.9.957..83....39..4..24..3.7.9");

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

        //DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelection(d)), new DigitSelection(0)];
        DigitSelections = [.. Enumerable.Range(1, 9).Select(d => new DigitSelection(d))];

        Grid = new GridViewModel(puzzle.Grid);
    }
}