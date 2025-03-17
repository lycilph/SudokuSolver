using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Infrastructure;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private GridViewModel gridViewModel;

    [ObservableProperty]
    private CommandManagerViewModel commandManagerViewModel;

    [ObservableProperty]
    private ObservableCollection<StrategyViewModel> strategies;

    public MainViewModel(GridService grid_service, GridViewModel gridViewModel, CommandManagerViewModel commandManagerViewModel)
    {
        this.grid_service = grid_service;
        GridViewModel = gridViewModel;
        CommandManagerViewModel = commandManagerViewModel;

        IStrategy[] strats =
            [
                new BasicEliminationStrategy(),
                new NakedSinglesStrategy()
            ];
        Strategies = strats.Select(s => new StrategyViewModel(s)).ToObservableCollection();
        this.grid_service = grid_service;
    }

    [RelayCommand]
    private void Reset()
    {
        grid_service.Reset();
    }
}