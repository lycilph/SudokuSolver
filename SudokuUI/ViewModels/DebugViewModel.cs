using CommunityToolkit.Mvvm.ComponentModel;
using Core.Commands;
using ObservableCollections;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DebugViewModel : ObservableObject
{
    public NotifyCollectionChangedSynchronizedViewList<ICommand> UndoStack { get; private set; }
    public NotifyCollectionChangedSynchronizedViewList<ICommand> RedoStack { get; private set; }

    [ObservableProperty]
    private ICommand? selected;

    public DebugViewModel(UndoRedoService undo_service)
    {
        UndoStack = undo_service.UndoStack.ToNotifyCollectionChanged();
        RedoStack = undo_service.RedoStack.ToNotifyCollectionChanged();
    }
}
