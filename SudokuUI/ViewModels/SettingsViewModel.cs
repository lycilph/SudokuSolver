using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;
using Core.Extensions;
using SudokuUI.Services;

namespace SudokuUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SettingsService settings_service;

    [ObservableProperty]
    private bool isActive = false;

    [ObservableProperty]
    private bool isLight = true;

    [ObservableProperty]
    private Theme selectedApplicationColor;

    public ObservableCollection<Theme> ApplicationColors { get; set; }

    public SettingsViewModel(SettingsService settings_service)
    {
        this.settings_service = settings_service;

        ApplicationColors = settings_service.Themes.ToObservableCollection();
        IsLight = settings_service.GetBaseColorScheme() == SettingsService.Light;
        SelectedApplicationColor = ApplicationColors.First(t => t.ColorScheme == settings_service.GetColorScheme());
    }

    partial void OnIsLightChanged(bool value) => settings_service.SetBaseColorScheme(value);
    partial void OnSelectedApplicationColorChanged(Theme value) => settings_service.SetColorScheme(value.ColorScheme);

    public void Show() => IsActive = true;
    public void Hide() => IsActive = false;
}