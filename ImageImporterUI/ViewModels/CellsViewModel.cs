using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporterUI.ViewModels;

public partial class CellsViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Cells";

    [ObservableProperty]
    private Image<Rgb, byte> inputImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> cellsImage = null!;

    public void Update()
    {
        InputImage = main.puzzle.Grid.Image;
        CellsImage = main.puzzle.CellsExtraction.Image;
    }
}
