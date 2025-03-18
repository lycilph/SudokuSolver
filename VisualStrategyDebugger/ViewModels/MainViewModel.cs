using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VisualStrategyDebugger.Service;

namespace VisualStrategyDebugger.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private GridViewModel gridViewModel;

    [ObservableProperty]
    private CommandManagerViewModel commandManagerViewModel;

    [ObservableProperty]
    private StrategyManagerViewModel strategyManagerViewModel;


    public MainViewModel(GridService grid_service,
                         GridViewModel gridViewModel,
                         CommandManagerViewModel commandManagerViewModel,
                         StrategyManagerViewModel strategyManagerViewModel)
    {
        this.grid_service = grid_service;

        GridViewModel = gridViewModel;
        CommandManagerViewModel = commandManagerViewModel;
        StrategyManagerViewModel = strategyManagerViewModel;
    }

    [RelayCommand]
    private void Reset()
    {
        grid_service.Reset();
    }
}