using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    private int margin = 0;

    [ObservableProperty]
    private int output = 0;

    public void Update()
    {
        InputImage = main.puzzle.InputImage;
        DebugImage = main.puzzle.Grid.DebugImage;
        GridImage = main.puzzle.Grid.Image;

        Margin = main.parameters.GridParameters.Margin;
        Output = main.parameters.GridParameters.OutputSize;
    }

    [RelayCommand]
    private void Run()
    {
        main.parameters.GridParameters.Margin = Margin;
        main.parameters.GridParameters.OutputSize = Output;

        _ = main.Update(MainViewModel.UpdateType.Grid);
    }
}
