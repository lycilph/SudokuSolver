using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SudokuUI.Messages;

namespace SudokuUI.ViewModels;

public partial class NotificationViewModel : ObservableRecipient, IRecipient<ShowNotificationMessage>
{
    [ObservableProperty]
    private bool showMessage = false;

    [ObservableProperty]
    private string message = string.Empty;

    public NotificationViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Show(string text)
    {
        ShowMessage = true;
        Message = text;
    }

    public void ReceiveAsync(ShowNotificationMessage message)
    {
        Show(message.Notification);
    }
}
