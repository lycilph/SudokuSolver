namespace VisualStrategyDebugger.Messages;

public class ShowNotificationMessage(string notification)
{
    public string Notification { get; } = notification;
}
