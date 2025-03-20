using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    public MainViewModel(SelectionService selection_service, GridViewModel gridVM, DigitSelectionViewModel digitSelectionVM)
    {
        this.selection_service = selection_service;
        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void Escape()
    {
        //if (Settings.IsOpen)
        //    HideSettings();
        //else
            selection_service.Clear();
    }

    [RelayCommand]
    private void NextDigit()
    {
        selection_service.Next();
    }

    [RelayCommand]
    private void PreviousDigit()
    {
        selection_service.Previous();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }
}