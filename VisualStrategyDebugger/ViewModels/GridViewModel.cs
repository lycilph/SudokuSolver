using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Core.Infrastructure;
using System.Collections.ObjectModel;
using VisualStrategyDebugger.Messages;
using VisualStrategyDebugger.Service;
using VisualStrategyDebugger.Temp;

namespace VisualStrategyDebugger.ViewModels;

public partial class GridViewModel : 
    ObservableRecipient, 
    IRecipient<ValueChangedMessage<IGridCommand>>,
    IRecipient<CommandExecutedMessage>,
    IRecipient<ResetMessage>
{
    private readonly GridService grid_service;

    [ObservableProperty]
    private ObservableCollection<CellViewModel> cells;

    public GridViewModel(GridService grid_service)
    {
        this.grid_service = grid_service;

        Cells = grid_service.Grid.Cells.Select(c => new CellViewModel(c)).ToObservableCollection();

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void ToggleIndex()
    {
        foreach (var cell in Cells)
            cell.ToggleIndex();
    }

    public void Receive(ValueChangedMessage<IGridCommand> message)
    {
        var command = message.Value;
        if (command != null)
        {
            var visualizer = command.GetVisualizer();
            visualizer?.Show(this);
        }
    }

    public void Receive(CommandExecutedMessage message)
    {
        ClearVisualization();
    }

    public void Receive(ResetMessage message)
    {
        ClearVisualization();
    }

    private void ClearVisualization()
    {
        foreach (var cell in Cells)
        {
            cell.Highlight = false;
            foreach (var candidate in cell.Candidates)
                candidate.Highlight = false;
        }
    }
}
