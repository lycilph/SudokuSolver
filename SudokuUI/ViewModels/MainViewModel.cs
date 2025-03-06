using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SelectionService selection_service;

    [ObservableProperty]
    private GridViewModel _grid = null!;

    [ObservableProperty]
    private SelectionViewModel _selection;

    [ObservableProperty]
    private SettingsViewModel _settings;

    public MainViewModel(SelectionService selection_service, GridViewModel grid, SelectionViewModel selection, SettingsViewModel settings)
    {
        this.selection_service = selection_service;

        Grid = grid;
        Selection = selection;
        Settings = settings;
    }

    [RelayCommand]
    private void NumberKeyPressed(string number)
    {
        if (int.TryParse(number, out int digit))
            selection_service.Digit = digit;
    }

    [RelayCommand]
    private void ClearSelection()
    {
        selection_service.ClearDigit();
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        selection_service.ToggleInputMode();
    }

    [RelayCommand]
    private void ShowSettings()
    {
        Settings.IsOpen = true;
    }

    [RelayCommand]
    private void HideSettings()
    {
        Settings.IsOpen = false;
    }
}