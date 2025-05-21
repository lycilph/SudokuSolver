using System.Management;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporterUI.ViewModels;

public partial class CaptureImageViewModel : ObservableObject
{
    private VideoCapture? video_capture = null;

    [ObservableProperty]
    private List<string> cameras = [];

    [ObservableProperty]
    private string selectedCamera = string.Empty;

    [ObservableProperty]
    private BitmapSource bitmapSource = null!;

    public CaptureImageViewModel()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        {
            foreach (var device in searcher.Get())
            {
                cameras.Add(device["Caption"].ToString() ?? "No name");
            }
        }

        if (cameras.Count > 0)
            SelectedCamera = cameras[0];

    }

    partial void OnSelectedCameraChanged(string value)
    {
        if (video_capture != null)
            ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;

        try
        {
            var index = cameras.FindIndex(c => c == selectedCamera);
            video_capture = new VideoCapture(index, VideoCapture.API.DShow);

            //video_capture = new VideoCapture(0, VideoCapture.API.DShow); // External webcam
            //video_capture = new VideoCapture(1, VideoCapture.API.DShow); // Labtop webcam
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

        using (var frame = video_capture.QueryFrame().ToImage<Bgr, Byte>())
        {
            BitmapSource = frame.ToBitmapSource();
        }
    }
}
