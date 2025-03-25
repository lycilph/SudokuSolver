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
    private readonly UndoRedoService undo_service;

    [ObservableProperty]
    private IStrategy wrappedObject;

    [ObservableProperty]
    private bool selected = false;

    public StrategyViewModel(IStrategy strategy)
    {
        puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();
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
}
