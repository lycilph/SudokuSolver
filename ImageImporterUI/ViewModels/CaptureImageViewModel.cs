using System.Management;
using System.Windows;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporterUI.ViewModels;

public partial class CaptureImageViewModel : ObservableObject, IViewAware
{
    private readonly string path;
    private VideoCapture? video_capture = null;

    public event EventHandler<bool>? OnRequestClose;

    [ObservableProperty]
    private List<string> cameras = [];

    [ObservableProperty]
    private string selectedCamera = string.Empty;

    [ObservableProperty]
    private Image<Rgb, byte> cameraImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> importImage = null!;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private string filename = string.Empty;

    public CaptureImageViewModel(string path)
    {
        this.path = path;

        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        {
            foreach (var device in searcher.Get())
            {
                Cameras.Add(device["Caption"].ToString() ?? "No name");
            }
        }
    }

    partial void OnSelectedCameraChanged(string value)
    {
        if (video_capture != null)
            ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;

        try
        {
            var index = cameras.FindIndex(c => c == selectedCamera);
            video_capture = new VideoCapture(index, VideoCapture.API.DShow);
        }
        catch (NullReferenceException ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }

        ComponentDispatcher.ThreadIdle += CaptureVideoFrame;
    }

    private void CaptureVideoFrame(object? sender, EventArgs e)
    {
        if (video_capture == null)
            return;

        using (var frame = video_capture.QueryFrame().ToImage<Rgb, Byte>())
        {
            CameraImage = frame.Clone();
        }
    }

    public void WindowContentRendered()
    {
        if (Cameras.Count > 0)
            SelectedCamera = Cameras[0];
    }

    public void WindowClosing()
    {
        ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;

        video_capture?.Release();
    }

    [RelayCommand]
    private void Capture()
    {
        ImportImage = CameraImage.Clone();
        Filename = 
            $"WebcamCapture_{DateTime.Now.ToString()}.jpg"
            .Replace("-", "")
            .Replace(":", "")
            .Replace(" ", "_");
    }

    [RelayCommand(CanExecute = nameof(CanImport))]
    private void Import()
    {
        CvInvoke.Imwrite(path+Filename, ImportImage);
        OnRequestClose?.Invoke(this, true);
    }

    private bool CanImport() => !string.IsNullOrWhiteSpace(Filename);
}
