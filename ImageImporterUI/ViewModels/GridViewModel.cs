using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageImporterUI.ViewModels;

public partial class GridViewModel(MainViewModel main) : ObservableObject
{
    public string Header { get; private set; } = "Grid";

    [ObservableProperty]
    private Image<Rgb, byte> inputImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> debugImage = null!;

    [ObservableProperty]
    private Image<Rgb, byte> gridImage = null!;

    public void Update()
    {
        InputImage = main.puzzle.InputImage;
        DebugImage = main.puzzle.Grid.DebugImage;
        GridImage = main.puzzle.Grid.Image;
    }
}
