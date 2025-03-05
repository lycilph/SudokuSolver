using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using Core.Archive.Strategies;

namespace SudokuUI;

public partial class MainWindow : Window
{
    public ObservableCollection<CellViewModel> Cells { get; private set; }

    public MainWindow()
    {
        InitializeComponent();

        var grid = new Core.Archive.Model.Grid(".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4");
        BasicEliminationStrategy.Execute(grid);
        Cells = new ObservableCollection<CellViewModel>(grid.Cells.Select(c => new CellViewModel(c)));

        DataContext = this;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Cells[0].Value = 2;
        Cells[4].Value = 7;

        Cells[1].Selected = !Cells[1].Selected;
    }

    private void Button_Checked(object sender, RoutedEventArgs e)
    {
        foreach (var toggle in button_panel.Children.OfType<ToggleButton>())
        {
            if (toggle != sender)
                toggle.IsChecked = false;
        }

        if (sender is ToggleButton button && button.Content is string str)
        {
            var value = int.Parse(str);
            foreach (var cell in Cells)
                cell.UpdateSelection(value);
        }
    }

    private void Button_Unchecked(object sender, RoutedEventArgs e)
    {
        foreach (var cell in Cells)
            cell.UpdateSelection(0);
    }
}