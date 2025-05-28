using System.Management;
using System.Windows;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using OpenCvSharp;

namespace PaddleOCRUI;

public partial class CaptureImageViewModel : ObservableObject, IViewAware
{
    private VideoCapture? video_capture = null;
    
    public event EventHandler<bool>? OnRequestClose;

    [ObservableProperty]
    private List<string> cameras = [];

    [ObservableProperty]
    private string selectedCamera = string.Empty;

    [ObservableProperty]
    private Mat cameraImage = null!;

    //[ObservableProperty]
    //private Image<Rgb, byte> importImage = null!;

    public CaptureImageViewModel()
    {


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
            var index = cameras.FindIndex(c => c == SelectedCamera);
            video_capture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
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

        CameraImage = video_capture.RetrieveMat().Clone();
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

        if (CameraImage != null)
            CameraImage.Dispose();
    }

    partial void OnCameraImageChanging(Mat value)
    {
        if (CameraImage != null)
            CameraImage.Dispose();
    }
}
