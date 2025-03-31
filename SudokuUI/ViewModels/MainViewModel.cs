using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SudokuUI.Messages;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SettingsService settings_service;

    [ObservableProperty]
    private GridViewModel gridVM;

    [ObservableProperty]
    private DigitSelectionViewModel digitSelectionVM;

    [ObservableProperty]
    private NotificationViewModel notificationVM;

    [ObservableProperty]
    private OverlayViewModel overlayVM;

    [ObservableProperty]
    private SettingsViewModel settingsVM;

    public MainViewModel(SettingsService settings_service,
                         GridViewModel gridVM,
                         DigitSelectionViewModel digitSelectionVM,
                         NotificationViewModel notificationVM,
                         OverlayViewModel overlayVM,
                         SettingsViewModel settingsVM)
    {
        this.settings_service = settings_service;

        GridVM = gridVM;
        DigitSelectionVM = digitSelectionVM;
        NotificationVM = notificationVM;
        OverlayVM = overlayVM;
        SettingsVM = settingsVM;

        OverlayVM.ClickCommand = EscapeCommand;

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsOpen))
                OverlayVM.IsOpen = settings_service.IsOpen;
        };
    }

    // Window commands (ie. buttons in the title bar)

    [RelayCommand]
    private void ShowSettings()
    {
        settings_service.Show();
    }

    // Input binding commands

    [RelayCommand]
    private void Escape()
    {
        if (settings_service.IsOpen)
        {
            settings_service.Hide();
            return;
        }

        if (OverlayVM.IsOpen)
        {
            OverlayVM.Hide();
            return;
        }
    }

    // Left side buttons

    [RelayCommand]
    private async Task NewPuzzle()
    {
        await OverlayVM.ShowNewGame();
    }

    [RelayCommand]
    private async Task ClearPuzzle()
    {
        OverlayVM.IsOpen = true;
        OverlayVM.ShowSpinner = true;
        await Task.Delay(1000);
        OverlayVM.ShowSpinner = false;
        OverlayVM.IsOpen = false;
    }

    [RelayCommand]
    private void Import()
    {
        WeakReferenceMessenger.Default.Send(new ShowNotificationMessage("Testing"));
    }

    // Right side buttons
}