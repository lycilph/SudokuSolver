using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Extensions;
using Core.Models;

namespace SudokuUI.ViewModels;

public partial class DigitSelectionViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isHintMode = false;

    public ObservableCollection<DigitViewModel> Digits { get; private set; }

    public DigitSelectionViewModel()
    {
        Digits = Grid.PossibleValues.Select(i => new DigitViewModel(i)).ToObservableCollection();
    }
}
