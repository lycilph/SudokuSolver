using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Messages;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class CommandManagerViewModel : 
    ObservableRecipient, 
    IRecipient<ValueChangedMessage<IGridCommand>>, 
    IRecipient<ExecuteCommandMessage>,
    IRecipient<ResetMessage>
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteCommand))]
    private IGridCommand? command;

    [ObservableProperty]
    private ObservableCollection<IGridCommand> undoStack = [];
    private ObservableCollection<IGridCommand> redoStack = [];

    public CommandManagerViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);

        UndoStack.CollectionChanged += (s, e) => UndoCommand.NotifyCanExecuteChanged();
        redoStack.CollectionChanged += (s, e) => RedoCommand.NotifyCanExecuteChanged();
    }

    public void Receive(ValueChangedMessage<IGridCommand> message)
    {
        Command = message.Value;
    }

    public void Receive(ExecuteCommandMessage message)
    {
        Execute();
    }

    public void Receive(ResetMessage message)
    {
        Command = null;

        if (message.NewGrid)
        {
            UndoStack.Clear();
            redoStack.Clear();
        }
    }

    private bool CanExecute() => Command != null;

    [RelayCommand(CanExecute=nameof(CanExecute))]
    private void Execute()
    {
        if (Command != null)
        {
            UndoStack.Add(Command);
            redoStack.Clear();
        }

        Command?.Do();
        Command = null;

        WeakReferenceMessenger.Default.Send(new CommandExecutedMessage());
    }

    private bool CanUndo => UndoStack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        var cmd = UndoStack.Last();

        UndoStack.Remove(cmd);
        redoStack.Add(cmd);

        cmd.Undo();
        WeakReferenceMessenger.Default.Send(new ResetMessage());
    }

    private bool CanRedo => redoStack.Count > 0;

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
    {
        var cmd = redoStack.Last();

        redoStack.Remove(cmd);
        UndoStack.Add(cmd);

        cmd.Do();
        WeakReferenceMessenger.Default.Send(new ResetMessage());
    }
}
