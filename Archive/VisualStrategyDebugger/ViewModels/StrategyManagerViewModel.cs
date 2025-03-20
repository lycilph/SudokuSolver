using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Infrastructure;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Messages;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class StrategyManagerViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<StrategyViewModel> strategies;

    public StrategyManagerViewModel()
    {
        IStrategy[] strats =
            [
                new BasicEliminationStrategy(),
                new NakedSinglesStrategy(),
                new HiddenSinglesStrategy()
            ];
        Strategies = strats.Select(s => new StrategyViewModel(s)).ToObservableCollection();
    }

    [RelayCommand]
    private void Next()
    {
        // Make sure the previous command has been executed, before planning the next
        WeakReferenceMessenger.Default.Send(new ExecuteCommandMessage());

        var strategy = PlanNextStrategy();
        if (strategy == null)
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("No more strategies to execute"));
        else
            strategy.Execute();
    }

    [RelayCommand]
    private void ToggleAll()
    {
        if (Strategies.All(s => s.Selected))
            Strategies.ForEach(s => s.Selected = false);
        else
            Strategies.ForEach(s => s.Selected = true);
    }

    private StrategyViewModel? PlanNextStrategy()
    {
        foreach (var strategy in Strategies.Where(s => s.Selected))
        {
            if (strategy.IsValid())
                return strategy;
        }
        return null;
    }
}
