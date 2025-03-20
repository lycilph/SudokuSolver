using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    public bool IsHintMode
    {
        get => selection_service.InputMode == SelectionService.Mode.Hints;
        set => selection_service.InputMode = (value ? SelectionService.Mode.Hints : SelectionService.Mode.Digits);
    }

    public ObservableCollection<DigitViewModel> Digits { get; private set; }

    public DigitSelectionViewModel(SelectionService selection_service)
    {
        this.selection_service = selection_service;

        Digits = Grid.PossibleValues.Select(i => new DigitViewModel(i, selection_service)).ToObservableCollection();

        selection_service.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(SelectionService.InputMode))
                OnPropertyChanged(nameof(IsHintMode));
        };
    }
}
