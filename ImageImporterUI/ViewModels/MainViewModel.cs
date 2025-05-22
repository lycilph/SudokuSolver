using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ImageImporter;
using ImageImporter.Models;
using ImageImporter.Parameters;
using ImageImporterUI.Views;

namespace ImageImporterUI.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<string>
{
    public static string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\Data\\Importer\\";

    private readonly LogViewModel logViewModel;
    private readonly GridViewModel gridViewModel;
    private readonly CellsViewModel cellsViewModel;
    private readonly NumbersViewModel numbersViewModel;

    public readonly Importer importer = new();
    public ImportParameters parameters = new();
    public Puzzle puzzle = new("NA");

    [ObservableProperty]
    private ObservableCollection<string> imageFilenames;

    [ObservableProperty]
    private string selectedImageFilename = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ObservableObject> viewModels = [];

    [ObservableProperty]
    private ObservableObject? selectedViewModel = null;

    [ObservableProperty]
    private string timeElapsed = string.Empty;

    [ObservableProperty]
    private bool isBusy = false;

    public MainViewModel()
    {
        ImageFilenames = [.. Directory
            .EnumerateFiles(path)
            .Where(f => Path.GetExtension(f) == ".jpg")
            .Select(s => Path.GetFileName(s))];

        logViewModel = new LogViewModel(this);
        gridViewModel = new GridViewModel(this);
        cellsViewModel = new CellsViewModel(this);
        numbersViewModel = new NumbersViewModel(this);
        ViewModels = [logViewModel, gridViewModel, cellsViewModel, numbersViewModel];

        IsActive = true; // Needed to recieve messages
    }

    // This is the "windows loaded" message, sent from the main window when everything is loaded and rendered
    public void Receive(string message)
    {
        if (string.IsNullOrWhiteSpace(SelectedImageFilename))
            SelectedImageFilename = ImageFilenames.First();
    }

    partial void OnSelectedImageFilenameChanged(string value)
    {
        _ = Update();
    }

    private async Task Update()
    {
        //IsBusy = true;
        var stop_watch = Stopwatch.StartNew();

        //await Task.Run(() => puzzle = importer.Import(path+SelectedImageFilename, parameters));
        puzzle = importer.Import(path + SelectedImageFilename, parameters);

        logViewModel.Update();
        gridViewModel.Update();
        cellsViewModel.Update();
        numbersViewModel.Update();

        stop_watch.Stop();
        TimeElapsed = $"Elapsed: {stop_watch.ElapsedMilliseconds:f2}ms";
        //IsBusy = false;

        // Update the viewmodels here
        SelectedViewModel = ViewModels.FirstOrDefault();
    }

    [RelayCommand]
    private void CaptureImage()
    {
        var vm = new CaptureImageViewModel(path);
        var capture_image_window = new CaptureImageWindow()
        {
            Owner = App.Current.MainWindow,
            DataContext = vm
        };
        var result = capture_image_window.ShowDialog();

        if (result == true)
        {
            ImageFilenames.Add(vm.Filename);
            SelectedImageFilename = vm.Filename;
        }
    }
}
