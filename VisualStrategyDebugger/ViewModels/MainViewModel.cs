using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Infrastructure;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private GridViewModel gridViewModel;

    [ObservableProperty]
    private CommandManagerViewModel commandManagerViewModel;

    [ObservableProperty]
    private ObservableCollection<StrategyViewModel> strategies;

    public MainViewModel(GridViewModel gridViewModel, CommandManagerViewModel commandManagerViewModel)
    {
        GridViewModel = gridViewModel;
        CommandManagerViewModel = commandManagerViewModel;

        IStrategy[] strats =
            [
                new BasicEliminationStrategy(),
                new NakedSinglesStrategy()
            ];
        Strategies = strats.Select(s => new StrategyViewModel(s)).ToObservableCollection();
    }
}