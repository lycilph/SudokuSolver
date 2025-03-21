﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using VisualStrategyDebugger.Messages;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private IStrategy strategy;

    [ObservableProperty]
    private bool selected;

    public StrategyViewModel(IStrategy strategy)
    {
        Strategy = strategy;
        selected = true;

        grid_service = App.Current.Services.GetRequiredService<GridService>();
    }

    public bool IsValid()
    {
        return Strategy.Plan(grid_service.Grid) != null;
    }

    [RelayCommand]
    public void Execute()
    {
        var command = Strategy.Plan(grid_service.Grid);
        if (command != null)
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<IGridCommand>(command));
        else
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage($"Cannot execute strategy {Strategy.Name}"));
    }
}
