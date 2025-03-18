using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Messages;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.Views;

namespace VisualStrategyDebugger.ViewModels;

public partial class GridManagerViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private ObservableCollection<Tuple<string, string>> predefinedGrids = [];

    [ObservableProperty]
    private Tuple<string, string> currentGrid;

    public GridManagerViewModel(GridService grid_service)
    {
        this.grid_service = grid_service;

        PredefinedGrids.Add(Tuple.Create("Easy", ".5..83.17...1..4..3.4..56.8....3...9.9.8245....6....7...9....5...729..861.36.72.4"));
        PredefinedGrids.Add(Tuple.Create("Moderate", "4...3.......6..8..........1....5..9..8....6...7.2........1.27..5.3....4.9........"));
        PredefinedGrids.Add(Tuple.Create("Hard", "98.7..6..75..4......3..8.7.8....9.5..3.2..1.....4....6.7...4.3....8..4......1...2"));

        CurrentGrid = predefinedGrids.First();
    }

    partial void OnCurrentGridChanged(Tuple<string, string> value)
    {
        grid_service.Import(value.Item2);
    }

    [RelayCommand]
    private void Import()
    {
        var dialog = new InputMessagebox
        {
            Title = "Import grid",
            Message = "Paste grid here:",
            Owner = App.Current.MainWindow
        };

        var result = dialog.ShowDialog();

        if (result == true)
        {
            PredefinedGrids.Add(Tuple.Create("Custom", dialog.Input));
            CurrentGrid = PredefinedGrids.Last();
        }
    }

    [RelayCommand]
    private void Reset()
    {
        grid_service.Reset();
    }
}
