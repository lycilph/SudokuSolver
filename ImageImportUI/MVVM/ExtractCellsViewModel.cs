using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV.Structure;
using Emgu.CV;
using CommunityToolkit.Mvvm.Input;

namespace ImageImportUI.MVVM;

public partial class ExtractCellsViewModel : ObservableObject
{
    private readonly ImageImporter importer;

    [ObservableProperty]
    private ExtractGridViewModel gridVM;

    [ObservableProperty]
    private Image<Rgb, byte> cellsImage = null!;

    [ObservableProperty]
    private int lowerThreshold = 5;

    [ObservableProperty]
    private int iterations = 1;

    [ObservableProperty]
    private int cellsCount = 0;

    [ObservableProperty]
    private List<Cell> cells = [];

    public ExtractCellsViewModel(ExtractGridViewModel gridVM, ImageImporter importer)
    {
        this.importer = importer;

        GridVM = gridVM;

        GridVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ExtractGridViewModel.GridImage))
            {
                Update();
            }
        };
    }

    [RelayCommand]
    private void Update()
    {
        (CellsImage, Cells, CellsCount) = importer.ExtractCells(GridVM.GridImage, LowerThreshold, Iterations, false);
    }
}
