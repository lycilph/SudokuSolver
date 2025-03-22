using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Commands;
using NLog;
using ObservableCollections;

namespace SudokuUI.Services;

public partial class UndoRedoService : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly ObservableStack<ICommand> undo = new();
    private readonly ObservableStack<ICommand> redo = new();

    private bool CanUndo => undo.Count > 0;
    private bool CanRedo => redo.Count > 0;
    private bool CanReset => CanUndo || CanRedo;

    public UndoRedoService()
    {
        undo.CollectionChanged += (in NotifyCollectionChangedEventArgs<ICommand> e) =>
        { 
            UndoCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        };
        redo.CollectionChanged += (in NotifyCollectionChangedEventArgs<ICommand> e) =>
        {
            RedoCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        };
    }

    public void Execute(ICommand command)
    {
        logger.Info("Executing command");

        command.Do();
        
        undo.Push(command);
        redo.Clear();
    }

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo() 
    {
        logger.Info("Undoing command");

        var command = undo.Pop();
        redo.Push(command);

        command.Undo();
    }
    
    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo() 
    {
        logger.Info("Redoing command");

        var command = redo.Pop();
        undo.Push(command);

        command.Do();
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        logger.Info("Resetting undo/redo stack");
    }
}
