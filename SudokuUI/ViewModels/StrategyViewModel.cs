using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using Core.Strategies;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Messages;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class StrategyViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly PuzzleService puzzle_service;
    private readonly SolverService solver_service;
    private readonly UndoRedoService undo_service;

    [ObservableProperty]
    private IStrategy wrappedObject;

    [ObservableProperty]
    private bool selected = true;

    public StrategyViewModel(IStrategy strategy)
    {
        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();
        solver_service = App.Current.Services.GetRequiredService<SolverService>();
        undo_service = App.Current.Services.GetRequiredService<UndoRedoService>();

        WrappedObject = strategy;
    }

    [RelayCommand]
    private void Execute()
    {
        logger.Info($"Executing strategy {WrappedObject.Name}");

        if (WrappedObject.Plan(puzzle_service.Grid) is BaseCommand command)
            undo_service.Execute(command);
        else
            WeakReferenceMessenger.Default.Send(new ShowNotificationMessage($"Cannot execute strategy {WrappedObject.Name}"));
    }

    [RelayCommand]
    private void Visualize()
    {
        logger.Info($"Visualizing strategy {WrappedObject.Name}");

        if (WrappedObject.Plan(puzzle_service.Grid) is BaseCommand command)
        {
            solver_service.SetHint(command);
            solver_service.Show();
        }
    }
}
