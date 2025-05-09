using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ImageImportUI.MVVM;

public partial class MainViewModel : ObservableObject
{
    public static string path = "C:\\Users\\Morten Lang\\source\\repos\\SudokuSolver\\ImageImportTest\\Data\\";

    private readonly ImageImporter importer = new();

    [ObservableProperty]
    private ExtractGridViewModel gridViewModel;

    [ObservableProperty]
    private ExtractCellsViewModel cellsViewModel;

    [ObservableProperty]
    private ExtractDigitsViewModel digitsViewModel;

    [ObservableProperty]
    private RecognizeDigitViewModel recognizeViewModel;

    [ObservableProperty]
    private List<string> imageFilenames;
    
    [ObservableProperty]
    private string selectedImageFilename;

    public MainViewModel()
    {
        GridViewModel = new ExtractGridViewModel(this, importer);
        CellsViewModel = new ExtractCellsViewModel(GridViewModel, importer);
        DigitsViewModel = new ExtractDigitsViewModel(cellsViewModel, importer);
        RecognizeViewModel = new RecognizeDigitViewModel(digitsViewModel, importer);

        ImageFilenames = [.. Directory
            .EnumerateFiles(path)
            .Select(s => Path.GetFileName(s))];
        SelectedImageFilename = ImageFilenames.First();
    }
}
