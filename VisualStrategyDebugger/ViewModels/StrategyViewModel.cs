using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private IStrategy strategy;

    public StrategyViewModel(IStrategy strategy)
    {
        Strategy = strategy;
        grid_service = App.Current.Services.GetRequiredService<GridService>();
    }

    [RelayCommand]
    private void Execute()
    {
        var command = Strategy.Plan(grid_service.Grid);
        command?.Do();
    }
}
