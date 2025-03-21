using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class SettingsOverlayViewModel : ObservableObject
{
    private readonly SettingsService settings_service;

    public bool IsOpen
    {
        get => settings_service.IsShown;
        set => settings_service.IsShown = value;
    }

    public SettingsOverlayViewModel(SettingsService settings_service)
    {
        this.settings_service = settings_service;

        settings_service.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsService.IsShown))
                OnPropertyChanged(nameof(IsOpen));
        };
    }

    [RelayCommand]
    private void HideSettings() => settings_service.Hide();
}
