using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Commands;
using Core.Extensions;
using Core.Strategies;
using ObservableCollections;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DebugViewModel : ObservableObject
{
    public NotifyCollectionChangedSynchronizedViewList<ICommand> UndoStack { get; private set; }
    public NotifyCollectionChangedSynchronizedViewList<ICommand> RedoStack { get; private set; }

    public ObservableCollection<StrategyViewModel> Strategies { get; private set; }

    [ObservableProperty]
    private ICommand? selected;

    public DebugViewModel(SolverService solver_service, UndoRedoService undo_service)
    {
        UndoStack = undo_service.UndoStack.ToNotifyCollectionChanged();
        RedoStack = undo_service.RedoStack.ToNotifyCollectionChanged();

        Strategies = solver_service.Strategies.Select(s => new StrategyViewModel(s)).ToObservableCollection();
    }
}
