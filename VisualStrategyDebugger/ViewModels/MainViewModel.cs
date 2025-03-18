using CommunityToolkit.Mvvm.ComponentModel;

namespace VisualStrategyDebugger.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GridViewModel gridViewModel;

    [ObservableProperty]
    private CommandManagerViewModel commandManagerViewModel;

    [ObservableProperty]
    private StrategyManagerViewModel strategyManagerViewModel;

    [ObservableProperty]
    private GridManagerViewModel gridManagerViewModel;


    public MainViewModel(GridViewModel gridViewModel,
                         CommandManagerViewModel commandManagerViewModel,
                         StrategyManagerViewModel strategyManagerViewModel,
                         GridManagerViewModel gridManagerViewModel)
    {
        GridViewModel = gridViewModel;
        CommandManagerViewModel = commandManagerViewModel;
        StrategyManagerViewModel = strategyManagerViewModel;
        GridManagerViewModel = gridManagerViewModel;
    }
}