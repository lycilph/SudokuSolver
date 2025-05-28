using System.Management;
using System.Windows;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using DirectShowLib;
using OpenCvSharp;

namespace PaddleOCRUI;

public partial class CaptureImageViewModel : ObservableObject, IViewAware
{
    private DsDevice[] direct_show_cameras;
    private VideoCapture? video_capture = null;

    public event EventHandler<bool>? OnRequestClose;

    [ObservableProperty]
    private List<string> cameras = [];

    [ObservableProperty]
    private string selectedCamera = string.Empty;

    [ObservableProperty]
    private Mat cameraImage = null!;

    [ObservableProperty]
    private Mat importImage = null!;

    public CaptureImageViewModel()
    {
        direct_show_cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        {
            foreach (var device in searcher.Get())
            {
                Cameras.Add(device["Caption"].ToString() ?? "No name");
            }
        }
    }
    
    private int GetCameraIndexForName(string name)
    {
        for (int i = 0; i < direct_show_cameras.Length; i++)
        {
            if (direct_show_cameras[i].Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                return i;
        }
        return -1;
    }

    partial void OnSelectedCameraChanged(string value)
    {
        if (video_capture != null)
            ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;

        try
        {
            var index = GetCameraIndexForName(SelectedCamera);
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

        CameraImage = video_capture.RetrieveMat().Flip(FlipMode.Y).Clone();
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

        CameraImage?.Dispose();
        ImportImage?.Dispose();
    }

    partial void OnCameraImageChanging(Mat value)
    {
        CameraImage?.Dispose();
    }

    partial void OnImportImageChanging(Mat value)
    {
        ImportImage?.Dispose();
    }
}
