using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Commands;
using NLog;
using ObservableCollections;
using SudokuUI.Messages;

namespace SudokuUI.Services;

public partial class UndoRedoService : ObservableRecipient, IRecipient<ResetMessage>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public ObservableStack<ICommand> UndoStack { get; private set; } = new();
    public ObservableStack<ICommand> RedoStack { get; private set; } = new();

    private bool CanUndo => UndoStack.Count > 0;
    private bool CanRedo => RedoStack.Count > 0;
    private bool CanReset => CanUndo || CanRedo;

    public UndoRedoService()
    {
        UndoStack.CollectionChanged += (in NotifyCollectionChangedEventArgs<ICommand> e) =>
        { 
            UndoCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        };
        RedoStack.CollectionChanged += (in NotifyCollectionChangedEventArgs<ICommand> e) =>
        {
            RedoCommand.NotifyCanExecuteChanged();
            ResetCommand.NotifyCanExecuteChanged();
        };

        IsActive = true;
    }

    public void Execute(ICommand command)
    {
        logger.Info("Executing command");

        command.Do();

        UndoStack.Push(command);
        RedoStack.Clear();
    }

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo() 
    {
        logger.Info("Undoing command");

        var command = UndoStack.Pop();
        RedoStack.Push(command);

        command.Undo();
    }
    
    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo() 
    {
        logger.Info("Redoing command");

        var command = RedoStack.Pop();
        UndoStack.Push(command);

        command.Do();
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        logger.Info("Resetting undo/redo stack");

        WeakReferenceMessenger.Default.Send(new ResetMessage());
    }

    public void Receive(ResetMessage message)
    {
        UndoStack.Clear();
        RedoStack.Clear();
    }
}
