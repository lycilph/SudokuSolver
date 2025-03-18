using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
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

    public CommandManagerViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
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
    }

    private bool CanExecute() => Command != null;

    [RelayCommand(CanExecute=nameof(CanExecute))]
    private void Execute()
    {
        Command?.Do();
        Command = null;

        WeakReferenceMessenger.Default.Send(new CommandExecutedMessage());
    }
}
