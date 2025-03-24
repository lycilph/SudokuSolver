using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Commands;
using Core.Strategies;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;

    [ObservableProperty]
    private IStrategy wrappedObject;

    [ObservableProperty]
    private bool selected = false;

    public StrategyViewModel(IStrategy strategy)
    {
        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();

        WrappedObject = strategy;
    }

    [RelayCommand]
    private void Execute()
    {
        logger.Info($"Executing strategy {WrappedObject.Name}");

        if (WrappedObject.Plan(puzzle_service.Grid) is BaseCommand command)
        {
            logger.Info($"{command.Name} is {command.IsValid()}");
            command.Do();
        }
        else
            logger.Info("Strategy invalid");
    }
}
