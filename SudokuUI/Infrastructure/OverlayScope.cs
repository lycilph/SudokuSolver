using SudokuUI.ViewModels;

namespace SudokuUI.Infrastructure;

public class OverlayScope : IDisposable
{
    private readonly OverlayViewModel overlay;
    private bool disposed = false;

    public Task OpenAnimationTask { get; private set; }

    // Constructor takes the overlay instance and calls Show
    public OverlayScope(OverlayViewModel overlay, bool show_spinner, string message)
    {
        this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
        OpenAnimationTask = this.overlay.Show(show_spinner, message); // Call the Show method when the object is created
    }

    // Dispose method to call Hide
    public void Dispose()
    {
        if (!disposed)
        {
            overlay.Hide(); // Call the Hide method when disposing
            disposed = true;
        }
    }
}
