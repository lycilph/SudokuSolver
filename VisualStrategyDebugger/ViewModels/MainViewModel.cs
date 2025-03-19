using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VisualStrategyDebugger.Messages;

namespace VisualStrategyDebugger.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<ShowNotificationMessage>
{
    [ObservableProperty]
    private GridViewModel gridViewModel;

    [ObservableProperty]
    private CommandManagerViewModel commandManagerViewModel;

    [ObservableProperty]
    private StrategyManagerViewModel strategyManagerViewModel;

    [ObservableProperty]
    private GridManagerViewModel gridManagerViewModel;

    [ObservableProperty]
    private bool showMessage = false;

    [ObservableProperty]
    private string notification = string.Empty;

    public MainViewModel(GridViewModel gridViewModel,
                         CommandManagerViewModel commandManagerViewModel,
                         StrategyManagerViewModel strategyManagerViewModel,
                         GridManagerViewModel gridManagerViewModel)
    {
        GridViewModel = gridViewModel;
        CommandManagerViewModel = commandManagerViewModel;
        StrategyManagerViewModel = strategyManagerViewModel;
        GridManagerViewModel = gridManagerViewModel;

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(ShowNotificationMessage message)
    {
        Notification = message.Notification;
        ShowMessage = true;
    }

    [RelayCommand]
    private void Test()
    {
        Notification = "Test notification";
        ShowMessage = true;
    }
}