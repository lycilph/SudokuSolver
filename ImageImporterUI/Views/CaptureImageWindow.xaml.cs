using System.Management;
using System.Windows;
using System.Windows.Interop;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporterUI.Views;

public partial class CaptureImageWindow : Window
{
    private VideoCapture video_capture = null!;

    public CaptureImageWindow()
    {
        InitializeComponent();

        //var cameraNames = new List<string>();
        //using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        //{
        //    foreach (var device in searcher.Get())
        //    {
        //        cameraNames.Add(device["Caption"].ToString());
        //    }
        //}

        if (video_capture == null)
        {
            try
            {
                //video_capture = new VideoCapture(0, VideoCapture.API.DShow); // External webcam
                video_capture = new VideoCapture(1, VideoCapture.API.DShow); // Labtop webcam
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // https://stackoverflow.com/questions/1111615/getting-inactivity-idle-time-in-a-wpf-application
        ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
    }

    private void ComponentDispatcher_ThreadIdle(object? sender, EventArgs e)
    {
        using (var frame = video_capture.QueryFrame().ToImage<Bgr, Byte>())
        {
            captured_image.Source = frame.ToBitmapSource();
        }
    }
}
