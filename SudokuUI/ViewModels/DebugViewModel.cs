using Core.Commands;
using ObservableCollections;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public class DebugViewModel
{
    public NotifyCollectionChangedSynchronizedViewList<ICommand> UndoStack { get; private set; }
    public NotifyCollectionChangedSynchronizedViewList<ICommand> RedoStack { get; private set; }

    public DebugViewModel(UndoRedoService undo_service)
    {
        UndoStack = undo_service.UndoStack.ToNotifyCollectionChanged();
        RedoStack = undo_service.RedoStack.ToNotifyCollectionChanged();
    }
}
