using CommunityToolkit.Mvvm.ComponentModel;
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
    private List<CellsExtractionParameters> parameters = [];

    [ObservableProperty]
    private CellsExtractionParameters selectedParameters;

    public void Update()
    {
        InputImage = main.puzzle.Grid.Image;
        DebugImage = main.puzzle.CellsExtraction.DebugImage;
        CellsImage = main.puzzle.CellsExtraction.Image;

        Parameters = main.parameters.CellsParameters;
        SelectedParameters = main.puzzle.CellsExtraction.ParametersUsed;
    }
}
