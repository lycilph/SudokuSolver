using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SelectionService selection_service;
    private readonly SettingsService settings_service;
    private readonly DebugService debug_service;

    [ObservableProperty]
    private bool isKeyboardDisabled = false;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    [ObservableProperty]
    private SettingsOverlayViewModel settingsOverlayVM;

    public MainViewModel(SelectionService selection_service,
                         SettingsService settings_service,
                         DebugService debug_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         SettingsViewModel settingsVM,
                         SettingsOverlayViewModel settingsOverlayVM)
    {
        this.selection_service = selection_service;
        this.settings_service = settings_service;
        this.debug_service = debug_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        SettingsVM = settingsVM;
        SettingsOverlayVM = settingsOverlayVM;

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsShown))
                IsKeyboardDisabled = settings_service.IsShown;
        };
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
        if (settings_service.IsShown)
            settings_service.Hide();
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
        settings_service.Show();
    }

    [RelayCommand]
    private void HideSettings()
    {
        settings_service.Hide();
    }

    [RelayCommand]
    private void Test()
    {
        debug_service.ShowDebugWindow();
    }
}