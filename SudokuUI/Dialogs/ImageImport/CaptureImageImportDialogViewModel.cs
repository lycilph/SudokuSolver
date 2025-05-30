using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectShowLib;
using OpenCvSharp;
using System.Management;
using System.Windows;
using System.Windows.Interop;

namespace SudokuUI.Dialogs.ImageImport;

public partial class CaptureImageImportDialogViewModel : ObservableObject, IDialogViewModel<ImportStepOutput>
{
    private readonly TaskCompletionSource<ImportStepOutput> task_completion_source;

    private DsDevice[] direct_show_cameras;
    private VideoCapture? video_capture = null;
    
    public Task<ImportStepOutput> DialogResult => task_completion_source.Task;
    
    [ObservableProperty]
    private List<string> cameras = [];

    [ObservableProperty]
    private string selectedCamera = string.Empty;

    [ObservableProperty]
    private Mat cameraImage = null!;

    [ObservableProperty]
    private Mat importImage = null!;

    public CaptureImageImportDialogViewModel()
    {
        task_completion_source = new TaskCompletionSource<ImportStepOutput>();

        direct_show_cameras = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        {
            foreach (var device in searcher.Get())
            {
                Cameras.Add(device["Caption"].ToString() ?? "No name");
            }
        }
    }

    public void Initialize()
    {
        // This is called by the view when it is ready to be displayed.
        if (Cameras.Count > 0)
            SelectedCamera = Cameras.First();
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

        CameraImage = video_capture.RetrieveMat().Clone();
    }

    partial void OnCameraImageChanging(Mat value)
    {
        CameraImage?.Dispose();
    }

    partial void OnImportImageChanging(Mat value)
    {
        ImportImage?.Dispose();
    }

    [RelayCommand]
    private void Capture()
    {
        ImportImage = CameraImage.Clone();
    }

    [RelayCommand]
    private void Next()
    {
        task_completion_source.SetResult(ImportStepOutput.Next(ImportImage));
        ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;
        CameraImage?.Dispose();
    }

    [RelayCommand]
    private void Cancel()
    {
        task_completion_source.SetResult(ImportStepOutput.Cancel());
        ComponentDispatcher.ThreadIdle -= CaptureVideoFrame;
        CameraImage?.Dispose();
        ImportImage?.Dispose();
    }
}
