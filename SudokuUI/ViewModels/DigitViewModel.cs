using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SudokuUI.Services;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace SudokuUI.ViewModels;

public partial class DigitViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly SelectionService selection_service;

    [ObservableProperty]
    private int _digit;
    
    // The # of this digit left in the puzzle (ie. how many is left to place)
    [ObservableProperty]
    private int _missing = 9;

    [ObservableProperty]
    private bool _selected = false;

    public DigitViewModel(int digit)
    {
        Digit = digit;

        selection_service = App.Current.Services.GetRequiredService<SelectionService>();
        selection_service.PropertyChanged += SelectionChanged;
    }

    private void SelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectionService.Digit))
        {
            Selected = selection_service.Digit == Digit;
        }
    }

    [RelayCommand]
    private void Select()
    {
        var selection_text = Selected ? "selected" : "deselected";
        logger.Debug($"Digit {Digit} is now {selection_text}");
        selection_service.Digit = Selected ? Digit : 0;
    }
}
