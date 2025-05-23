﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Emgu.CV;
using ImageImporter;
using ImageImporter.Models;
using ImageImporter.Parameters;
using ImageImporterUI.Views;

namespace ImageImporterUI.ViewModels;

public partial class MainViewModel : ObservableRecipient, IRecipient<string>
{
    public static string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\Data\\Importer\\";

    public enum UpdateType { All, Grid, Cells };

    private readonly LogViewModel logViewModel;
    private readonly GridViewModel gridViewModel;
    private readonly CellsViewModel cellsViewModel;
    private readonly NumbersViewModel numbersViewModel;
    private readonly ImproveNumberRecognitionViewModel improveNumberRecognitionViewModel;

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
        improveNumberRecognitionViewModel = new ImproveNumberRecognitionViewModel(this);
        ViewModels = [logViewModel, gridViewModel, cellsViewModel, numbersViewModel, improveNumberRecognitionViewModel];

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
        _ = Update(UpdateType.All);
    }

    public async Task Update(UpdateType type)
    {
        IsBusy = true;
        var stop_watch = Stopwatch.StartNew();

        switch (type)
        {
            case UpdateType.All:
                // Do the entire import + recognition
                await Task.Run(() => puzzle = importer.Import(path + SelectedImageFilename, parameters));

                //foreach (var cell in puzzle.Cells)
                //{
                //    var filename = path + "Cells\\" + $"cell{cell.Id}.jpg";
                //    CvInvoke.Imwrite(filename, cell.Image);
                //}
                //foreach (var num in puzzle.Numbers)
                //{
                //    var filename = path + "Cells\\" + $"cell_processed{num.Cell.Id}.jpg";
                //    CvInvoke.Imwrite(filename, num.ImageProcessed);
                //}

                // Update all individual vms with new puzzle
                logViewModel.Update();
                gridViewModel.Update();
                cellsViewModel.Update();
                numbersViewModel.Update();
                improveNumberRecognitionViewModel.Update();
                // Update the viewmodels here
                SelectedViewModel = ViewModels.FirstOrDefault();
                break;
            case UpdateType.Grid:
                // Do only the grid extraction
                await Task.Run(() => importer.ExtractGrid(puzzle, parameters.GridParameters));
                // Update relevant individual vms with new puzzle
                gridViewModel.Update();
                break;
            case UpdateType.Cells:
                // Do only the cells extraction
                await Task.Run(() => importer.ExtractCells(puzzle, cellsViewModel.SelectedParameters));
                // Update relevant individual vms with new puzzle
                cellsViewModel.Update();
                break;
        }

        stop_watch.Stop();
        TimeElapsed = $"Elapsed: {stop_watch.ElapsedMilliseconds:f2}ms";
        IsBusy = false;
    }

    [RelayCommand]
    private void UpdateAll()
    {
        _ = Update(UpdateType.All);
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
