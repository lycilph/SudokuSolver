using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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

    [ObservableProperty]
    private SettingsViewModel settingsVM;
    public MainViewModel(SelectionService selection_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         SettingsViewModel settingsVM)
    {
        this.selection_service = selection_service;
        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        SettingsVM = settingsVM;
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
        if (SettingsVM.IsOpen)
            HideSettings();
        else
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

    [RelayCommand]
    private void ShowSettings()
    {
        SettingsVM.IsOpen = true;
    }

    [RelayCommand]
    private void HideSettings()
    {
        SettingsVM.IsOpen = false;
    }

    [RelayCommand]
    private void Test()
    {
        var puzzle_service = App.Current.Services.GetRequiredService<PuzzleService>();
        var grid = puzzle_service.Grid;
        var cell = grid.Cells.Where(c => c.IsEmpty).First();
        if (cell != null)
            cell.Value = cell.Candidates.First();
    }
}