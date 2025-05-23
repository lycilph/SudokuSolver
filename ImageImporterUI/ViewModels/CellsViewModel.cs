using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;
using ImageImporter.Parameters;

namespace ImageImporterUI.ViewModels;

public partial class CellsViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Cells";

    [ObservableProperty]
    private Image<Rgb, byte> inputImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> debugImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> cellsImage = null!;

    [ObservableProperty]
    private ObservableCollection<CellsExtractionParameters> parameters = [];

    [ObservableProperty]
    private CellsExtractionParameters selectedParameters = null!;

    [ObservableProperty]
    private int threshold = 0;

    [ObservableProperty]
    private int iterations = 0;

    public void Update()
    {
        InputImage = main.puzzle.Grid.Image;
        DebugImage = main.puzzle.CellsExtraction.DebugImage;
        CellsImage = main.puzzle.CellsExtraction.Image;

        Parameters = [.. main.parameters.CellsParameters];
        SelectedParameters = main.puzzle.CellsExtraction.ParametersUsed;
    }

    partial void OnSelectedParametersChanged(CellsExtractionParameters value)
    {
        Threshold = SelectedParameters.Threshold;
        Iterations = SelectedParameters.Iterations;
    }

    [RelayCommand]
    public void Add()
    {
        var parameters = new CellsExtractionParameters(0, 0);
        main.parameters.CellsParameters.Add(parameters);

        Parameters.Add(parameters);
        SelectedParameters = parameters;
    }

    [RelayCommand]
    private void Run()
    {
        SelectedParameters.Threshold = Threshold;
        SelectedParameters.Iterations = Iterations;

        _ = main.Update(MainViewModel.UpdateType.Cells);
    }
}
